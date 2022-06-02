using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RedLockNet;
using RedLockSample.Contract;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLockSample.Caching.Redis
{

    /// <summary>
    /// 实现缓存服务   StackExchange.Redis 库的抽象 实现
    /// </summary>
    public class CacheService : ICacheService
    {
        private readonly IDatabase redisCache;
        private readonly IDistributedLockFactory distributedLockFactory;
        private readonly RedisConfiguration options;

        public CacheService(
           IDatabase database,
           IDistributedLockFactory distributedLockFactory,
           IOptions<RedisConfiguration> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(RedisConfiguration));

            redisCache = database;
            this.distributedLockFactory = distributedLockFactory;
            this.options = options.Value;
        }

        /// <summary>
        /// DoActionWithLockAsync 方法，可用于控制处理器功能的并发性。
        /// </summary>
        /// <param name="lockKey"></param>
        /// <param name="processor"></param>
        /// <returns></returns>
        public async Task<LockProcessResult> DoActionWithLockAsync(string lockKey, Func<Task> processor)
        {
            var processResult = new LockProcessResult();
            try
            {
                await using var redLock = await distributedLockFactory.CreateLockAsync
                    (lockKey, TimeSpan.FromSeconds(options.ExpiryTimeFromSeconds),
                    TimeSpan.FromSeconds(options.WaitTimeFromSeconds),
                    TimeSpan.FromMilliseconds(options.RetryTimeFromMilliseconds)
                    );
                if (redLock.IsAcquired) await processor();
                else processResult.SetException(new Exception("The lock wasn't acquired"));
            }
            catch (Exception ex)
            {
                processResult.SetException(ex);
            }
            
            return processResult;
        }

        public Task<LockProcessResult<TInput>> DoActionWithLockAsync<TInput>(string lockKey, TInput parameter, Func<TInput, Task> processor)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> GetAsync<TEntity>(string key)
        {
            var entity = await redisCache.StringGetAsync(key);
            if (entity.HasValue)
            {
                return Deserialize<TEntity>(entity);
            }
            else
            {
                return default;
            }
        }

        public async Task SetAsync<TEntity>(string key, TEntity value)
        {

            var stringEntity = Serialize(value);
            await redisCache.StringSetAsync(key, stringEntity);
        }



        public static string Serialize<T>(T obj) //where T : class
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(string obj) //where T : class
        {
            return JsonConvert.DeserializeObject<T>(obj);
        }

    }
}
