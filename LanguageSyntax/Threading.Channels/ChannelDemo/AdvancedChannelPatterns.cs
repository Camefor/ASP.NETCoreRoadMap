using System.Threading.Channels;
using System.Diagnostics;

namespace ChannelDemo
{
    public class AdvancedChannelPatterns
    {
        public async Task DemonstratePatternsAsync()
        {
            Console.WriteLine("\n=== 高级Channel模式演示 ===\n");

            // 1. 批处理模式
            await DemonstrateBatchProcessingAsync();

            Console.WriteLine("\n" + new string('-', 50) + "\n");

            // 2. 多级管道模式
            await DemonstratePipelinePatternAsync();

            Console.WriteLine("\n" + new string('-', 50) + "\n");

            // 3. 超时和重试模式
            await DemonstrateTimeoutAndRetryAsync();
        }

        private async Task DemonstrateBatchProcessingAsync()
        {
            Console.WriteLine("模式1: 批处理消费者");
            Console.WriteLine("将单个项目收集成批次进行处理，提高吞吐量\n");

            var channel = Channel.CreateUnbounded<BatchData>();

            // 启动批量消费者
            var batchConsumer = Task.Run(() => BatchConsumerAsync(channel.Reader));

            // 启动生产者
            var producers = new List<Task>();
            for (int i = 0; i < 5; i++)
            {
                int producerId = i;
                producers.Add(Task.Run(() => BatchProducerAsync(channel.Writer, producerId)));
            }

            // 运行10秒
            await Task.Delay(10000);

            // 停止
            channel.Writer.Complete();
            await Task.WhenAll(producers);
            await batchConsumer;

            Console.WriteLine("批处理演示完成\n");
        }

        private async Task BatchConsumerAsync(ChannelReader<BatchData> reader)
        {
            var batch = new List<BatchData>();
            var batchSize = 100;
            var batchTimeoutMs = 1000; // 1秒超时

            await foreach (var item in reader.ReadAllAsync())
            {
                batch.Add(item);

                // 当批次达到指定大小或超时时处理批次
                if (batch.Count >= batchSize)
                {
                    await ProcessBatchAsync(batch);
                    batch.Clear();
                }
            }

            // 处理剩余项目
            if (batch.Count > 0)
            {
                await ProcessBatchAsync(batch);
            }

            Console.WriteLine($"批量消费者完成，批次处理模式运行结束");
        }

        private async Task BatchProducerAsync(ChannelWriter<BatchData> writer, int producerId)
        {
            var random = new Random(producerId);
            var count = 0;

            try
            {
                while (count < 1000)
                {
                    var data = new BatchData
                    {
                        Id = Guid.NewGuid(),
                        ProducerId = producerId,
                        Value = random.Next(1, 1000),
                        CreatedAt = DateTime.UtcNow
                    };

                    await writer.WriteAsync(data);
                    count++;

                    await Task.Delay(random.Next(1, 50));
                }
            }
            finally
            {
                Console.WriteLine($"批处理生产者 {producerId} 完成，生产了 {count} 条数据");
            }
        }

        private async Task ProcessBatchAsync(List<BatchData> batch)
        {
            Console.WriteLine($"处理批次: {batch.Count} 条数据");

            // 模拟批量操作（如批量写入数据库）
            await Task.Delay(50);

            // 模拟批量处理结果
            var totalValue = batch.Sum(x => x.Value);
            var avgValue = (double)totalValue / batch.Count;

            Console.WriteLine($" - 批次处理完成：平均值 {avgValue:F2}");
        }

        private async Task DemonstratePipelinePatternAsync()
        {
            Console.WriteLine("模式2: 多级管道处理");
            Console.WriteLine("数据通过多个处理阶段，每个阶段有自己的消费者\n");

            // 创建三个通道形成管道
            var stage1Channel = Channel.CreateBounded<PipelineData>(1000);
            var stage2Channel = Channel.CreateBounded<PipelineData>(1000);
            var stage3Channel = Channel.CreateBounded<PipelineData>(1000);

            // 启动管道处理器
            var stage1Processor = Task.Run(() => Stage1ProcessorAsync(stage1Channel.Reader, stage2Channel.Writer));
            var stage2Processor = Task.Run(() => Stage2ProcessorAsync(stage2Channel.Reader, stage3Channel.Writer));
            var stage3Processor = Task.Run(() => Stage3ProcessorAsync(stage3Channel.Reader));

            // 启动生产者
            var producer = Task.Run(() => PipelineProducerAsync(stage1Channel.Writer));

            // 运行8秒
            await Task.Delay(8000);

            // 停止
            stage1Channel.Writer.Complete();
            await producer;
            await stage1Processor;

            stage2Channel.Writer.Complete();
            await stage2Processor;

            stage3Channel.Writer.Complete();
            await stage3Processor;

            Console.WriteLine("管道处理演示完成\n");
        }

