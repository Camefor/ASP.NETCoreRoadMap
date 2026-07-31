using System.Threading.Channels;
using ChannelDemo;

//参照：https://www.cnblogs.com/ljbguanli/p/19343088
// Console.WriteLine("Hello, Channel!");

//Channel<T> 是 System.Threading.Channels 命名空间中的一个类，提供了一个线程安全的异步通道，用于在多个任务/线程之间传递数据。它实现了生产者-消费者模式，但采用了完全不同的设计哲学。

//创建通道方法：
// var channel = Channel.CreateBounded<int>(10); //有界通道
//有界通道配置选项
var boundedChannelOptions = new BoundedChannelOptions(10)
{
    FullMode = BoundedChannelFullMode.Wait,
    SingleWriter = false, // 多写
    SingleReader = true // 单读
};
var channel = Channel.CreateBounded<int>(boundedChannelOptions);

// Channel.CreateUnbounded<int>(); //无界通道 (无限容量)

// Channel<T> 的一个关键设计是读写分离：
ChannelWriter<int> writer = channel.Writer;
ChannelReader<int> reader = channel.Reader;
/*
 * 这种分离设计带来了以下优势：
 * 可以将写入端暴露给生产者，读取端暴露给消费者
 * 更清晰的职责划分
 * 灵活的通道控制能力
 */


// 操作方法
//异步写入
await writer.WriteAsync(666);
//非阻塞写入尝试
Console.WriteLine(writer.TryWrite(2333) ? "写入成功" : "通道已满，处理背压");

//写入并标记完成
await writer.WriteAsync(123);

//标记不再有新数据
writer.Complete();
// writer.TryComplete();

/*
*namespace System.Threading.Channels
   /// <summary>Specifies the behavior to use when writing to a bounded channel that is already full.</summary>
   public enum BoundedChannelFullMode
       /// <summary>Wait for space to be available in order to complete the write operation.</summary>
       Wait,
       /// <summary>Remove and ignore the newest item in the channel in order to make room for the item being written.</summary>
       DropNewest,
       /// <summary>Remove and ignore the oldest item in the channel in order to make room for the item being written.</summary>
       DropOldest,
       /// <summary>Drop the item being written.</summary>
       DropWrite
*/

// 运行高并发Channel演示
Console.WriteLine("\n是否运行高并发Channel演示？(y/n): ");
var input = Console.ReadLine();

if (input?.ToLower() == "y")
{
    await DemoRunner.RunAllDemosAsync();
}

//关于 IAsyncEnumerable<T>：
//https://markheath.net/post/async-enumerable-1
// 在.NET中，IAsyncEnumerable<T>是一个在C# 8.0和.NET Core 3.0中引入的接口，
// 它允许异步迭代数据序列，特别是在需要延迟检索或从外部数据源（如数据库或API）获取数据时，而不会阻塞线程。
// 通过await foreach循环使用IAsyncEnumerable<T>，可以实现对异步数据流的非阻塞迭代，从而提高内存效率和程序响应性，
// 尤其是在处理大型数据集或需要立即处理可用数据块时非常有用。

Console.WriteLine("\n程序结束，按任意键退出...");
Console.ReadKey();