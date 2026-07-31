using Microsoft.Extensions.Logging;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using RedLockSample.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RedLockSample.Caching.Redis
{

    /// <summary>
    /// 负责创建 RedLockFactory 的静态实例。
    /// </summary>
    public static class RedLockProvider
    {

        /// <summary>
        /// RedLockFactory 的静态实例。
        /// </summary>
        public static RedLockFactory RedLockFactoryObject;

        public static void SetRedLockFactory(RedisConfiguration redisOptions, ILoggerFactory loggerFactory)
        {
            if (string.IsNullOrEmpty(redisOptions.Connection))
            {
                throw new ArgumentException("Invalid RedisUrl for Creating RedLock");
            }
            var endpoints = new List<RedLockEndPoint>()
            {
                new RedLockEndPoint()
                {
                    EndPoint = new DnsEndPoint(redisOptions.BaseUrl,redisOptions.Port),
                    Password = redisOptions.Password
                }
                //在这里，您可以添加所有 Redis 端点，但是，它也适用于一个端点。 （Redis 官方文档建议的最佳实践是至少有三个端点）。
            };
            RedLockFactoryObject = RedLockFactory.Create(endpoints, redisOptions.LogLockingProcess ? loggerFactory : null);
            //如果您想在记录器中详细查看与 DLM 的所有交互，还可以为 Create 方法设置 LoggerFactory。 （最好将 ConsoleLogger 发送到此提供程序，以防您在获取锁定时遇到问题。您可以使用此记录器监控您的 DLM）。
        }
    }
}
