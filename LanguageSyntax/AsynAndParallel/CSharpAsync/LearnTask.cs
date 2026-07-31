using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAsync
{

    /// <summary>
    /// 学习异步编程
    /// </summary>
    public class LearnTask
    {
        public static void TestDemo()
        {
            //C# 异步编程基础知识

            //Task
            //Task<T>

            Task.Delay(TimeSpan.FromSeconds(1)); //用于延时执行任务，只能用于异步等待任务，等待过程中不会影响UI操作，仍能保持界面操作流畅；
            Console.WriteLine("im here");

            Thread.Sleep(TimeSpan.FromSeconds(1));//如果不通过异步方法来执行，会影响到UI操作，休眠期间界面有停顿现象。
            Console.WriteLine("im here now");

            Console.WriteLine("------------------------\r\n");


            //Lambda表达式 主要用于简化委托的代码编写。
            //lambda的概念建立在委托的基础上
            //Lambda表达式本质上就是一种匿名委托。

            //委托本质上仍旧是一个类，该类继承自 System.MulticastDelegate 类，该类维护一个带有链接的委托列表，在调用多播委托时，将按照委托列表的委托顺序而调用的。
            //还包括一个接受两个参数的构造函数和 3 个重要方法：BeginInvoke、EndInvoke 和 Invoke。

            //.NET 的事件模型建立在委托机制之上，事件是对委托的封装

            //凡是能使用匿名委托的地方，都可以用Lambda表达式来实现

            //Action 和 Func 委托

            //Action委托封装了不带返回值的方法 (有0-16个输入参数，返回类型为 void )
            //Func委托封装了带返回值的方法 (有0-16个输入参数，返回类型为 TResult )

            Action actionDemo = () => SomeMethod();
            actionDemo();

            Action<string> a = ShowMessage;
            a("OK");

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

        }

        private static void SomeMethod()
        {
            Console.WriteLine("hi i am a method");
        }

        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
