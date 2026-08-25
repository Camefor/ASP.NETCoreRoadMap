# AsyncSnapshotPipelineDemo


- 多来源接入与“当前最佳来源”选择，提炼为 `SourceCoordinator`。
- 消息反序列化、分区、状态计算和模型组装，提炼为 `SnapshotProcessor`。
- 有界队列、按 Key 分区、攒批和异步写入，提炼为 `AsyncBatchStore`。
- 存储层仅用 `ConcurrentDictionary` 模拟，便于替换为真实 KV 存储而不改变处理链路。

## 文件职责

- `Program.cs`：只负责组装依赖和启动流水线。
- `Models.cs`：定义跨阶段传递的事件、快照和存储条目模型。
- `GeneratedEventSource.cs`：提供可替换的虚构消息源实现。
- `SourceCoordinator.cs`：汇聚来源并执行新鲜度/优先级择优。
- `SnapshotProcessor.cs`：按分区并行加工事件。
- `AsyncBatchStore.cs`：按 Key 分区、攒批并写入内存存储。

## 数据流

```text
GeneratedEventSource（可替换为 Kafka、MQ、HTTP、Redis 队列）
        ↓
SourceCoordinator（有界 Channel + 来源新鲜度/优先级择优）
        ↓
SnapshotProcessor（按实体分区，并行加工）
        ↓
AsyncBatchStore（按 Key 分区、批量合并、异步写入）
        ↓
ConcurrentDictionary（本地内存，模拟 Redis 适配器）
```

## 运行

```powershell
dotnet run --project .\Advanced\AsyncSnapshotPipelineDemo\AsyncSnapshotPipelineDemo.csproj
```

## 可替换点

- 将 `GeneratedEventSource` 实现替换为 MQ/Kafka Consumer、HTTP Polling 或 Redis 队列读取器。
- 将 `AsyncBatchStore.FlushAsync` 替换为 Redis Pipeline、批量写入 API 或其他 KV 存储客户端。
- 生产环境应根据消息可靠性需求调整丢弃策略、重试、死信和优雅停机流程。
