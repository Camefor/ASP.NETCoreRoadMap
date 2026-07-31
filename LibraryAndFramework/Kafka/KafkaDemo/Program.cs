using Confluent.Kafka;

namespace KafkaDemo
{
    internal class Program
    {
        // 你的 Kafka 服务器地址和刚才建好的 Topic
        private const string BrokerList = "localhost:9092";
        private const string TopicName = "test";

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("🚀 Kafka Demo 已启动！");

            var cts = new CancellationTokenSource();

            // 在后台线程启动消费者
            var consumerTask = Task.Run(() => StartConsumer(cts.Token));

            //  在主线程启动生产者 (接收用户输入)
            await StartProducerAsync();

            // 用户输入 exit 退出后，取消消费者线程
            cts.Cancel();
            await consumerTask;

        }

        /// <summary>
        /// 生产者代码：负责发送消息
        /// </summary>
        /// <returns></returns>
        static async Task StartProducerAsync()
        {
            var config = new ProducerConfig
            {
                BootstrapServers = BrokerList,
                Acks = Acks.Leader
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();

            Console.WriteLine("👉 请输入要发送的消息 (输入 'exit' 退出):");

            while (true)
            {
                var text = Console.ReadLine();
                if (text?.ToLower() == "exit")
                {
                    break;
                }

                try
                {
                    //发送消息到指定的 Topic
                    var deliveryResult = await producer.ProduceAsync(TopicName, new Message<Null, string> { Value = text ?? string.Empty });
                    Console.WriteLine($"[生产者] ✅ 消息已发送至分区: {deliveryResult.Partition}, 偏移量: {deliveryResult.Offset}");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"[生产者] ❌ 发送失败: {e.Error.Reason}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[生产者] ❌ 发送失败: {e}");
                }
            }
        }

        static void StartConsumer(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = BrokerList,
                GroupId = "my-first-consumer-group", // 消费者组 ID
                AutoOffsetReset = AutoOffsetReset.Earliest, // 如果没有偏移量记录，从最早的消息开始读
                EnableAutoCommit = true,
            };

            using var consumer = new ConsumerBuilder<Null, string>(config).Build();

            // 订阅 Topic

            consumer.Subscribe(TopicName);

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = consumer.Consume(TimeSpan.FromSeconds(1));

                    if (consumeResult != null)
                    {
                        Console.WriteLine($"\n    >>> [消费者] 📥 收到新消息: {consumeResult.Message.Value}");
                        Console.Write("👉 继续输入: ");
                    }

                }
            }
            catch (OperationCanceledException)
            {
                //正常退出 不处理异常
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                // 离开组并提交最终偏移量
                consumer.Close();
            }
        }
    }
}
