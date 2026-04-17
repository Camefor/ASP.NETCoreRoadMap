using System.Threading.Channels;

namespace ChannelDemo
{
    public class HighConcurrencyChannelDemo
    {
        private readonly Channel<TradeData> _channel;
        private readonly int _producerCount;
        private readonly int _consumerCount;
        private readonly CancellationTokenSource _cts;

        public HighConcurrencyChannelDemo(int producerCount = 10, int consumerCount = 3)
        {
            _producerCount = producerCount;
            _consumerCount = consumerCount;
            _cts = new CancellationTokenSource();

            // 创建有界通道，防止内存溢出
            var options = new BoundedChannelOptions(10000)
            {
                // 当通道满时，等待其他项目消费
                FullMode = BoundedChannelFullMode.Wait,
                // 允许多个生产者同时写入
                SingleWriter = false,
                // 允许多个消费者同时读取
                SingleReader = false,
                // 启用continuations以获得更好的性能
                AllowSynchronousContinuations = false
            };

            _channel = Channel.CreateBounded<TradeData>(options);
        }

        public async Task RunAsync(TimeSpan duration)
        {
            Console.WriteLine($"\n=== 高并发Channel演示开始 ===");
            Console.WriteLine($"生产者数量: {_producerCount}, 消费者数量: {_consumerCount}");
            Console.WriteLine($"通道容量: 10000, 运行时长: {duration.TotalMinutes} 分钟\n");

            // 启动统计任务
            var statsTask = Task.Run(async () => await PrintStatisticsAsync(_cts.Token));

            // 启动消费者任务（先启动消费者，确保数据能被及时处理）
            var consumerTasks = new List<Task>();
            for (int i = 0; i < _consumerCount; i++)
            {
                int consumerId = i;
                consumerTasks.Add(Task.Run(() => ConsumerAsync(consumerId, _cts.Token)));
            }

            // 等待一小段时间确保消费者已准备就绪
            await Task.Delay(1000);

            // 启动生产者任务
            var producerTasks = new List<Task>();
            for (int i = 0; i < _producerCount; i++)
            {
                int producerId = i;
                producerTasks.Add(Task.Run(() => ProducerAsync(producerId, _cts.Token)));
            }

            // 运行指定时间后停止
            await Task.Delay(duration);

            // 停止生产者
            Console.WriteLine("\n正在停止生产者...");
            _cts.Cancel();

            // 等待所有生产者完成
            await Task.WhenAll(producerTasks);
            Console.WriteLine("所有生产者已停止");

            // 标记通道完成（不再有新数据）
            _channel.Writer.Complete();

            // 等待所有消费者完成
            await Task.WhenAll(consumerTasks);
            Console.WriteLine("所有消费者已完成处理");

            // 停止统计
            statsTask.Wait(TimeSpan.FromSeconds(5));

            Console.WriteLine("\n=== 演示完成 ===\n");
        }

        private async Task ProducerAsync(int producerId, CancellationToken token)
        {
            var random = new Random(producerId + Environment.TickCount);
            var processedCount = 0;

            try
            {
                while (!token.IsCancellationRequested)
                {
                    // 模拟交易数据生成
                    var tradeData = GenerateTradeData(random, producerId);

                    // 异步写入通道
                    await _channel.Writer.WriteAsync(tradeData, token);

                    processedCount++;

                    // 模拟数据生成间隔（高频率）
                    await Task.Delay(random.Next(1, 10), token);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"生产者 {producerId} 已停止，处理了 {processedCount} 条数据");
            }
        }

        private async Task ConsumerAsync(int consumerId, CancellationToken token)
        {
            var processedCount = 0;
            var startTime = DateTime.UtcNow;

            try
            {
                await foreach (var tradeData in _channel.Reader.ReadAllAsync(token))
                {
                    // 模拟数据处理（业务逻辑）
                    await ProcessTradeDataAsync(tradeData, consumerId);

                    processedCount++;

                    // 每1000条记录输出一次进度
                    if (processedCount % 1000 == 0)
                    {
                        var elapsed = DateTime.UtcNow - startTime;
                        var rate = processedCount / elapsed.TotalSeconds;
                        Console.WriteLine($"消费者 {consumerId}: 已处理 {processedCount} 条, 速率: {rate:F2} 条/秒");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // 正常取消，不需要处理
            }
            finally
            {
                var elapsed = DateTime.UtcNow - startTime;
                var avgRate = processedCount / elapsed.TotalSeconds;
                Console.WriteLine($"消费者 {consumerId} 完成: 总处理 {processedCount} 条, 平均速率: {avgRate:F2} 条/秒");
            }
        }

        private async Task ProcessTradeDataAsync(TradeData tradeData, int consumerId)
        {
            // 模拟复杂的业务处理逻辑
            await Task.Delay(1); // 模拟IO操作

            // 这里可以添加实际的业务处理，例如：
            // - 数据验证和清洗
            // - 写入数据库
            // - 调用外部API
            // - 计算聚合指标等

            // 模拟处理失败的情况（1%的概率）
            if (new Random().Next(0, 100) == 0)
            {
                Console.WriteLine($"消费者 {consumerId}: 处理失败 - TradeId: {tradeData.TradeId}");
            }
        }

        private TradeData GenerateTradeData(Random random, int producerId)
        {
            return new TradeData
            {
                TradeId = Guid.NewGuid(),
                Symbol = GetRandomSymbol(random),
                Price = Math.Round((decimal)random.NextDouble() * 1000M + 100M, 2),
                Quantity = random.Next(1, 10000),
                Timestamp = DateTime.UtcNow,
                ProducerId = producerId,
                Side = random.Next(0, 2) == 0 ? 'B' : 'S' // Buy 或 Sell
            };
        }

        private string GetRandomSymbol(Random random)
        {
            var symbols = new[] { "AAPL", "GOOGL", "MSFT", "AMZN", "META", "TSLA", "NVDA", "JPM", "V", "JNJ" };
            return symbols[random.Next(symbols.Length)];
        }

        private async Task PrintStatisticsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(5000, token); // 每5秒输出一次统计

                Console.WriteLine($"\n--- 通道统计 ---");
                Console.WriteLine($"当前通道项目数: {_channel.Reader.Count}");
                Console.WriteLine($"通道容量使用率: {(double)_channel.Reader.Count / 10000 * 100:F2}%");
                Console.WriteLine($"通道是否完成: {_channel.Reader.Completion.IsCompleted}");
                Console.WriteLine($"----------------\n");
            }
        }
    }

    public class TradeData
    {
        public Guid TradeId { get; set; }
        public string Symbol { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime Timestamp { get; set; }
        public int ProducerId { get; set; }
        public char Side { get; set; } // 'B' for Buy, 'S' for Sell

        public override string ToString()
        {
            return $"Trade:{TradeId:N} {Symbol} {Side} {Quantity}@{Price}";
        }
    }
}