using System.Threading.Channels;

namespace AsyncSnapshotPipelineDemo;

public sealed class SourceCoordinator(IReadOnlyCollection<IEventSource> sources)
{
    private readonly IReadOnlyCollection<IEventSource> _sources = sources;
    private readonly Dictionary<string, DateTimeOffset> _lastSeen = [];

    public async Task RunAsync(
        ChannelWriter<RawEvent> output,
        CancellationToken cancellationToken)
    {
        var candidates = Channel.CreateBounded<RawEvent>(new BoundedChannelOptions(256)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = false
        });

        var producers = _sources.Select(source => Task.Run(async () =>
        {
            await foreach (var item in source.ReadAsync(cancellationToken))
            {
                await candidates.Writer.WriteAsync(item, cancellationToken);
            }
        }, cancellationToken)).ToArray();
        var producerCompletion = Task.Run(async () =>
        {
            try
            {
                await Task.WhenAll(producers);
            }
            finally
            {
                candidates.Writer.TryComplete();
            }
        });

        try
        {
            while (await candidates.Reader.WaitToReadAsync(cancellationToken))
            {
                while (candidates.Reader.TryRead(out var candidate))
                {
                    _lastSeen[candidate.Source] = candidate.OccurredAt;
                    var activeSource = _sources
                        .Where(source => _lastSeen.TryGetValue(source.Name, out var seen)
                            && candidate.OccurredAt - seen <= TimeSpan.FromSeconds(2))
                        .OrderByDescending(source => source.Priority)
                        .FirstOrDefault();

                    // 只转发当前仍然新鲜且优先级最高的来源，模拟故障切换与择优。
                    if (activeSource?.Name == candidate.Source)
                    {
                        await output.WriteAsync(candidate, cancellationToken);
                    }
                }

                if (producers.All(task => task.IsCompleted) && candidates.Reader.Count == 0)
                {
                    break;
                }
            }
        }
        finally
        {
            candidates.Writer.TryComplete();
            await Task.WhenAll(producers);
            await producerCompletion;
            output.TryComplete();
        }
    }
}
