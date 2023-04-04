using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.Mongo
{
    /// <summary>
    /// 用于di
    /// </summary>
    public interface IMongoRepository
    {
        Task CreateIndexesAsync();
    }

    public interface IMongoRepository<T> : IMongoRepository where T : MongoEntity
    {
        #region 查询
        Task<T> FirstOrDefaultAsync(FilterDefinition<T> filter, Expression<Func<T, object>> order = null, bool isDescending = true, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> ConfigureProjection = default);
        Task<List<T>> FindAsync(FilterDefinition<T> filter, Expression<Func<T, object>> order, bool isDescending, int? pageIndex, int? size, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> ConfigureProjection);
        //Task<List<T>> SelectAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> configureFilter = default, Action<IFindFluent<T, T>> configureQuery = default, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> configureProjection = default);
        #endregion

        #region 删除
        Task<DeleteResult> DeleteOneAsync(FilterDefinition<T> filter);
        Task<DeleteResult> DeleteManyAsync(FilterDefinition<T> filter);
        #endregion

        #region 新增
        Task InsertOneAsync(T item);
        Task InsertManyAsync(IEnumerable<T> items, InsertManyOptions options = null, CancellationToken cancellationToken = default, int? batchSize = default);
        #endregion

        #region 更新
        //Task<UpdateResult> UpsertOneAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> configureFilter, Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> configureUpdate);
        //Task<UpdateResult> UpsertManyAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> configureFilter, Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> configureUpdate);
        //Task<UpdateResult> UpdateOneAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> configureFilter, Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> configureUpdate);
        //Task<UpdateResult> UpdateManyAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> configureFilter, Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> configureUpdate);
        #endregion

        #region 统计
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
        Task<long> CountAsync(FilterDefinition<T> filter);
        #endregion

    }
}
