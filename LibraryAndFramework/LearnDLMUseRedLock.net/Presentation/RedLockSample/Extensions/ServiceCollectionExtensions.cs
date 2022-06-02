using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RedLockNet;
using RedLockSample.Caching.Redis;
//using RedLockSample.Caching.Redis;
using RedLockSample.Contract;
using StackExchange.Redis;

namespace RedLockSample.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RedisConfiguration>(configuration.GetSection("Redis"));
        }

        /// <summary>
        /// 为 IserviceCollection 创建一个扩展，用于在 Redis 上配置我们的 RedLock。
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureDLM(this IServiceCollection services, IConfiguration configuration)
        {
            var redisOptions = configuration.GetSection(nameof(RedisConfiguration)).Get<RedisConfiguration>();
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisOptions.Connection);
            services.AddSingleton(s => redis.GetDatabase());

            // Initialize and register distributed lock factory
            var loggerFactory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Trace).AddConsole());

            RedLockProvider.SetRedLockFactory(redisOptions, loggerFactory);
            services.AddSingleton(typeof(IDistributedLockFactory), RedLockProvider.RedLockFactoryObject);
        }
    }
}
