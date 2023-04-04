using System.Threading.Tasks;
using Nest;

namespace Tools.Elastic
{
    public interface IAliasProvider
    {
        /// <summary>
        ///     添加别名
        /// </summary>
        /// <param name="index"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        Task<BulkAliasResponse> AddAliasAsync(string alias);

        /// <summary>
        ///     删除别名
        /// </summary>
        /// <param name="index"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        BulkAliasResponse RemoveAlias(string alias);

        //別名滚动
        //说明：
        //如果别名logs_write指向的索引是7天前（含）创建的或索引的文档数>=1000或索引的大小>= 5gb，则会创建一个新索引 logs-000002，并把别名logs_writer指向新创建的logs-000002索引
        //注意：rollover是你请求它才会进行操作，并不是自动在后台进行的。你可以周期性地去请求它。
        //await elasticClient.Indices.RolloverAsync("ab", x => x.Conditions(m => m.MaxAge(TimeSpan.FromDays(7)).MaxDocs(1000)));
    }
}