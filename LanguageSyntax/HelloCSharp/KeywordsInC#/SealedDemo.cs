using System;
using System.Collections.Generic;
using System.Text;

namespace KeywordsInCSharp
{
    // sealed (密封) 关键字 （在 C# 的官方语法定义中：sealed 既是关键字（Keyword），也是修饰符（Modifier）。）
    // 作用：
    // 1：为了防止子类去“魔改”规则。你作为框架的基类设计者，提供了一个标准的上下文给子类用，你不希望子类再去继承 从而破坏你底层统一的内存管理或分发逻辑。
    // 2：JIT 性能压榨 (Devirtualization) 去虚化（Devirtualization）与内联（Inlining）。在没有 sealed 的情况下，当你通过基类引用调用一个虚方法或重写方法时，CLR 必须在运行时去查找对象的虚方法表（V-Table），以确定到底该执行哪个子类的具体代码。这个间接寻址的过程会带来微小的性能损耗，更致命的是，它阻止了 JIT 编译器将该方法内联（Inline）。

    #region 性能提升的核心原理拆解

    /**
1. 虚方法调用开销与“去虚化 (Devirtualization)”
在没有任何约束的普通类中，当你调用一个对象的方法时（尤其是重写的方法或实现了接口的方法），底层 CPU 不能直接跳转到代码执行，而是要经历一个“查表”的过程：

找到对象实例在内存中的地址。

找到该对象的类型指针（MethodTable）。

顺藤摸瓜找到虚方法表 (V-Table)。

从表里查出这个方法到底是由哪个子类实现的，拿到真实的内存地址。

最后才跳转执行。

这就是所谓的间接调用 (Indirect Call)，它不仅慢，还会破坏 CPU 的指令分支预测。

sealed 的魔法：
当你把类标记为 sealed，JIT 编译器在编译阶段就拿到了“免死金牌”——它 100% 确信这个类不可能有子类了。于是，JIT 会直接把上述繁琐的查表过程全部砍掉，将方法的调用直接变成直接调用 (Direct Call)。这就是去虚化。

2. 去虚化带来的终极奖励：“内联 (Inlining)”
去虚化本身能省下一点点查表的时间，但这并不是最大的性能收益。去虚化真正的价值，是它解锁了方法内联的前提条件。

JIT 编译器非常聪明，如果它发现一个方法体积很小（比如只是获取一个属性，或者做个简单的数学计算），它会想把这个方法里的代码直接“复制粘贴”到调用它的地方，从而彻底省去方法调用的栈帧创建、参数压栈等开销（这就是内联）。

但是，如果一个方法是虚方法（或者可能被子类重写），JIT 是绝对不敢内联它的，因为运行时到底执行哪段代码根本不确定。
只有当你加上 sealed（或者是 sealed override），明确告诉 JIT 代码实现是唯一的，JIT 才会毫无顾忌地把你的行情处理逻辑直接内联到主循环中。

3. 数组协变检查优化（隐藏的性能杀手）
除了方法调用，sealed 在处理数组时还能提供额外的性能。
在 .NET 中，如果你把一个对象存入数组（例如 object[] arr = new string[10]; arr[0] = new MyClass();），CLR 每次赋值都会在运行时进行类型安全检查（Type Check），防止你把错误类型的对象塞进去。
如果你定义的行情实体类是 sealed class TickData，由于它不可能有子类，CLR 在处理 TickData[] 数组的赋值时，可以省去大量复杂的层级继承检查，写入速度更快。
     * **/

    #endregion

    // 主要有两个作用目标：
    // 类（Class）和重写的方法/属性（Overridden Members）。

    // 1. 作用于类 (Sealed Classes)
    // 当把 sealed 放在类声明前，意味着这个类处于继承链的绝对底端，任何其他类都无法继承它。
    public sealed class Cat
    {
        public int Age { get; set; }
    }
    //public class BlackCat : Cat { } // 将会发生编译错误(活动)  CS0509	“BlackCat”: 无法从密封类型“Cat”派生 
    //C# 中的所有结构体（struct，包括 int, double 等）都是隐式密封的，你不能继承一个 struct。


    //2. 作用于方法或属性 (Sealed Methods/Properties)
    // sealed 不能直接用于普通方法或虚方法（virtual），它只能与 override 关键字组合使用。
    //它的作用是：允许当前类重写父类的方法，但禁止它的子类继续重写该方法。

    public class ConnectionBase
    {
        public virtual void Connect() { Console.WriteLine("Base Connect"); }
    }

    public class RedisConnection : ConnectionBase
    {
        // 重写并密封该方法
        public sealed override void Connect()
        {
            //base.Connect();
            Console.WriteLine("Redis Connect");
        }
    }

    public class AdvancedRedisConnection : RedisConnection
    {
        //编译错误：“AdvancedRedisConnection.Connect()”: 继承成员“RedisConnection.Connect()”是密封的，无法进行重写
        //public override void Connect() { }
    }



    /**
     * 三、 架构设计与最佳实践
在工程化实践中，关于 sealed 的使用有一个著名的架构争论：“默认开放”还是“默认密封”？

在早期的 .NET 开发中，大家习惯不加 sealed，方便以后随时继承。但在现代 .NET 架构（特别是微服务和领域驱动设计 DDD）中，“默认密封（Seal by Default）”已成为顶级专家的共识：

1. 强烈建议使用 sealed 的场景：

DTO（数据传输对象）与 ViewModel：这类纯数据承载类（如你发往前端的 JSON 实体或 Redis 缓存实体）没有任何继承的意义。

配置类（Options/Settings）：保障配置结构的严格性。

核心安全或业务规则类：防止恶意代码通过继承来重写并绕过你的权限校验或行情清洗规则。

实现特定接口的服务类：如果你通过依赖注入（DI）注册了 IMarketSyncService，具体的实现类 RedisMarketSyncService 应该被 sealed。外界只需要依赖接口，不需要继承你的实现。

2. 不该使用 sealed 的场景：

设计为框架基类的类型：例如你写了一个通用的 TcpClientBase，本来就是指望业务去继承并实现特定协议的。

EF Core 的代理实体：如果使用 Entity Framework Core 的延迟加载（Lazy Loading），EF 需要在运行时动态生成子类代理，此时实体类不能被 sealed。

一句话总结：
当你写下一个新的 class 时，如果脑海里没有明确的“这个类未来会被谁继承”的蓝图，请果断加上 sealed。它不仅能让你的意图更清晰，更能免费白嫖到底层的性能优化。
     * **/

}
