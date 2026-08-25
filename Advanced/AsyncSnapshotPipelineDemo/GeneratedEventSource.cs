namespace AsyncSnapshotPipelineDemo;

public sealed class GeneratedEventSource(string name, int priority, int seed) : IEventSource
{
    public string Name { get; } = name;

    public int Priority { get; } = priority;

    public async IAsyncEnumerable<RawEvent> ReadAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var random = new Random(seed);
        for (var index = 0; index < 18; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Delay(TimeSpan.FromMilliseconds(25 + random.Next(30)), cancellationToken);

            yield return new RawEvent(
                Name,
                $"item-{index % 5:00}",
                DateTimeOffset.UtcNow,
                100 + random.Next(0, 80),
                new Dictionary<string, string> { ["format"] = "demo" });
        }
    }
}
