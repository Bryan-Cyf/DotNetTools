using System.Threading.Tasks;
using Nest;

namespace Tools.Elastic
{
    public interface IUpdateProvider<T> where T : ElasticEntity
    {
        Task<bool> UpdateAsync(string key, T entity);
    }
}
