using AbnormalDetermination.Redis;
using StackExchange.Redis;
using System;

namespace AbnormalDetermination
{

    /// <summary>
    /// 异常判定模型业务代码
    /// </summary>
    public class Test
    {
        public static void TestFunc()
        {
            Console.WriteLine("Hello World!");
        }

        /// <summary>
        /// 测试redis连接
        /// </summary>
        public static void TestConnectRedis()
        {
            string _connectionString = "127.0.0.1:6379,connectTimeout=1000,connectRetry=1,syncTimeout=10000,allowadmin=true";
            string _instanceName = "local";
            int _defaultDB = 10;

            string key = "testKey";
            RedisConnectHelper _redisConnect = new RedisConnectHelper(_connectionString, _instanceName, _defaultDB);
            IDatabase _redis = _redisConnect.GetDatabase();
            _redis.StringSet(key, "hello world value in redis");

            var context = _redis.StringGet(key);/*_redisClient.StringGet<string>(key);*/
            Console.WriteLine(context);
        }

    }
}
