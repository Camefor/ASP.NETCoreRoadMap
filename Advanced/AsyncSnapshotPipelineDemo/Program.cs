using System.Threading.Channels;

namespace AsyncSnapshotPipelineDemo;

public static class Program
{
    public static async Task Main()
    {
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var source = new SourceCoordinator([
            new GeneratedEventSource("mq-primary", priority: 100, seed: 7),
            new GeneratedEventSource("api-fallback", priority: 50, seed: 11)
        ]);
        var input = Channel.CreateBounded<RawEvent>(new BoundedChannelOptions(256)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true,
            SingleWriter = true
        });
        var store = new AsyncBatchStore(partitionCount: 3);
        var processor = new SnapshotProcessor(partitionCount: 3, store);

        var storeTask = store.StartAsync(cancellation.Token);
        var processorTask = processor.RunAsync(input.Reader, cancellation.Token);
        await source.RunAsync(input.Writer, cancellation.Token);
        await processorTask;
        store.Complete();
        await storeTask;

        Console.WriteLine($"completed; distinct snapshots={store.Count}");
        foreach (var item in store.Snapshot().OrderBy(pair => pair.Key))
        {
            Console.WriteLine($"{item.Key} => value={item.Value.Value}, delta={item.Value.Delta}, state={item.Value.State}");
        }

        Console.ReadKey();
    }
}
