using CSharpAsync;
using System.Threading.Tasks;

namespace CSharpAsync;
class Program
{
    private static async Task Main(string[] args)
    {


        //C# 异步编程基础知识

        //LearnTask.TestDemo();

        await Task.Delay(TimeSpan.FromSeconds(1)); //用于延时执行任务，只能用于异步等待任务，等待过程中不会影响UI操作，仍能保持界面操作流畅；
        Console.WriteLine("im here");

        Task_vs_Thread_Differences.TestDemo2();

        Console.ReadKey();


        Func<int, string, bool> funcDemo = (int x, string s) => s.Length > x;
        Console.WriteLine(funcDemo(3, "hello"));

        Console.WriteLine("------------------------\r\n");



        //方法中代码比较少时,更常见的做法是 用匿名方法和Lambda表达式一起使用
        Action a2 = () => Console.WriteLine("Ok from a2 action");
        a2();//无参数 Action委托

        Action<string> b = (s) => Console.WriteLine(s); //要求 一个string类型参数的委托
        b("OK from b action");

        Console.WriteLine("------------------------\r\n");



        //异步编程基本技术
        //目前流行的多线程编程模型基本上都是通过异步模式和并行编程模型来实现的
        //.NET 过时的异步编程的实现方式：
        //APM
        //EAP

        //.NET框架4.0时 引入 基于任务的异步模式 （Task-based Asynchronous Pattern, TAP）
        // 改进： 从C# 5,.NET 4.5时 引入 async await 关键字 以及Task.Run 方法。在TAP基础上改进模型、


        //async 是一个修饰符
        //await 是一个运算符可将其用于表达式中 该运算符表示等待异步执行的结果。
        //await运算符和同步编程的最大区别是：异步等待任务完成时，既不会继续执行其后面的代码，也不好影响用户对UI界面的操作。
        Task task1 = Method1Async();
        await task1;

        await Method1Async();


        Console.ReadKey();
    }

    private static void SomeMethod()
    {
        Console.WriteLine("hi i am a method");
    }

    private static void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }

    private static async Task Method1Async()
    {
        Console.WriteLine("i am Method1Async");
        await Task.Delay(TimeSpan.FromSeconds(1));
    }
}

