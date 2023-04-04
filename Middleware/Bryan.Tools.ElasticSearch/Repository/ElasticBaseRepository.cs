using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Nest;

namespace Tools.Elastic
{
    public abstract class ElasticBaseRepository<T> : IElasticRepository<T> where T : ElasticEntity
    {
        private readonly IElasticClient _elasticClient;

        public string IndexName { get; set; }

        public ElasticBaseRepository(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
            IndexName = IndexHelper.GetIndexName<T>();
        }

        #region Delete

        public async Task<DeleteByQueryResponse> DeleteByQueryAsync(Expression<Func<T, bool>> expression)
        {
            var request = new DeleteByQueryRequest<T>(IndexName);
            var build = new QueryBuilder<T>();
            request.Query = build.GetQueryContainer(expression);
            var response = await _elasticClient.DeleteByQueryAsync(request);
            if (!response.IsValid)
                throw new Exception("删除失败:" + response.OriginalException.Message);
            return response;
        }

        public async Task<bool> DeleteByQueryAsync(Func<QueryContainerDescriptor<T>, QueryContainer> selector)
        {
            var delete = new DeleteByQueryDescriptor<T>(IndexName)
                .Query(selector);

            var response = await _elasticClient.DeleteByQueryAsync(delete);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return true;
        }

        #endregion

        #region Update

        public async Task<bool> UpdateAsync(string key, T entity)
        {
            var request = new UpdateRequest<T, object>(IndexName, key)
            {
                Doc = entity
            };

            var response = await _elasticClient.UpdateAsync(request);
            if (!response.IsValid)
                throw new Exception("更新失败:" + response.OriginalException.Message);
            return true;
        }

        #endregion

        #region Index

        public async Task<bool> IndexExistsAsync()
        {
            var res = await _elasticClient.Indices.ExistsAsync(IndexName);
            return res.Exists;
        }

        public async Task InsertAsync(T entity)
        {
            var exists = await IndexExistsAsync();
            if (!exists)
            {
                await ((ElasticClient)_elasticClient).CreateIndexAsync<T>(IndexName);
            }

            var response = await _elasticClient.IndexAsync(entity, s => s.Index(IndexName));

            if (!response.IsValid)
                throw new Exception("新增数据失败:" + response.OriginalException.Message);
        }

        public async Task InsertManyAsync(IEnumerable<T> entity)
        {
            var exists = await IndexExistsAsync();
            if (!exists)
            {
                await ((ElasticClient)_elasticClient).CreateIndexAsync<T>(IndexName);
                await AddAliasAsync(typeof(T).Name);
            }

            var bulkRequest = new BulkRequest(IndexName)
            {
                Operations = new List<IBulkOperation>()
            };
            var operations = entity.Select(o => new BulkIndexOperation<T>(o)).Cast<IBulkOperation>().ToList();
            bulkRequest.Operations = operations;
            var response = await _elasticClient.BulkAsync(bulkRequest);

            if (!response.IsValid)
                throw new Exception("批量新增数据失败:" + response.OriginalException.Message);
        }

        public async Task DeleteIndexAsync()
        {
            var exists = await IndexExistsAsync();
            if (!exists) return;
            var response = await _elasticClient.Indices.DeleteAsync(IndexName);

            if (!response.IsValid)
                throw new Exception("删除index失败:" + response.OriginalException.Message);
        }

        #endregion

        #region Alias

        public async Task<BulkAliasResponse> AddAliasAsync(string alias)
        {
            var response = await _elasticClient.Indices.BulkAliasAsync(b => b.Add(al => al
                .Index(IndexName)
                .Alias(alias)));

            if (!response.IsValid)
                throw new Exception("添加Alias失败:" + response.OriginalException.Message);
            return response;
        }


        public BulkAliasResponse RemoveAlias(string alias)
        {
            var response = _elasticClient.Indices.BulkAlias(b => b.Remove(al => al
                .Index(IndexName)
                .Alias(alias)));

            if (!response.IsValid && response.ApiCall.HttpStatusCode != (int)HttpStatusCode.NotFound)
                throw new Exception("删除Alias失败:" + response.OriginalException?.Message);
            return response;
        }

        #endregion

        #region Search

        public IEsQueryable<T> Queryable()
        {
            return new QueryableProvider<T>(_elasticClient);
        }

        public async Task<long> GetTotalCountAsync(Func<QueryContainerDescriptor<T>, QueryContainer> request)
        {
            var search = new SearchDescriptor<T>(IndexName).Query(request);
            var response = await _elasticClient.SearchAsync<T>(search);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Total;
        }

        public async Task<bool> ExistAsync(string id)
        {
            var response = await _elasticClient.DocumentExistsAsync(DocumentPath<T>.Id(id).Index(IndexName));

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Exists;
        }

        public async Task<T> GetAsync(string id)
        {
            var response = await _elasticClient.GetAsync(DocumentPath<T>.Id(id).Index(IndexName));
            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Source;
        }

        public async Task<T> GetAsync(IGetRequest request)
        {
            var response = await _elasticClient.GetAsync<T>(request);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Source;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var search = new SearchDescriptor<T>(IndexName)
                .MatchAll();

            var response = await _elasticClient.SearchAsync<T>(search);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Hits.Select(hit => hit.Source).ToList();
        }

        public async Task<IEnumerable<T>> GetManyAsync(IEnumerable<string> ids)
        {
            var response = await _elasticClient.GetManyAsync<T>(ids, IndexName);

            return response.Select(item => item.Source).ToList();
        }

        public async Task<IEnumerable<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> request, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null)
        {
            var search = new SearchDescriptor<T>(IndexName)
                .Query(request);

            if (sort != null)
            {
                search.Sort(sort);
            }

            var response = await _elasticClient.SearchAsync<T>(search);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Hits.Select(hit => hit.Source).ToList();
        }

        public async Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> request,
            Func<AggregationContainerDescriptor<T>, IAggregationContainer> aggregationsSelector)
        {
            var search = new SearchDescriptor<T>(IndexName)
                .Query(request)
                .Aggregations(aggregationsSelector);

            var response = await _elasticClient.SearchAsync<T>(search);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response;
        }

        public async Task<IEnumerable<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector)
        {
            var list = new List<T>();
            var response = await _elasticClient.SearchAsync(selector);

            if (!response.IsValid)
                throw new Exception(response.ServerError?.ToString(), response.OriginalException);

            return response.Hits.Select(hit => hit.Source).ToList();
        }

        public async Task<IEnumerable<T>> SearchInAllFields(string term)
        {
            var search = new SearchDescriptor<T>(IndexName)
                .Query(p => NestExtensions.BuildMultiMatchQuery<T>(term));

            var result = await _elasticClient.SearchAsync<T>(search);

            return result.Documents.ToList();
        }

        #endregion

    }
}