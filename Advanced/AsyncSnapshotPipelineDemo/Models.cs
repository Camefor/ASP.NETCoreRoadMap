namespace AsyncSnapshotPipelineDemo;

public sealed record RawEvent(
    string Source,
    string EntityId,
    DateTimeOffset OccurredAt,
    decimal Value,
    IReadOnlyDictionary<string, string> Attributes);

public sealed record ProcessedSnapshot(
    string EntityId,
    decimal Value,
    decimal Delta,
    DateTimeOffset ObservedAt,
    string Source,
    string State);

public sealed record StorageEntry(string Key, ProcessedSnapshot Value, int ExpireSeconds);

public interface IEventSource
{
    string Name { get; }

    int Priority { get; }

    IAsyncEnumerable<RawEvent> ReadAsync(CancellationToken cancellationToken);
}
