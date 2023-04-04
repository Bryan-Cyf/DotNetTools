using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tools.Elastic
{
    public interface IIndexProvider<T> where T : ElasticEntity
    {
        /// <summary>
        /// 新增数据
        /// </summary>
        Task InsertAsync(T entity);

        /// <summary>
        /// 批量新增
        /// </summary>
        Task InsertManyAsync(IEnumerable<T> entity);

        /// <summary>
        /// 判断索引是否存在
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<bool> IndexExistsAsync();

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <returns></returns>
        Task DeleteIndexAsync();
    }
}