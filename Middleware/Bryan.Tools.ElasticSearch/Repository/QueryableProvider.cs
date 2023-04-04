using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nest;

namespace Tools.Elastic
{
    public class QueryableProvider<T> : IEsQueryable<T> where T : ElasticEntity
    {
        private readonly IElasticClient _client;

        private readonly MappingIndex _mappingIndex;

        private readonly ISearchRequest _request;

        private long _totalNumber;

        private QueryBuilder<T> _queryBuilder;

        public QueryableProvider(IElasticClient client)
        {
            _queryBuilder = new QueryBuilder<T>();
            _mappingIndex = _queryBuilder.GetMappingIndex();
            _request = new SearchRequest(_mappingIndex.IndexName)
            {
                Size = 10000
            };
            _client = client;
        }

        public IEsQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            _request.Query = _queryBuilder.GetQueryContainer(expression);
            return this;
        }

        public virtual List<T> ToList()
        {
            return _ToList<T>();
        }

        public async Task<T> FirstAsync()
        {
            var result = await _ToListAsync<T>();
            return result.FirstOrDefault();
        }

        public virtual async Task<List<T>> ToListAsync()
        {
            return await _ToListAsync<T>();
        }

        public virtual List<T> ToPageList(int pageIndex, int pageSize)
        {
            _request.From = ((pageIndex < 1 ? 1 : pageIndex) - 1) * pageSize;
            _request.Size = pageSize;
            return _ToList<T>();
        }

        public virtual List<T> ToPageList(int pageIndex, int pageSize, out long totalNumber)
        {
            var list = ToPageList(pageIndex, pageSize);
            totalNumber = _totalNumber;
            return list;
        }

        public virtual IEsQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc)
        {
            _OrderBy(expression, type);
            return this;
        }

        public IEsQueryable<T> GroupBy(Expression<Func<T, object>> expression)
        {
            _GroupBy(expression);
            return this;
        }

        private void _GroupBy(Expression expression)
        {
            var propertyName = ReflectionExtensionHelper.GetProperty(expression as LambdaExpression).Name;
            propertyName = _mappingIndex.Columns.FirstOrDefault(x => x.PropertyName == propertyName)?.SearchName ?? propertyName;
            _request.Aggregations = new TermsAggregation(propertyName)
            {
                Field = propertyName,
                Size = 1000
            };
        }

        private void _OrderBy(Expression expression, OrderByType type = OrderByType.Asc)
        {
            var propertyName = ReflectionExtensionHelper.GetProperty(expression as LambdaExpression).Name;
            propertyName = _mappingIndex.Columns.FirstOrDefault(x => x.PropertyName == propertyName)?.SearchName ?? propertyName;
            _request.Sort = new ISort[]
            {
                new FieldSort
                {
                    Field = propertyName,
                    Order = type == OrderByType.Asc ? SortOrder.Ascending : SortOrder.Descending
                }
            };
        }

        private List<TResult> _ToList<TResult>() where TResult : class
        {
            var response = _client.Search<TResult>(_request);

            if (!response.IsValid)
                throw new Exception($"查询失败:{response.OriginalException.Message}");

            _totalNumber = response.Total;
            return response.Documents.ToList();
        }

        private async Task<List<TResult>> _ToListAsync<TResult>() where TResult : class
        {
            var response = await _client.SearchAsync<TResult>(_request);

            if (!response.IsValid)
                throw new Exception($"查询失败:{response.OriginalException?.Message}");
            _totalNumber = response.Total;
            return response.Documents.ToList();
        }
    }
}