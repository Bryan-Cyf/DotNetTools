using System.Threading.Tasks;
using Nest;

namespace Tools.Elastic
{
    public interface IElasticRepository
    {

    }
    public interface IElasticRepository<T> : IIndexProvider<T>, IDeleteProvider<T>, IUpdateProvider<T>, IAliasProvider, ISearchProvider<T>, IElasticRepository where T : ElasticEntity
    {

    }
}