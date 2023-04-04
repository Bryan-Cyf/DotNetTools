using Nest;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tools.Elastic
{
    public interface ISearchProvider<T> where T : ElasticEntity
    {
        IEsQueryable<T> Queryable();

        Task<T> GetAsync(string id);

        Task<T> GetAsync(IGetRequest request);

        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetManyAsync(IEnumerable<string> ids);

        Task<IEnumerable<T>> SearchAsync(Func<SearchDescriptor<T>, ISearchRequest> selector);

        Task<IEnumerable<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> request, Func<SortDescriptor<T>, IPromise<IList<ISort>>> sort = null);

        Task<ISearchResponse<T>> SearchAsync(Func<QueryContainerDescriptor<T>, QueryContainer> request, Func<AggregationContainerDescriptor<T>, IAggregationContainer> aggregationsSelector);

        Task<IEnumerable<T>> SearchInAllFields(string term);

        Task<bool> ExistAsync(string id);

        Task<long> GetTotalCountAsync(Func<QueryContainerDescriptor<T>, QueryContainer> request);
    }
}