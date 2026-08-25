using System.Collections.Concurrent;
using System.Threading.Channels;

namespace AsyncSnapshotPipelineDemo;

public sealed class AsyncBatchStore(int partitionCount, int maxBatchSize = 8)
{
    private readonly Channel<StorageEntry>[] _partitions = Enumerable.Range(0, partitionCount)
        .Select(_ => Channel.CreateBounded<StorageEntry>(new BoundedChannelOptions(256)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = false
        }))
        .ToArray();

    private readonly ConcurrentDictionary<string, ProcessedSnapshot> _values = new();

    public int Count => _values.Count;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.WhenAll(Enumerable.Range(0, _partitions.Length)
            .Select(index => ConsumePartitionAsync(index, cancellationToken)));
    }

    public ValueTask EnqueueAsync(StorageEntry entry, CancellationToken cancellationToken)
    {
        var partition = (entry.Key.GetHashCode() & int.MaxValue) % _partitions.Length;
        return _partitions[partition].Writer.WriteAsync(entry, cancellationToken);
    }

    public void Complete()
    {
        foreach (var partition in _partitions)
        {
            partition.Writer.TryComplete();
        }
    }

    public IReadOnlyDictionary<string, ProcessedSnapshot> Snapshot() => _values;

    private async Task ConsumePartitionAsync(int partition, CancellationToken cancellationToken)
    {
        var reader = _partitions[partition].Reader;
        var batch = new Dictionary<string, StorageEntry>();
        var lastFlush = DateTimeOffset.UtcNow;

        while (await reader.WaitToReadAsync(cancellationToken))
        {
            while (reader.TryRead(out var entry))
            {
                // 同一批次相同 Key 只保留最后值，减少无意义的存储写入。
                batch[entry.Key] = entry;
                if (batch.Count >= maxBatchSize)
                {
                    await FlushAsync(batch, cancellationToken);
                    lastFlush = DateTimeOffset.UtcNow;
                }
            }

            if (batch.Count > 0 && DateTimeOffset.UtcNow - lastFlush >= TimeSpan.FromMilliseconds(150))
            {
                await FlushAsync(batch, cancellationToken);
                lastFlush = DateTimeOffset.UtcNow;
            }
        }

        if (batch.Count > 0)
        {
            await FlushAsync(batch, cancellationToken);
        }

        Console.WriteLine($"store partition={partition} stopped");
    }

    private Task FlushAsync(Dictionary<string, StorageEntry> batch, CancellationToken cancellationToken)
    {
        foreach (var entry in batch.Values)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _values[entry.Key] = entry.Value;
        }

        Console.WriteLine($"store batch flushed size={batch.Count}");
        batch.Clear();
        return Task.CompletedTask;
    }
}