        private async Task PipelineProducerAsync(ChannelWriter<PipelineData> writer)
        {
            var random = new Random();
            var count = 0;

            try
            {
                while (count < 500)
                {
                    var data = new PipelineData
                    {
                        Id = Guid.NewGuid(),
                        Stage = 0,
                        Data = $"原始数据_{count}",
                        Timestamp = DateTime.UtcNow
                    };

                    await writer.WriteAsync(data);
                    count++;

                    await Task.Delay(random.Next(10, 50));
                }
            }
            finally
            {
                Console.WriteLine($"管道生产者完成，生产了 {count} 条数据");
            }
        }

        private async Task Stage1ProcessorAsync(ChannelReader<PipelineData> reader, ChannelWriter<PipelineData> writer)
        {
            var processed = 0;
            await foreach (var data in reader.ReadAllAsync())
            {
                await Task.Delay(10); // 模拟处理时间

                data.Stage = 1;
                data.Data = $"[阶段1处理] {data.Data}";
                processed++;

                await writer.WriteAsync(data);
            }

            writer.Complete();
            Console.WriteLine($"阶段1处理器完成，处理了 {processed} 条数据");
        }

        private async Task Stage2ProcessorAsync(ChannelReader<PipelineData> reader, ChannelWriter<PipelineData> writer)
        {
            var processed = 0;
            await foreach (var data in reader.ReadAllAsync())
            {
                await Task.Delay(15);

                data.Stage = 2;
                data.Data = $"[阶段2处理] {data.Data}";
                processed++;

                await writer.WriteAsync(data);
            }

            writer.Complete();
            Console.WriteLine($"阶段2处理器完成，处理了 {processed} 条数据");
        }

        private async Task Stage3ProcessorAsync(ChannelReader<PipelineData> reader)
        {
            var processed = 0;
            await foreach (var data in reader.ReadAllAsync())
            {
                await Task.Delay(5);

                data.Stage = 3;
                data.Data = $"[阶段3处理] {data.Data}";
                processed++;

                // 最终处理完成
                if (processed % 50 == 0)
                {
                    Console.WriteLine($"最终完成: {processed} 条数据");
                }
            }

            Console.WriteLine($"阶段3处理器完成，处理了 {processed} 条数据");
        }

        private async Task DemonstrateTimeoutAndRetryAsync()
        {
            Console.WriteLine("模式3: 超时处理和重试机制");
            Console.WriteLine("处理失败的数据会进入重试队列，超过重试次数则进入死信队列\n");

            var mainChannel = Channel.CreateBounded<RetryData>(1000);
            var retryChannel = Channel.CreateBounded<RetryData>(100);
            var deadLetterChannel = Channel.CreateUnbounded<RetryData>();

            // 启动消费者
            var mainConsumer = Task.Run(() => RetryConsumerAsync(mainChannel.Reader, retryChannel.Writer, deadLetterChannel.Writer));
            var retryConsumer = Task.Run(() => RetryProcessorAsync(retryChannel.Reader, deadLetterChannel.Writer));
            var deadLetterConsumer = Task.Run(() => DeadLetterProcessorAsync(deadLetterChannel.Reader));

            // 启动生产者（包含一些会失败的数据）
            var producers = new List<Task>();
            for (int i = 0; i < 3; i++)
            {
                int producerId = i;
                producers.Add(Task.Run(() => RetryProducerAsync(mainChannel.Writer, producerId)));
            }

            // 运行10秒
            await Task.Delay(10000);

            // 停止
            mainChannel.Writer.Complete();
            await Task.WhenAll(producers);
            await mainConsumer;

            retryChannel.Writer.Complete();
            await retryConsumer;

            deadLetterChannel.Writer.Complete();
            await deadLetterConsumer;

            Console.WriteLine("超时重试演示完成\n");
        }

