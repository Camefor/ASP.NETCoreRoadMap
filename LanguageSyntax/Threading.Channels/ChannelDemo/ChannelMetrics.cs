using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;

namespace ChannelDemo
{
    public class ChannelMetrics
    {
        private readonly object _lock = new object();
        private long _totalProduced;
        private long _totalConsumed;
        private long _currentQueueSize;
        private long _maxQueueSize;
        private readonly ConcurrentQueue<TimeSpan> _processingTimes = new();
        private readonly ConcurrentQueue<TimeSpan> _queueTimes = new();
        private readonly Stopwatch _stopwatch = new();
        private DateTime _lastReportTime = DateTime.UtcNow;

        public void StartTiming()
        {
            _stopwatch.Start();
        }

        public void RecordProduced()
        {
            Interlocked.Increment(ref _totalProduced);
        }

        public void Consumed()
        {
            Interlocked.Increment(ref _totalConsumed);

            if (_processingTimes.Count > 10000)
            {
                _processingTimes.TryDequeue(out _);
            }
        }

        public void RecordQueueSize(int size)
        {
            Interlocked.Exchange(ref _currentQueueSize, size);

            lock (_lock)
            {
                if (size > _maxQueueSize)
                {
                    _maxQueueSize = size;
                }
            }
        }
    }

    public class PerformanceMonitor
    {
        private readonly Channel<string> _metricsChannel;
        private readonly Timer _reportTimer;
        // private readonly Dictionary<string, PerformanceCounter> _counters;

        private readonly ConcurrentQueue<(DateTime timestamp, long size)> _queueSizeHistory = new();
        private readonly ConcurrentQueue<(DateTime timestamp, double throughput)> _throughputHistory = new();
        private readonly ConcurrentQueue<(DateTime timestamp, double latency)> _latencyHistory = new();

        public PerformanceMonitor()
        {
            _metricsChannel = Channel.CreateUnbounded<string>();
            // _counters = new Dictionary<string, PerformanceCounter>();

            // 每秒生成一次报告
            _reportTimer = new Timer(GenerateReport, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));

            // 启动指标消费者
            Task.Run(ProcessMetricsAsync);
        }

        public async Task ReportThroughputAsync(string channelName, double itemsPerSecond)
        {
            await _metricsChannel.Writer.WriteAsync($"throughput:{channelName}:{itemsPerSecond:F2}");
        }

        public async Task ReportLatencyAsync(string channelName, TimeSpan latency)
        {
            await _metricsChannel.Writer.WriteAsync($"latency:{channelName}:{latency.TotalMilliseconds:F2}");
        }

        public async Task ReportQueueSizeAsync(string channelName, int size)
        {
            await _metricsChannel.Writer.WriteAsync($"queue:{channelName}:{size}");
            _queueSizeHistory.Enqueue((DateTime.UtcNow, size));

            // 只保留最近5分钟的数据
            while (_queueSizeHistory.TryPeek(out var oldest) && DateTime.UtcNow - oldest.timestamp > TimeSpan.FromMinutes(5))
            {
                _queueSizeHistory.TryDequeue(out _);
            }
        }

        private async Task ProcessMetricsAsync()
        {
            await foreach (var metric in _metricsChannel.Reader.ReadAllAsync())
            {
                var parts = metric.Split(':');
                if (parts.Length < 3) continue;

                var type = parts[0];
                var channel = parts[1];
                var value = parts[2];

                switch (type)
                {
                    case "throughput":
                        if (double.TryParse(value, out var throughput))
                        {
                            _throughputHistory.Enqueue((DateTime.UtcNow, throughput));
                        }
                        break;

                    case "latency":
                        if (double.TryParse(value, out var latency))
                        {
                            _latencyHistory.Enqueue((DateTime.UtcNow, latency));
                        }
                        break;
                }
            }
        }

        private void GenerateReport(object? state)
        {
            var now = DateTime.UtcNow;

            // 计算最近1秒的平均吞吐量
            var recentThroughput = _throughputHistory
                .Where(x => now - x.timestamp <= TimeSpan.FromSeconds(1))
                .DefaultIfEmpty()
                .Average(x => x.throughput);

            // 计算最近1秒的平均延迟
            var recentLatency = _latencyHistory
                .Where(x => now - x.timestamp <= TimeSpan.FromSeconds(1))
                .DefaultIfEmpty()
                .Average(x => x.latency);

            // 获取当前队列大小
            var currentQueueSize = _queueSizeHistory.TryPeek(out var latest) ? latest.size : 0;

            Console.WriteLine($"[性能监控] 吞吐量: {recentThroughput:F2} 条/秒, " +
                            $"延迟: {recentLatency:F2} ms, " +
                            $"队列大小: {currentQueueSize}");
        }

        public void Stop()
        {
            _reportTimer?.Dispose();
            _metricsChannel.Writer.Complete();
        }

