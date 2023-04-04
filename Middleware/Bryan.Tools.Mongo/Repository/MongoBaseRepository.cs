using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Tools.Mongo
{
    /// <summary>
    /// mongo仓储
    /// </summary>
    public abstract class MongoBaseRepository<T> : IMongoRepository<T> where T : MongoEntity
    {
        /// <summary>
        /// 获取集合名称
        /// </summary>
        protected string CollectionName { get; set; }

        /// <summary>
        /// mongo客户端
        /// </summary>
        protected IMongoClient _mongoClient => _mongoDb?.Client;

        /// <summary>
        /// mongo数据库
        /// </summary>
        protected readonly IMongoDatabase _mongoDb;

        /// <summary>
        /// mongo集合,根据配置生成
        /// </summary>
        protected IMongoCollection<T> _mongoColl => CreateMongoCollection();

        public IMongoQueryable<T> AsQueryable(AggregateOptions aggregateOptions = null) => _mongoColl.AsQueryable(aggregateOptions);

        /// <summary>
        /// mongo集合,优先读从库
        /// </summary>
        protected IMongoCollection<T> _mongoCollWithSecondaryPreferred => _mongoDb.GetCollection<T>(CollectionName, new MongoCollectionSettings
        {
            ReadPreference = new ReadPreference(ReadPreferenceMode.SecondaryPreferred)
        });

        /// <summary>
        /// mongo集合,只读主库
        /// </summary>
        protected IMongoCollection<T> _mongoCollWithPrimary => _mongoDb.GetCollection<T>(CollectionName, new MongoCollectionSettings
        {
            ReadPreference = new ReadPreference(ReadPreferenceMode.Primary)
        });

        /// <summary>
        /// 执行策略,默认异常重试三次
        /// </summary>
        protected AsyncPolicy _policy { get; set; }

        /// <summary>
        /// 创建集合的配置
        /// </summary>
        protected MongoCollectionSettings _mongoCollectionSettings { get; set; }

        #region Builders
        public static FilterDefinitionBuilder<T> Filter { get; } = Builders<T>.Filter;
        public static ProjectionDefinitionBuilder<T> Project { get; } = Builders<T>.Projection;
        public static UpdateDefinitionBuilder<T> Update { get; } = Builders<T>.Update;
        public static IndexKeysDefinitionBuilder<T> IndexKeys { get; } = Builders<T>.IndexKeys;
        public static List<UpdateDefinition<T>> CreateUpdateDefinitionList() => new List<UpdateDefinition<T>>();
        public static SortDefinitionBuilder<T> Sort { get; } = Builders<T>.Sort;
        #endregion

        public MongoBaseRepository(IMongoDatabase mongoDatabase)
        {
            _mongoDb = mongoDatabase;
            CollectionName = CollectionHelper.GetCollectionName<T>();

            _policy = Policy.Handle<Exception>(x => !(x is System.FormatException && !(x is MongoDB.Driver.MongoCommandException)))
                .WaitAndRetryAsync(3, x => TimeSpan.FromSeconds(1), (ex, time, index, context) =>
                {
                });
        }
        #region 新增
        public Task InsertOneAsync(T item)
        {
            if (item == null) throw new ArgumentNullException();
            item.CreateOn ??= DateTime.Now;
            return ReRun(() => CreateMongoCollection().InsertOneAsync(item));
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="batchSize">分批次插入,可避免BSONObj size异常</param>
        public async Task InsertManyAsync(IEnumerable<T> items, InsertManyOptions options = null, CancellationToken cancellationToken = default, int? batchSize = default)
        {
            if (items == null || items.Count() == 0) return;
            foreach (var item in items)
            {
                item.CreateOn ??= DateTime.Now;
            }
            options ??= new InsertManyOptions();
            //options.IsOrdered = false;//异常会继续插入

            //解决BSONObj size限制
            if (batchSize != default)
            {
                var temp = new List<T>();
                foreach (var item in items)
                {
                    temp.Add(item);
                    if (temp.Count % batchSize == 0)
                    {
                        await ReRun(() => CreateMongoCollection().InsertManyAsync(temp, options, cancellationToken)).ConfigureAwait(false);
                        temp.Clear();
                    }
                }
                if (temp.Count > 0)
                {
                    await ReRun(() => CreateMongoCollection().InsertManyAsync(temp, options, cancellationToken)).ConfigureAwait(false);
                }
            }
            else
            {
                await ReRun(() => CreateMongoCollection().InsertManyAsync(items, options, cancellationToken)).ConfigureAwait(false);
            }
        }
        #endregion

        #region 统计
        public Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
        {
            return ExistszAsync(x => x.Where(filter));
        }

        public async Task<bool> ExistszAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> configureFilter)
        {
            var totalCount = await CountzAsync(configureFilter).ConfigureAwait(false);
            return (totalCount > 0) ? true : false;
        }

        public Task<long> CountAsync(FilterDefinition<T> filter)
        {
            return CountzAsync(_ => filter);
        }

        public Task<long> CountAsync(Expression<Func<T, bool>> filter)
        {
            return CountzAsync(x => x.Where(filter));
        }

        public Task<long> CountzAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> configureFilter)
        {
            return ReRun(() => CreateMongoCollection().CountDocumentsAsync(configureFilter(Filter)));
        }

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsCollectionExistsAsync()
        {
            var filter = new BsonDocument("name", CollectionName);
            var collections = await _mongoDb.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter }).ConfigureAwait(false);
            return await collections.AnyAsync().ConfigureAwait(false);
        }
        #endregion

        #region 删除
        public Task<DeleteResult> DeleteOneAsync(FilterDefinition<T> filter)
        {
            return DeleteOnezAsync(_ => filter);
        }
        public Task<DeleteResult> DeleteOneAsync(Expression<Func<T, bool>> filter)
        {
            return DeleteOnezAsync(_ => filter);
        }
        public Task<DeleteResult> DeleteOnezAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> configureFilter)
        {
            return ReRun(() => CreateMongoCollection().DeleteOneAsync(configureFilter(Filter)));
        }
        public Task<DeleteResult> DeleteManyAsync(FilterDefinition<T> filter)
        {
            return DeleteManyzAsync(_ => filter);
        }
        public Task<DeleteResult> DeleteManyAsync(Expression<Func<T, bool>> filter)
        {
            return DeleteManyzAsync(_ => filter);
        }
        public Task<DeleteResult> DeleteManyzAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> configureFilter)
        {
            return ReRun(() => CreateMongoCollection().DeleteManyAsync(configureFilter(Filter)));
        }

        public Task<T> FindOneAndDeleteAsync(Expression<Func<T, bool>> filter, FindOneAndDeleteOptions<T, T> options = null)
        {
            return ReRun(() => CreateMongoCollection().FindOneAndDeleteAsync(filter, options));
        }
        #endregion

        #region 查询
        public IFindFluent<T, T> Find(FilterDefinition<T> filter)
        {
            return CreateMongoCollection().Find(filter);
        }
        public IFindFluent<T, T> Find(Expression<Func<T, bool>> filter)
        {
            return CreateMongoCollection().Find(filter);
        }

        /// <summary>
        /// 根据id获取数据
        /// </summary>
        /// <param name="id">id</param>
        /// <returns>一条数据</returns>
        public Task<T> GetAsync(string id)
        {
            return FirstOrDefaultzAsync(f => f.Where(x => x.Id == id));
        }

        /// <summary>
        /// 查询第一条记录
        /// </summary>
        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> filter = default, Expression<Func<T, object>> order = null, bool isDescending = true, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> ConfigureProjection = default)
        {
            return FirstOrDefaultAsync(Find(filter), order, isDescending, ConfigureProjection);
        }

        /// <summary>
        /// 查询第一条记录
        /// </summary>
        public Task<T> FirstOrDefaultAsync(FilterDefinition<T> filter, Expression<Func<T, object>> order = null, bool isDescending = true, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> ConfigureProjection = default)
        {
            return FirstOrDefaultAsync(Find(filter), order, isDescending, ConfigureProjection);
        }

        public Task<T> FirstOrDefaultzAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter, Func<SortDefinitionBuilder<T>, SortDefinition<T>> sort = default, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> project = default)
        {
            var query = Find(filter(Filter));
            if (sort != default)
            {
                query = query.Sort(sort(Sort));
            }

            if (project != default)
            {
                query = query.Project<T>(project(Project));
            }
            return query.FirstOrDefaultAsync();
        }

        private Task<T> FirstOrDefaultAsync(IFindFluent<T, T> query, Expression<Func<T, object>> order = null, bool isDescending = true, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> ConfigureProjection = default)
        {
            if (order != null)
            {
                query = isDescending ? query.SortByDescending(order) : query.SortBy(order);
            }

            if (ConfigureProjection != default)
            {
                query = query.Project<T>(ConfigureProjection(Project));
            }

            return ReRun(() => query.FirstOrDefaultAsync());
        }

        /// <summary>
        /// 分页查询,默认返回全部
        /// </summary>
        ///<param name="pageIndex">页数,从1开始</param>
        public Task<List<T>> FindAsync(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order = null, bool isDescending = true, int? pageIndex = default, int? size = default, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> configureProjection = default)
        {
            return FindAsync(Find(filter), order: order, isDescending: isDescending, pageIndex: pageIndex, size: size, configureProjection: configureProjection);
        }

        /// <summary>
        /// 分页查询,默认返回全部
        /// </summary>
        ///<param name="pageIndex">页数,从1开始</param>
        public Task<List<T>> FindAsync(FilterDefinition<T> filter = default, Expression<Func<T, object>> order = default, bool isDescending = true, int? pageIndex = default, int? size = default, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> ConfigureProjection = default)
        {
            return FindAsync(Find(filter ?? Filter.Empty), order: order, isDescending: isDescending, pageIndex: pageIndex, size: size, configureProjection: ConfigureProjection);
        }

        private Task<List<T>> FindAsync(IFindFluent<T, T> query, Expression<Func<T, object>> order = default, bool isDescending = true, int? pageIndex = default, int? size = default, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> configureProjection = default)
        {
            if (order != null)
            {
                query = isDescending ? query.SortByDescending(order) : query.SortBy(order);
            }

            if (configureProjection != default)
            {
                query = query.Project<T>(configureProjection(Project));
            }

            if (pageIndex != default)
            {
                query = query.Skip((pageIndex - 1) * size);
            }

            if (size != default)
            {
                query = query.Limit(size);
            }

            return ReRun(() => query.ToListAsync());
        }

        public Task<List<T>> FindzAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter, Func<SortDefinitionBuilder<T>, SortDefinition<T>> sort = default, int? skip = default, int? limit = default, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> project = default)
        {
            var query = Find(filter?.Invoke(Filter));
            query = Findz(query, sort, skip, limit, project);
            return ReRun(() => query.ToListAsync());
        }

        public Task<List<T>> FindzAsync(Expression<Func<T, bool>> filter, Func<SortDefinitionBuilder<T>, SortDefinition<T>> sort = null, int? skip = default, int? limit = default, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> project = default)
        {
            var query = Find(filter);
            query = Findz(query, sort, skip, limit, project);
            return ReRun(() => query.ToListAsync());
        }

        public Task ForEachAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter, Func<T, int, Task> processor, Func<SortDefinitionBuilder<T>, SortDefinition<T>> sort = default, int? skip = default, int? limit = default, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> project = default)
        {
            var query = Find(filter?.Invoke(Filter));
            query = Findz(query, sort, skip, limit, project);
            return ReRun(() => query.ForEachAsync(processor));
        }

        private IFindFluent<T, T> Findz(IFindFluent<T, T> query, Func<SortDefinitionBuilder<T>, SortDefinition<T>> sort, int? skip, int? limit, Func<ProjectionDefinitionBuilder<T>, ProjectionDefinition<T>> project)
        {
            if (sort != default)
            {
                query = query.Sort(sort(Sort));
            }
            query = query.Skip(skip).Limit(limit);
            if (project != default)
            {
                query = query.Project<T>(project(Project));
            }
            return query;
        }

        public Task<T> FindOneAndUpdateAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter, Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> update, Action<FindOneAndUpdateOptions<T, T>> options = default)
        {
            var option = new FindOneAndUpdateOptions<T, T>();
            if (options != default)
            {
                options(option);
            }
            return CreateMongoCollection().FindOneAndUpdateAsync(filter(Filter), update(Update), option);
        }
        #endregion

        #region 更新
        public Task<UpdateResult> UpsertOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            return UpsertOnezAsync(_ => filter, _ => update);
        }

        public Task<UpdateResult> UpsertOneAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
        {
            return UpsertOnezAsync(x => filter, _ => update);
        }

        public Task<UpdateResult> UpsertOnezAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> configureFilter, Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> configureUpdate)
        {
            return ReRun(() => CreateMongoCollection().UpdateOneAsync(configureFilter(Filter), configureUpdate(Update),
                new UpdateOptions { IsUpsert = true }));
        }

        public Task<UpdateResult> UpsertManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            return UpsertManyzAsync(_ => filter, _ => update);
        }

        public Task<UpdateResult> UpsertManyAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
        {
            return UpsertManyzAsync(_ => filter, _ => update);
        }

        public Task<UpdateResult> UpsertManyzAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> configureFilter, Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> configureUpdate)
        {
            return ReRun(() => CreateMongoCollection().UpdateManyAsync(configureFilter(Filter), configureUpdate(Update), new UpdateOptions { IsUpsert = true }));
        }

        public Task<UpdateResult> UpdateOneAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
        {
            return UpdateOnezAsync(_ => filter, _ => update);
        }

        public Task<UpdateResult> UpdateOneAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            return UpdateOnezAsync(_ => filter, _ => update);
        }

        public Task<UpdateResult> UpdateOnezAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> filter, Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> update)
        {
            return ReRun(() => CreateMongoCollection().UpdateOneAsync(filter(Filter), update(Update)));
        }

        public Task<UpdateResult> UpdateManyAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            return UpdateManyzAsync(_ => filter, _ => update);
        }

        public Task<UpdateResult> UpdateManyAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
        {
            return UpdateManyzAsync(_ => filter, _ => update);
        }

        public Task<UpdateResult> UpdateManyzAsync(Func<FilterDefinitionBuilder<T>, FilterDefinition<T>> configureFilter, Func<UpdateDefinitionBuilder<T>, UpdateDefinition<T>> configureUpdate)
        {
            return ReRun(() => CreateMongoCollection().UpdateManyAsync(configureFilter(Filter), configureUpdate(Update)));
        }

        #endregion

        #region 索引

        /// <summary>
        /// 获取所有索引
        /// </summary>
        /// <returns></returns>
        private async Task<IEnumerable<string>> IndexesListAsync()
        {
            var indexes = await CreateMongoCollection().Indexes.List().ToListAsync().ConfigureAwait(false);
            return indexes.Select(x => x["name"].AsString);
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        public Task<IEnumerable<string>> IndexesCreateManyAsync(IEnumerable<CreateIndexModel<T>> models)
        {
            if (models == null || models.Count() == 0) return null;
            return ReRun(() => CreateMongoCollection().Indexes.CreateManyAsync(models));
        }

        public Task<string> IndexesCreateOneAsync(string indexName, Func<IndexKeysDefinitionBuilder<T>, IndexKeysDefinition<T>> configare, CreateIndexOptions<T> options) => IndexesCreateOneAsync(indexName, configare, options as CreateIndexOptions);
        public Task<string> IndexesCreateOneAsync(string indexName, Func<IndexKeysDefinitionBuilder<T>, IndexKeysDefinition<T>> configare, bool Unique) => IndexesCreateOneAsync(indexName, configare, new CreateIndexOptions() { Unique = Unique });

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="indexName">索引名称</param>
        public async Task<string> IndexesCreateOneAsync(string indexName, Func<IndexKeysDefinitionBuilder<T>, IndexKeysDefinition<T>> configare, CreateIndexOptions options = default)
        {
            if (string.IsNullOrEmpty(indexName))
            {
                throw new ArgumentException($"“{nameof(indexName)}”不能是 Null 或为空。", nameof(indexName));
            }

            var indexes = await IndexesListAsync().ConfigureAwait(false);
            if (indexes.Contains(indexName)) return string.Empty;

            options ??= new CreateIndexOptions();
            options.Background = true;
            options.Name = indexName;
            CreateIndexModel<T> models = new CreateIndexModel<T>(configare(IndexKeys), options);

            var indexNameRet = await ReRun(() => CreateMongoCollection().Indexes.CreateOneAsync(models)).ConfigureAwait(false);
            return indexNameRet;
        }


        #endregion

        #region 公共方法
        /// <summary>
        /// 默认重试三次
        /// </summary>
        protected Task<T1> ReRun<T1>(Func<Task<T1>> callback)
        {
            return _policy.ExecuteAsync(() => callback());
        }

        /// <summary>
        /// 默认重试三次
        /// </summary>
        protected Task ReRun(Func<Task> callback)
        {
            return _policy.ExecuteAsync(() => callback());
        }

        /// <summary>
        /// 创建集合
        /// </summary>
        protected virtual IMongoCollection<T> CreateMongoCollection(MongoCollectionSettings mongoCollectionSettings = default) => _mongoDb.GetCollection<T>(CollectionName, mongoCollectionSettings ?? _mongoCollectionSettings);
        #endregion

        #region 抽象方法
        /// <summary>
        /// 创建索引
        /// </summary>
        /// <returns></returns>
        public abstract Task CreateIndexesAsync();

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <returns></returns>
        internal virtual async Task DropIndexesAsync()
        {
            var dropIndexNames = GetDropIndexNames();
            if (dropIndexNames?.Count() > 0)
            {
                var indexNames = await IndexesListAsync().ConfigureAwait(false);
                foreach (var name in dropIndexNames.Where(x => indexNames.Contains(x)))
                {
                    await ReRun(() => CreateMongoCollection().Indexes.DropOneAsync(name)).ConfigureAwait(false);
                }
            }
        }

        public virtual IEnumerable<string> GetDropIndexNames()
        {
            return null;
        }
        #endregion
    }
}
