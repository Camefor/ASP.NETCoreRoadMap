using RedLockSample.Common;
using RedLockSample.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLockSample.Service
{
    public class ContributionService : IContributionService
    {
        private readonly ICacheService cacheService;

        public ContributionService(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public async Task AddContributionWithDLM(int value)
        {
            var result = await cacheService.DoActionWithLockAsync<int>(
                LockKeyProvider.ContributionLockKey,
                value,
                async (arg) => await AddContributionToCache(value));

            if (!result.IsSuccessfullyProcessed)
            {
                var exception = result.Exception;
                //persist or somehow process the failed item
            }
        }

        public async Task AddContributionWithoutDLM(int value)
        {
            await AddContributionToCache(value);
        }

        private async Task AddContributionToCache(int value)
        {
            var cacheKey = CacheKeyProvider.GetAddContributionKey;

            var currentValue = await cacheService.GetAsync<int>(cacheKey);

            var newValue = currentValue + value;
            await cacheService.SetAsync(cacheKey, newValue);

        }
    }

}
