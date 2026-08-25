using System.Threading.Channels;

namespace AsyncSnapshotPipelineDemo;

public sealed class SnapshotProcessor(int partitionCount, AsyncBatchStore store)
{
    private readonly int _partitionCount = partitionCount;
    private readonly AsyncBatchStore _store = store;

    public async Task RunAsync(ChannelReader<RawEvent> input, CancellationToken cancellationToken)
    {
        var partitions = Enumerable.Range(0, _partitionCount)
            .Select(_ => Channel.CreateBounded<RawEvent>(new BoundedChannelOptions(128)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleReader = true,
                SingleWriter = false
            }))
            .ToArray();

        var consumers = partitions.Select((partition, index) => ConsumePartitionAsync(
            partition.Reader, index, cancellationToken)).ToArray();

        await foreach (var item in input.ReadAllAsync(cancellationToken))
        {
            var partition = (item.EntityId.GetHashCode() & int.MaxValue) % _partitionCount;
            await partitions[partition].Writer.WriteAsync(item, cancellationToken);
        }

        foreach (var partition in partitions)
        {
            partition.Writer.TryComplete();
        }

        await Task.WhenAll(consumers);
    }

    private async Task ConsumePartitionAsync(
        ChannelReader<RawEvent> input,
        int partition,
        CancellationToken cancellationToken)
    {
        var previousValues = new Dictionary<string, decimal>();
        await foreach (var item in input.ReadAllAsync(cancellationToken))
        {
            previousValues.TryGetValue(item.EntityId, out var previous);
            var delta = item.Value - previous;
            previousValues[item.EntityId] = item.Value;

            var state = delta switch
            {
                > 20 => "surge",
                < -20 => "drop",
                _ => "steady"
            };

            await _store.EnqueueAsync(new StorageEntry(
                $"snapshot:{item.EntityId}",
                new ProcessedSnapshot(item.EntityId, item.Value, delta, item.OccurredAt, item.Source, state),
                300), cancellationToken);

            if (item.Attributes.TryGetValue("format", out var format) && format == "demo")
            {
                Console.WriteLine($"processor partition={partition}, entity={item.EntityId}, state={state}");
            }
        }
    }
}
