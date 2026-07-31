using System.Runtime.CompilerServices;

// https://www.cnblogs.com/TangQF/articles/19450148

/**


# chatgpt response

如果让我对 **C# 多线程中最容易被误解的关键字** 排个名，那么 **`volatile` 一定排第一**。

很多人认为：

> **volatile = 线程安全**

这是**错误的**。

还有很多人认为：

> **volatile = 每次都从主内存读取**

这也是**不准确的**。

微软官方文档甚至已经明确建议：

> **绝大多数情况下，不要使用 volatile，而应该优先使用 `lock`、`Interlocked` 或其他同步原语。** ([Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/volatile?utm_source=chatgpt.com))

---

# 一、volatile 到底解决什么问题？

一句话：

> **volatile 保证"可见性（Visibility）"和部分"有序性（Ordering）"，但不保证原子性（Atomicity）。** ([Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/volatile?utm_source=chatgpt.com))

三大并发问题：

```
多个线程
      │
      ▼
 ┌─────────────┐
 │ 可见性      │ ← volatile解决
 ├─────────────┤
 │ 指令重排序  │ ← volatile部分解决
 ├─────────────┤
 │ 原子性      │ ← volatile解决不了
 └─────────────┘
```

---

# 二、为什么会有 volatile？

先看一个最经典的问题。

## 线程A

```csharp
while (!_stop)
{
    DoWork();
}
```

线程B

```csharp
_stop = true;
```

按理说：

```
线程A一直循环

线程B修改

_stop=true

线程A退出
```

现实中：

```
线程A

while(true)
{
}
```

竟然可能永远不会退出。

为什么？

---

# 三、CPU不是一直在读变量吗？

不是。

现代CPU会做大量优化。

例如：

```
bool stop = false;

while (!stop)
{
}
```

JIT可能认为：

```
stop没有人在改
```

于是直接优化成：

```
bool temp = stop;

while(!temp)
{
}
```

注意：

```
temp
```

一直存在寄存器。

以后根本不会重新读取内存。

于是：

```
线程B：

stop=true
```

线程A：

```
一直读寄存器

false

false

false

false
```

永远不会退出。

微软官方文档中的示例就是这种情况。([Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/volatile?utm_source=chatgpt.com))

---

# 四、volatile 做了什么？

如果：

```csharp
private volatile bool _stop;
```

那么：

JIT不能这样优化：

```
bool temp=_stop;

while(!temp)
```

必须变成：

```
while(!_stop)
{
}
```

也就是说：

**每次循环必须重新读取这个字段。**

注意：

这里很多文章说：

> "每次从主内存读取"

严格来说是不准确的。

官方说法是：

> volatile 不保证立即看到其它CPU写入的最新值，也不等于强制访问主内存，而是禁止某些优化，并建立特定的内存访问顺序。([Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/volatile?utm_source=chatgpt.com))

---

# 五、volatile解决的是"可见性"

举例：

```
CPU1

count=10
```

CPU2

```
count=5
```

没有volatile：

```
CPU2

一直看自己的缓存

不知道CPU1更新了
```

volatile以后：

CPU必须重新进行一次真正的内存读取（而不是一直使用寄存器中的旧值），并配合.NET内存模型提供的可见性语义，而不是无限期复用旧值。([Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.threading.volatile?view=net-9.0&utm_source=chatgpt.com))

---

# 六、volatile不能保证原子性

例如：

```csharp
volatile int count;
```

很多人以为：

```
count++;
```

安全。

实际上：

```
count++
```

等于：

```
读取

+

1

写回
```

三个步骤。

线程A：

```
count=5
```

线程B：

```
count=5
```

线程A：

```
写6
```

线程B：

```
写6
```

最后：

```
count==6
```

实际上应该：

```
7
```

所以：

```
volatile
```

不能解决。

应该：

```csharp
Interlocked.Increment(ref count);
```

或者：

```csharp
lock(...)
{
    count++;
}
```

这是微软官方推荐的方式。([Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/volatile?utm_source=chatgpt.com))

---

# 七、volatile还能禁止指令重排序

现代CPU：

代码：

```
A
B
C
```

实际可能执行：

```
B
A
C
```

只要：

结果一致。

例如：

```csharp
data = 100;

ready = true;
```

CPU可能：

```
ready=true

data=100
```

线程2：

```csharp
if(ready)
{
    Console.WriteLine(data);
}
```

可能打印：

```
0
```

因为：

```
ready

已经可见

但是

data

还没写完
```

如果：

```csharp
volatile bool ready;
```

那么：

```
data

必须先完成

ready

才能写
```

这样：

```
ready=true

意味着

data一定已经准备好
```

这就是：

Memory Barrier（内存屏障）的作用之一。([Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/api/system.threading.volatile?view=net-9.0&utm_source=chatgpt.com))

---

# 八、真实生产环境有哪些使用场景？

## 场景1：后台线程退出（最经典）

例如：

```
BackgroundService

Worker

Consumer
```

```csharp
private volatile bool _stop;
```

后台：

```csharp
while (!_stop)
{
    Consume();
}
```

停止：

```csharp
_stop = true;
```

几乎所有：

```
消费者线程

轮询线程

Socket线程

```

都见过这种写法。

不过在现代.NET中，更推荐：

```csharp
CancellationToken
```

而不是：

```
volatile bool
```

---

## 场景2：配置热更新

例如：

```
配置中心

Apollo

Nacos

Consul
```

配置：

```
EnableLog
```

后台：

```
监听配置变化
```

收到：

```
EnableLog=true
```

更新：

```csharp
private volatile bool _enableLog;
```

业务线程：

```csharp
if (_enableLog)
{
    Log();
}
```

无需lock。

因为：

只是：

```
读

写

bool
```

---

## 场景3：缓存对象替换（生产中很常见）

例如：

```csharp
volatile Dictionary<int, Stock> _cache;
```

后台：

```
重新加载缓存
```

```csharp
_cache = newCache;
```

业务：

```csharp
var item = _cache[id];
```

注意：

这里：

**不是修改Dictionary。**

而是：

```
整个引用

替换
```

volatile保证：

所有线程看到的是最新引用。

---

## 场景4：单例初始化（双重检查锁）

以前经典：

```csharp
private static volatile Singleton _instance;
```

```csharp
if(_instance==null)
{
    lock(lockObj)
    {
        if(_instance==null)
        {
            _instance=new Singleton();
        }
    }
}
```

为什么？

因为：

```
new Singleton()
```

不是一步。

可能：

```
① 分配内存

② 引用赋值

③ 构造函数
```

如果：

```
②

③

发生重排序
```

另一个线程：

```
_instance!=null

但是对象还没初始化完成
```

volatile就是为了防止这种问题。

不过：

.NET现在推荐：

```
Lazy<T>

static初始化

LazyInitializer
```

已经很少自己写这种代码。

---

# 九、volatile不能解决哪些问题？

例如：

```csharp
volatile List<int> list;
```

线程A：

```csharp
list.Add(1);
```

线程B：

```csharp
list.Add(2);
```

仍然：

线程不安全。

volatile只是：

```
List引用

可见
```

不是：

```
List内部

线程安全
```

---

# 十、什么时候应该用 volatile？

可以参考下面这张决策表：

| 场景 | volatile | 更推荐方案 |
|------|----------|------------|
| bool 停止标志 | ✅ 可以 | `CancellationToken` |
| 单纯状态标志 | ✅ 可以 | `Volatile.Read/Write` 也可 |
| 计数器 | ❌ | `Interlocked` |
| List、Dictionary 修改 | ❌ | `lock` 或并发集合 |
| 多字段一致性 | ❌ | `lock` |
| 单例 | 很少需要 | `Lazy<T>` |
| 高并发业务 | 很少 | `lock`、`ReaderWriterLockSlim`、并发集合 |

---

# 十一、现代 .NET 开发中的建议

在现在（.NET 6/7/8/9）的生产环境中，`volatile` 的使用频率已经远低于十几年前。

一个典型的优先级可以记为：

```
CancellationToken
        ↓
ConcurrentDictionary
        ↓
Channel<T>
        ↓
lock
        ↓
ReaderWriterLockSlim
        ↓
Interlocked
        ↓
Volatile.Read/Write
        ↓
volatile 关键字
```

真正需要直接使用 `volatile` 关键字的场景已经比较少，通常只出现在**高性能框架、底层并发组件、无锁（lock-free）算法或非常简单的状态标志**中。
对于绝大多数业务系统，`lock`、`Interlocked`、`CancellationToken` 等更高级的同步机制更安全、更容易维护，也更符合微软当前的官方建议。
([Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/volatile?utm_source=chatgpt.com))


**/

