namespace ChannelDemo
{
    public class DemoRunner
    {
        public static async Task RunAllDemosAsync()
        {
            Console.WriteLine("System.Threading.Channels 高并发最佳实践演示");
            Console.WriteLine("=============================================");

            // 1. 基础高并发演示
            await RunBasicHighConcurrencyDemo();

            Console.WriteLine("\n\n");

            // 2. 高级模式演示
            await RunAdvancedPatternDemo();

            Console.WriteLine("\n\n");

            // 3. 性能对比演示
            await RunPerformanceComparison();

            Console.WriteLine("\n\n");

            // 4. Actor模式演示
            await RunActorPatternDemo();

            Console.WriteLine("\n所有演示完成！");
        }

        private static async Task RunBasicHighConcurrencyDemo()
        {
            Console.WriteLine("1. 基础高并发场景演示");
            Console.WriteLine("---------------------");

            var demo = new HighConcurrencyChannelDemo(
                producerCount: 20,  // 20个生产者
                consumerCount: 5    // 5个消费者
            );

            await demo.RunAsync(TimeSpan.FromSeconds(15)); // 运行15秒
        }

        private static async Task RunAdvancedPatternDemo()
        {
            Console.WriteLine("2. 高级Channel模式演示");
            Console.WriteLine("----------------------");

            var patterns = new AdvancedChannelPatterns();
            await patterns.DemonstratePatternsAsync();
        }

        private static async Task RunPerformanceComparison()
        {
            Console.WriteLine("3. Channel配置性能对比");
            Console.WriteLine("----------------------");

            var comparer = new ChannelConfigurationComparer();
            await comparer.CompareConfigurationsAsync();
        }

        private static async Task RunActorPatternDemo()
        {
            Console.WriteLine("4. Actor模式演示");
            Console.WriteLine("----------------");

            // 创建订单处理Actor
            var orderActor = new Actor<OrderMessage>("订单处理器", async message =>
            {
                Console.WriteLine($"处理订单: {message.OrderId} (优先级: {message.Priority})");
                await Task.Delay(new Random().Next(10, 100)); // 模拟订单处理
                Console.WriteLine($"订单 {message.OrderId} 处理完成");
            });

            // 创建库存更新Actor
            var inventoryActor = new Actor<InventoryMessage>("库存处理器", async message =>
            {
                Console.WriteLine($"更新库存: 商品 {message.ProductId} 数量 {message.Quantity}");
                await Task.Delay(new Random().Next(5, 50)); // 模拟库存更新
                Console.WriteLine($"库存 {message.ProductId} 更新完成");
            });

            // 创建通知发送Actor
            var notificationActor = new Actor<NotificationMessage>("通知处理器", async message =>
            {
                Console.WriteLine($"发送通知给用户 {message.UserId}: {message.Content}");
                await Task.Delay(new Random().Next(20, 200)); // 模拟通知发送
                Console.WriteLine($"通知已发送给用户 {message.UserId}");
            });

            // 启动生产者
            var producers = new List<Task>();

            // 订单生产者
            producers.Add(Task.Run(async () =>
            {
                for (int i = 0; i < 50; i++)
                {
                    var order = new OrderMessage
                    {
                        OrderId = Guid.NewGuid(),
                        Priority = i % 5 == 0 ? "High" : "Normal",
                        Amount = new Random().Next(10, 1000)
                    };

                    await orderActor.SendAsync(order);
                    await Task.Delay(new Random().Next(10, 100));
                }
            }));

            // 库存生产者
            producers.Add(Task.Run(async () =>
            {
                for (int i = 0; i < 30; i++)
                {
                    var inventory = new InventoryMessage
                    {
                        ProductId = $"PROD_{i:D3}",
                        Quantity = new Random().Next(-10, 100),
                        Reason = new Random().Next(0, 2) == 0 ? "销售" : "补货"
                    };

                    await inventoryActor.SendAsync(inventory);
                    await Task.Delay(new Random().Next(50, 200));
                }
            }));

            // 通知生产者
            producers.Add(Task.Run(async () =>
            {
                for (int i = 0; i < 40; i++)
                {
                    var notification = new NotificationMessage
                    {
                        UserId = $"USER_{i % 5}",
                        Content = $"您有新的消息 #{i + 1}",
                        Type = i % 3 == 0 ? "Email" : "SMS"
                    };

                    await notificationActor.SendAsync(notification);
                    await Task.Delay(new Random().Next(30, 150));
                }
            }));

            // 等待所有生产者完成
            await Task.WhenAll(producers);

            // 停止所有Actor
            await Task.WhenAll(
                orderActor.StopAsync(),
                inventoryActor.StopAsync(),
                notificationActor.StopAsync()
            );

            Console.WriteLine("Actor模式演示完成");
        }
    }

    // Actor模式的消息类型
    public class OrderMessage
    {
        public Guid OrderId { get; set; }
        public string Priority { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class InventoryMessage
    {
        public string ProductId { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class NotificationMessage
    {
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}