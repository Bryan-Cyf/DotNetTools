using Nest;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tools.Elastic
{
    public interface IDeleteProvider<T> where T : ElasticEntity
    {
        Task<DeleteByQueryResponse> DeleteByQueryAsync(Expression<Func<T, bool>> expression);

        Task<bool> DeleteByQueryAsync(Func<QueryContainerDescriptor<T>, QueryContainer> selector);
    }
}