namespace KeywordsInCSharp
{
    public static class VolatileDemo
    {
        private const int WriterDelayMilliseconds = 200;
        private static readonly TimeSpan WorkerTimeout = TimeSpan.FromSeconds(1);

        private static bool _plainStopRequested;
        private static volatile bool _volatileStopRequested;

        public static void Run()
        {
            Console.WriteLine("=== volatile 关键字：线程停止标记实验 ===");
            Console.WriteLine("线程 A 紧密循环，线程 B 在 200 ms 后把停止标记改为 true。");
            Console.WriteLine("建议使用 dotnet run -c Release，并且不要附加调试器。\n");

            RunVolatileExperiment();
            Console.WriteLine();

            bool plainThreadStopped = RunPlainExperiment();
            if (plainThreadStopped)
            {
                Console.WriteLine();
                RunCachedReadSimulation();
            }
        }

        private static void RunVolatileExperiment()
        {
            _volatileStopRequested = false;
            using var workerStarted = new ManualResetEventSlim(false);

            var threadA = new Thread(() => VolatileThreadA(workerStarted))
            {
                IsBackground = true,
                Name = "volatile-thread-a"
            };
            var threadB = new Thread(() => VolatileThreadB(workerStarted))
            {
                Name = "volatile-thread-b"
            };

            Console.WriteLine("[1] volatile 字段");
            threadA.Start();
            threadB.Start();

            threadB.Join();
            bool stopped = threadA.Join(WorkerTimeout);

            Console.WriteLine("线程 B：已写入 true。");
            Console.WriteLine(stopped
                ? "线程 A：观察到修改并退出。"
                : "线程 A：超时；请检查机器是否负载过高。");
        }

