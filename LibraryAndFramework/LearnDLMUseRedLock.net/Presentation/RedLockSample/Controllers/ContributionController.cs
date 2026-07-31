using Microsoft.AspNetCore.Mvc;
using RedLockSample.Common;
using RedLockSample.Contract;

namespace RedLockSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContributionController : ControllerBase
    {
        private readonly IContributionService _contributionService;
        private readonly ICacheService _cacheService;

        public ContributionController(IContributionService contributionService, ICacheService cacheService)
        {
            _contributionService = contributionService;
            _cacheService = cacheService;

            _cacheService.SetAsync(LockKeyProvider.ContributionLockKey, 0).ConfigureAwait(false).GetAwaiter();
        }

        /// <summary>
        /// 使用分布式锁控制并发 
        /// 效果 每次50个并发请求
        /// </summary>
        /// <returns></returns>
        [HttpGet("With_DLM")]
        public async Task<int> AddContributionWithDLM()
        {
            List<Task> addContributionTasks = new List<Task>();
            for (int i = 1; i <= 50; i++)
            {
                addContributionTasks.Add(_contributionService.AddContributionWithDLM(1));
            }
            await Task.WhenAll(addContributionTasks);
            return await _cacheService.GetAsync<int>(CacheKeyProvider.GetAddContributionKey);
        }

        /// <summary>
        /// 不使用分布式锁 效果  50个并发请求 每次读取的值不固定，
        /// </summary>
        /// <returns></returns>
        [HttpGet("Without_DLM")]
        public async Task<int> AddContributionsWithoutDLM()
        {
            List<Task> addContributionTasks = new List<Task>();
            for (int i = 1; i <= 50; i++)
            {
                addContributionTasks.Add(_contributionService.AddContributionWithoutDLM(1));
            }
            await Task.WhenAll(addContributionTasks);
            return await _cacheService.GetAsync<int>(CacheKeyProvider.GetAddContributionKey);
        }
    }
}
