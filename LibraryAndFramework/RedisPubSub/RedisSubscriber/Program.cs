// See https://aka.ms/new-console-template for more information
using StackExchange.Redis;
namespace RedisSubscriber
{
    class Program
    {
        private const string RedisConnectionString = "localhost:6379";
        private static ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(RedisConnectionString);
        private const string Channel = "test-channel";
        static void Main(string[] args)
        {
            Console.WriteLine("Listening test-channel");
            var pubsub = connection.GetSubscriber();

            pubsub.Subscribe(Channel, (channel, message) =>
            {
                Console.WriteLine("Message received from test-channel : " + message);
            });

            /**
             * https://zhuanlan.zhihu.com/p/411160154
             * Pub/Sub 最大问题是：丢数据。
             * 如果发生以下场景，就有可能导致数据丢失：
             * 消费者下线
             * Redis 宕机
             * 消息堆积
             * **/
            Console.ReadLine();
        }
    }

}