        private static bool RunPlainExperiment()
        {
            _plainStopRequested = false;
            using var workerStarted = new ManualResetEventSlim(false);

            var threadA = new Thread(() => PlainThreadA(workerStarted))
            {
                // 普通字段版本可能永不退出，后台线程可防止整个演示程序被挂住。
                IsBackground = true,
                Name = "plain-thread-a"
            };
            var threadB = new Thread(() => PlainThreadB(workerStarted))
            {
                Name = "plain-thread-b"
            };

            Console.WriteLine("[2] 普通字段（真实竞态，结果不保证）");
            threadA.Start();
            threadB.Start();

            threadB.Join();
            bool stopped = threadA.Join(WorkerTimeout);

            Console.WriteLine("线程 B：已写入 true。");
            Console.WriteLine(stopped
                ? "线程 A：本次也退出了。这是允许的，但不代表该写法线程安全。"
                : "线程 A：等待超时，复现了没有可见性保证时可能出现的死循环。");

            return stopped;
        }

        private static void RunCachedReadSimulation()
        {
            _plainStopRequested = false;
            using var workerStarted = new ManualResetEventSlim(false);

            var threadA = new Thread(() => CachedValueThreadA(workerStarted))
            {
                IsBackground = true,
                Name = "cached-value-thread-a"
            };
            var threadB = new Thread(() => PlainThreadB(workerStarted))
            {
                Name = "cached-value-thread-b"
            };

            Console.WriteLine("[3] 缓存旧值（确定性模拟）");
            Console.WriteLine("本机没有在真实竞态中复现，下面显式模拟 JIT 将读取移出循环的效果。");
            threadA.Start();
            threadB.Start();

            threadB.Join();
            bool stopped = threadA.Join(WorkerTimeout);

            Console.WriteLine("线程 B：共享字段已经是 true。");
            Console.WriteLine(stopped
                ? "线程 A：意外退出。"
                : "线程 A：仍使用进入循环前缓存的 false，因此等待超时。");
            Console.WriteLine("注意：这是概念模拟，不表示 JIT 每次都会做这种优化。");
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
        private static void VolatileThreadA(ManualResetEventSlim workerStarted)
        {
            workerStarted.Set();

            // volatile 读具有 acquire 语义；循环体必须保持为空，避免其他调用干扰实验。
            while (!_volatileStopRequested)
            {
            }
        }

        private static void VolatileThreadB(ManualResetEventSlim workerStarted)
        {
            workerStarted.Wait();
            Thread.Sleep(WriterDelayMilliseconds);

            // volatile 写具有 release 语义，线程 A 会继续从该字段执行 volatile 读。
            _volatileStopRequested = true;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
        private static void PlainThreadA(ManualResetEventSlim workerStarted)
        {
            workerStarted.Set();

            // 没有 volatile 时，内存模型不保证线程 A 能观察到线程 B 的写入。
            while (!_plainStopRequested)
            {
            }
        }

        private static void PlainThreadB(ManualResetEventSlim workerStarted)
        {
            workerStarted.Wait();
            Thread.Sleep(WriterDelayMilliseconds);
            _plainStopRequested = true;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.AggressiveOptimization)]
        private static void CachedValueThreadA(ManualResetEventSlim workerStarted)
        {
            // 只读取一次共享字段，确定性模拟 JIT 可能把普通字段读取提升到循环外的结果。
            bool cachedStopRequested = _plainStopRequested;
            workerStarted.Set();

            while (!cachedStopRequested)
            {
            }
        }
    }
}
