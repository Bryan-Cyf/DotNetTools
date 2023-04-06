using Microsoft.AspNetCore.Mvc;
using Nest;
using Tools.Elastic;

namespace Bryan.Demo.WebApi.Controllers
{
    public class ElasticController : BaseController
    {
        //方式1
        private readonly IElasticRepository<UserWallet> _userWalletRespository;

        //方式2—等效
        private readonly UserWalletElasticRepository _userCustomWalletRespository;

        public ElasticController(IElasticRepository<UserWallet> userWalletRespository
            , UserWalletElasticRepository userCustomWalletRespository)
        {
            _userWalletRespository = userWalletRespository;
            _userCustomWalletRespository = userCustomWalletRespository;
        }

        #region Index 数据库操作

        /// <summary>
        /// 删除索引—删除库
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DeleteIndex()
        {
            await _userWalletRespository.DeleteIndexAsync();
            return Ok();
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Insert(UserWallet model)
        {
            var user = new UserWallet
            {
                UserId = "A112312312311",
                UserName = $"U{DateTime.Now.Second.ToString()}",
                CreateTime = DateTime.Now,
                Money = 110m
            };
            await _userWalletRespository.InsertAsync(model);
            return Ok();
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> InsertRange()
        {
            var users = new List<UserWallet>();

            var rd = new Random();
            for (var i = 0; i < 100; i++)
            {
                users.Add(new UserWallet
                {
                    UserId = rd.Next().ToString(),
                    UserName = $"U{DateTime.Now.Second.ToString()}",
                    CreateTime = DateTime.Now,
                    Money = rd.Next(10, 100)
                });
            }

            await _userWalletRespository.InsertManyAsync(users);
            return Ok();
        }

        #endregion

        #region Delete 删除数据

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Delete(string name)
        {
            await _userWalletRespository.DeleteByQueryAsync(x => x.UserName.Contains(name) || x.UserName == "U26");

            return Ok();
        }

        #endregion

        #region Update 更新

        /// <summary>
        /// 根据Id修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> UpdateByKey(string id)
        {
            var record = new UserWallet
            {
                UserId = "1458487865768454",
                UserName = "Update"
            };
            await _userWalletRespository.UpdateAsync(id, record);
            return Ok();
        }

        /// <summary>
        /// 根据Id修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> UpdateAsync(string id)
        {
            var userWallet = await _userWalletRespository.Queryable().Where(x => x.Id == id).FirstAsync();

            if (userWallet == null) return Ok("Success");
            userWallet.UserName = "Update";
            await _userWalletRespository.UpdateAsync(id, userWallet);
            return Ok();
        }

        #endregion

        #region Search 查找

        [HttpGet]
        public IActionResult SearchAll()
        {
            var data = _userWalletRespository.Queryable().OrderBy(x => x.Money).ToList();
            return Ok(data);
        }

        [HttpGet]
        public IActionResult SearchPage(string name)
        {
            var data = _userWalletRespository.Queryable().Where(x => x.UserName.Contains(name)).ToPageList(1, 2);
            return Ok(data);
        }

        [HttpGet]
        public IActionResult SearchPageNumber(string name)
        {
            var data = _userWalletRespository.Queryable().Where(x => x.UserName.Contains(name)).ToPageList(1, 2, out var total);
            return Ok(new
            {
                total,
                data
            });
        }

        [HttpGet]
        public async Task<IActionResult> SearchAsync(string name)
        {
            var data = await _userWalletRespository.Queryable().Where(x => x.UserName.Contains(name)).ToListAsync();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GroupAsync()
        {
            var data = await _userWalletRespository.Queryable().GroupBy(x => x.UserName).ToListAsync();
            return Ok(data);
        }

        #endregion

        #region Alias 别名

        /// <summary>
        /// 添加别名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AddAlias()
        {
            _userWalletRespository.AddAliasAsync("alias");
            return Ok();
        }

        /// <summary>
        /// 删除别名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult RemoveAlias()
        {
            _userWalletRespository.RemoveAlias("alias");
            return Ok();
        }

        #endregion

        #region NEST 原生

        [HttpGet]
        public async Task<IActionResult> NestSearchAsync(string name, string userId, DateTime createTime)
        {
            var data = await _userCustomWalletRespository.GetAllAsync();

            data = await _userCustomWalletRespository.GetByNameWithTerm(name);

            data = await _userCustomWalletRespository.GetByNameWithMatch(name);

            data = await _userCustomWalletRespository.GetByNameWithMatchPhrase(name);

            data = await _userCustomWalletRespository.GetByNameWithMatchPhrasePrefix(name);

            data = await _userCustomWalletRespository.GetByNameWithWildcard(name);

            data = await _userCustomWalletRespository.GetByNameWithFuzzy(name);

            data = await _userCustomWalletRespository.SearchInAllFiels(name);

            data = await _userCustomWalletRespository.GetByNameAndDescriptionMultiMatch(name);

            data = await _userCustomWalletRespository.GetActorsCondition(name, userId, createTime);

            data = await _userCustomWalletRespository.GetActorsAllCondition(name);

            var data2 = await _userCustomWalletRespository.GetActorsAggregation();

            return Ok();
        }

        #endregion

    }

    [ElasticTable("userwallet")]
    public class UserWallet : ElasticEntity
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public DateTime CreateTime { get; set; }

        public decimal Money { get; set; }
    }

    public class UserWalletElasticRepository : ElasticBaseRepository<UserWallet>
    {
        public UserWalletElasticRepository(IElasticClient elasticClient) : base(elasticClient)
        {
        }

        //lowcase
        public async Task<ICollection<UserWallet>> GetByNameWithTerm(string name)
        {
            var query = new QueryContainerDescriptor<UserWallet>()
                .Term(p => p.Field(p => p.UserName)
                .Value(name)
                .Boost(6.0));
            var result = await SearchAsync(_ => query);

            return result?.ToList();
        }

        //using operator OR in case insensitive
        public async Task<List<UserWallet>> GetByNameWithMatch(string name)
        {
            var query = new QueryContainerDescriptor<UserWallet>()
                .Match(p => p.Field(f => f.UserName)
                .Query(name)
                .Operator(Operator.And));
            var result = await SearchAsync(_ => query, x => x.Ascending(a => a.Money));

            return result?.ToList();
        }

        public async Task<List<UserWallet>> GetByNameWithMatchPhrase(string name)
        {
            var query = new QueryContainerDescriptor<UserWallet>()
                .MatchPhrase(p => p.Field(f => f.UserName)
                .Query(name));
            var result = await SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<List<UserWallet>> GetByNameWithMatchPhrasePrefix(string name)
        {
            var query = new QueryContainerDescriptor<UserWallet>()
                .MatchPhrasePrefix(p => p.Field(f => f.UserName)
                .Query(name));
            var result = await SearchAsync(_ => query);

            return result?.ToList();
        }

        //contains
        public async Task<List<UserWallet>> GetByNameWithWildcard(string name)
        {
            var query = new QueryContainerDescriptor<UserWallet>()
                .Wildcard(w => w.Field(f => f.UserName)
                .Value($"*{name}*"));
            var result = await SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<UserWallet>> GetByNameWithFuzzy(string name)
        {
            var query = new QueryContainerDescriptor<UserWallet>()
                .Fuzzy(descriptor => descriptor.Field(p => p.UserName)
                .Value(name));
            var result = await SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<List<UserWallet>> SearchInAllFiels(string term)
        {
            var query = NestExtensions.BuildMultiMatchQuery<UserWallet>(term);
            var result = await SearchAsync(_ => query);

            return result.ToList();
        }

        public async Task<List<UserWallet>> GetByNameAndDescriptionMultiMatch(string term)
        {
            var query = new QueryContainerDescriptor<UserWallet>()
                .MultiMatch(p => p
                    .Fields(p => p.Field(f => f.UserName).Field(d => d.UserId))
                    .Query(term)
                    .Operator(Operator.And));

            var result = await SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<List<UserWallet>> GetActorsCondition(string name, string userId, DateTime? createTime)
        {
            QueryContainer query = new QueryContainerDescriptor<UserWallet>();

            if (!string.IsNullOrEmpty(name))
            {
                query = query && new QueryContainerDescriptor<UserWallet>()
                    .MatchPhrasePrefix(qs => qs
                        .Field(fs => fs.UserName)
                        .Query(name));
            }
            if (!string.IsNullOrEmpty(userId))
            {
                query = query && new QueryContainerDescriptor<UserWallet>()
                    .MatchPhrasePrefix(qs => qs
                        .Field(fs => fs.UserId)
                        .Query(userId));
            }
            if (createTime.HasValue)
            {
                query = query && new QueryContainerDescriptor<UserWallet>()
                    .Bool(b => b
                        .Filter(f => f
                            .DateRange(dt => dt
                                .Field(field => field.CreateTime)
                                .GreaterThanOrEquals(createTime)
                                .LessThanOrEquals(createTime)
                                .TimeZone("+00:00"))));
            }

            var result = await SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<List<UserWallet>> GetActorsAllCondition(string term)
        {
            var query = new QueryContainerDescriptor<UserWallet>().Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.UserName))));
            int.TryParse(term, out var numero);

            query = query && new QueryContainerDescriptor<UserWallet>().Wildcard(w => w.Field(f => f.UserName).Value($"*{term}*")) //bad performance, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<UserWallet>().Wildcard(w => w.Field(f => f.UserId).Value($"*{term}*")) //bad performance, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<UserWallet>().Term(w => w.Money, numero);

            var result = await SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<object> GetActorsAggregation()
        {
            var query = new QueryContainerDescriptor<UserWallet>().Bool(b => b.Must(m => m.Exists(e => e.Field(f => f.UserName))));

            var result = await SearchAsync(_ => query, a => a
                        .Sum("TotalMoney", sa => sa.Field(o => o.Money))
                        .Average("AvMoney", sa => sa.Field(p => p.Money)));

            var totalMoney = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "TotalMoney");
            var avMoney = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "AvMoney");

            return new { TotalMoney = totalMoney, AvMoney = avMoney };
        }
    }
}