        public async Task ExportMetricsAsync(string filePath)
        {
            var metrics = new
            {
                QueueSizeHistory = _queueSizeHistory.ToArray(),
                ThroughputHistory = _throughputHistory.ToArray(),
                LatencyHistory = _latencyHistory.ToArray(),
                GeneratedAt = DateTime.UtcNow
            };

            var json = System.Text.Json.JsonSerializer.Serialize(metrics, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);
            Console.WriteLine($"导出性能指标报告到: {filePath}");
        }
    }

    // 比较不同Channel配置的性能测试工具
    public class ChannelConfigurationComparer
    {
        public async Task CompareConfigurationsAsync()
        {
            Console.WriteLine("\n=== Channel配置性能对比 ===");
            Console.WriteLine("测试不同的通道配置在高负载下的性能表现\n");

            var scenarios = new[]
            {
                new { Name = "有界通道 - Wait模式", Config = CreateBoundedWaitChannel() },
                new { Name = "有界通道 - DropNewest模式", Config = CreateBoundedDropNewestChannel() },
                new { Name = "有界通道 - DropOldest模式", Config = CreateBoundedDropOldestChannel() },
                // new { Name = "无界通道", Config = CreateUnboundedChannel() }
            };

            const int testDataCount = 100000;
            const int producerCount = 10;
            const int consumerCount = 5;

            foreach (var scenario in scenarios)
            {
                await RunPerformanceTestAsync(scenario.Name, scenario.Config, testDataCount, producerCount, consumerCount);

                // 清理间隔
                await Task.Delay(2000);
            }

            Console.WriteLine("\n性能对比测试完成\n");
        }

        private BoundedChannelOptions CreateBoundedWaitChannel()
        {
            return new BoundedChannelOptions(10000)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleWriter = false,
                SingleReader = false,
                AllowSynchronousContinuations = false
            };
        }

        private BoundedChannelOptions CreateBoundedDropNewestChannel()
        {
            return new BoundedChannelOptions(10000)
            {
                FullMode = BoundedChannelFullMode.DropNewest,
                SingleWriter = false,
                SingleReader = false,
                AllowSynchronousContinuations = false
            };
        }

        private BoundedChannelOptions CreateBoundedDropOldestChannel()
        {
            return new BoundedChannelOptions(10000)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleWriter = false,
                SingleReader = false,
                AllowSynchronousContinuations = false
            };
        }    
        private UnboundedChannelOptions CreateUnboundedChannel()
        {
            var unboundedChannelOption = new UnboundedChannelOptions();
            return unboundedChannelOption;
        }

        private async Task RunPerformanceTestAsync(string name, BoundedChannelOptions? options,
            int testDataCount, int producerCount, int consumerCount)
        {
            Console.WriteLine($"\n测试场景: {name}");

            var channel = options != null
                ? Channel.CreateBounded<PerfTestData>(options)
                : Channel.CreateUnbounded<PerfTestData>();

            var sw = Stopwatch.StartNew();
            var produced = 0L;
            var consumed = 0L;
            var dropped = 0L;

            // 启动消费者
            var consumerTasks = new List<Task>();
            for (int i = 0; i < consumerCount; i++)
            {
                int consumerId = i;
                consumerTasks.Add(Task.Run(async () =>
                {
                    await foreach (var item in channel.Reader.ReadAllAsync())
                    {
                        await Task.Delay(1); // 模拟处理
                        Interlocked.Increment(ref consumed);
                    }
                }));
            }

            // 等待消费者准备就绪
            await Task.Delay(100);

            // 启动生产者
            var producerTasks = new List<Task>();
            for (int i = 0; i < producerCount; i++)
            {
                int producerId = i;
                var itemsPerProducer = testDataCount / producerCount;

                producerTasks.Add(Task.Run(async () =>
                {
                    for (int j = 0; j < itemsPerProducer; j++)
                    {
                        var data = new PerfTestData
                        {
                            Id = Guid.NewGuid(),
                            ProducerId = producerId,
                            Index = j
                        };

                        if (channel.Writer.TryWrite(data))
                        {
                            Interlocked.Increment(ref produced);
                        }
                        else
                        {
                            Interlocked.Increment(ref dropped);
                        }

                        // 模拟数据产生间隔
                        await Task.Delay(1);
                    }
                }));
            }

            // 等待所有生产者完成
            await Task.WhenAll(producerTasks);
            channel.Writer.Complete();

            // 等待所有消费者完成
            await Task.WhenAll(consumerTasks);

            sw.Stop();

            // 输出性能统计
            Console.WriteLine($"  测试时长: {sw.Elapsed.TotalSeconds:F2} 秒");
            Console.WriteLine($"  生产数量: {produced:N}");
            Console.WriteLine($"  消费数量: {consumed:N}");
            Console.WriteLine($"  丢弃数量: {dropped:N}");
            Console.WriteLine($"  平均吞吐量: {produced / sw.Elapsed.TotalSeconds:F2} 条/秒");

            if (produced > 0)
            {
                Console.WriteLine($"  丢失率: {(double)dropped / (produced + dropped) * 100:F2}%");
            }
        }
    }

    public class PerfTestData
    {
        public Guid Id { get; set; }
        public int ProducerId { get; set; }
        public int Index { get; set; }
    }

    // 使用Channel实现简单的Actor模式
    public class Actor<TMessage>
    {
        private readonly Channel<TMessage> _channel;
        private readonly Func<TMessage, Task> _handler;
        private readonly Task _processingTask;

        public Actor(string name, Func<TMessage, Task> handler, int capacity = 1000)
        {
            Name = name;
            _handler = handler;

            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = true,
                SingleWriter = false
            };

            _channel = Channel.CreateBounded<TMessage>(options);
            _processingTask = ProcessMessagesAsync();
        }

        public string Name { get; }

        public async Task SendAsync(TMessage message)
        {
            await _channel.Writer.WriteAsync(message);
        }

        public bool TrySend(TMessage message)
        {
            return _channel.Writer.TryWrite(message);
        }

        private async Task ProcessMessagesAsync()
        {
            await foreach (var message in _channel.Reader.ReadAllAsync())
            {
                try
                {
                    await _handler(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Actor {Name} 处理消息失败: {ex.Message}");
                }
            }
        }

        public async Task StopAsync()
        {
            _channel.Writer.Complete();
            await _processingTask;
        }
    }
}