        private async Task RetryProducerAsync(ChannelWriter<RetryData> writer, int producerId)
        {
            var random = new Random(producerId);
            var count = 0;

            try
            {
                while (count < 200)
                {
                    var data = new RetryData
                    {
                        Id = Guid.NewGuid(),
                        ProducerId = producerId,
                        RetryCount = 0,
                        MaxRetries = 3,
                        SuccessRate = random.NextDouble() < 0.7 ? 0.3 : 0.95, // 30%的数据有70%失败率
                        CreatedAt = DateTime.UtcNow
                    };

                    await writer.WriteAsync(data);
                    count++;

                    await Task.Delay(random.Next(10, 100));
                }
            }
            finally
            {
                Console.WriteLine($"重试生产者 {producerId} 完成，生产了 {count} 条数据");
            }
        }

        private async Task RetryConsumerAsync(ChannelReader<RetryData> reader, ChannelWriter<RetryData> retryWriter, ChannelWriter<RetryData> deadLetterWriter)
        {
            var processed = 0;
            var failed = 0;

            await foreach (var data in reader.ReadAllAsync())
            {
                try
                {
                    // 模拟处理超时
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                    await ProcessWithTimeoutAsync(data, cts.Token);
                    processed++;
                }
                catch (TimeoutException)
                {
                    Console.WriteLine($"处理超时: {data.Id:N}");
                    failed++;

                    // 加入重试队列
                    if (data.RetryCount < data.MaxRetries)
                    {
                        data.RetryCount++;
                        data.LastRetryAt = DateTime.UtcNow;
                        await retryWriter.WriteAsync(data);
                    }
                    else
                    {
                        await deadLetterWriter.WriteAsync(data);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"处理失败: {data.Id:N} - {ex.Message}");
                    failed++;
                }
            }

            Console.WriteLine($"重试消费者完成: 成功 {processed}, 失败 {failed}");
        }

        private async Task RetryProcessorAsync(ChannelReader<RetryData> reader, ChannelWriter<RetryData> deadLetterWriter)
        {
            var retried = 0;
            var dead = 0;

            await foreach (var data in reader.ReadAllAsync())
            {
                var delay = TimeSpan.FromSeconds(Math.Pow(2, data.RetryCount)); // 指数退避
                await Task.Delay(delay);

                try
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    await ProcessWithTimeoutAsync(data, cts.Token);
                    retried++;
                    Console.WriteLine($"重试成功: {data.Id:N} (第 {data.RetryCount} 次重试)");
                }
                catch
                {
                    if (data.RetryCount < data.MaxRetries)
                    {
                        data.RetryCount++;
                        // await retryChannel.Writer.WriteAsync(data); // 这里需要外部通道引用
                    }
                    else
                    {
                        dead++;
                        await deadLetterWriter.WriteAsync(data);
                    }
                }
            }

            Console.WriteLine($"重试处理器完成: 重试成功 {retried}, 死信 {dead}");
        }

        private async Task DeadLetterProcessorAsync(ChannelReader<RetryData> reader)
        {
            var deadCount = 0;
            await foreach (var data in reader.ReadAllAsync())
            {
                deadCount++;
                Console.WriteLine($"死信: {data.Id:N} (重试 {data.RetryCount} 次)");
            }
            Console.WriteLine($"死信处理器完成: {deadCount} 条");
        }

        private async Task ProcessWithTimeoutAsync(RetryData data, CancellationToken token)
        {
            // 模拟处理时间
            await Task.Delay(new Random().Next(100, 1000), token);

            // 模拟失败
            if (new Random().NextDouble() > data.SuccessRate)
            {
                throw new InvalidOperationException("模拟处理失败");
            }
        }
    }

    public class BatchData
    {
        public Guid Id { get; set; }
        public int ProducerId { get; set; }
        public int Value { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PipelineData
    {
        public Guid Id { get; set; }
        public int Stage { get; set; }
        public string Data { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class RetryData
    {
        public Guid Id { get; set; }
        public int ProducerId { get; set; }
        public int RetryCount { get; set; }
        public int MaxRetries { get; set; }
        public double SuccessRate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastRetryAt { get; set; }
    }
}