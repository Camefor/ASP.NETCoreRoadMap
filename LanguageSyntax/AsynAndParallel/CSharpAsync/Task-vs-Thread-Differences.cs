using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAsync
{
    /// <summary>
    /// https://stackoverflow.com/questions/13429129/task-vs-thread-differences
    /// https://www.albahari.com/threading/
    /// </summary>
    public class Task_vs_Thread_Differences
    {


        public static void TestDemo1()
        {
            Thread t = new Thread(WriteY);
            t.Start();
            //t.IsAlive //一旦启动，线程的 IsAlive 属性将返回 true，直到线程结束。
            //当传递给线程构造函数的委托完成执行时，线程结束。一旦结束，线程就无法重新启动。
            for (int i = 0; i < 1000; i++) Console.Write("x");

            //细节： https://www.albahari.com/threading/NewThread.png
        }

        public static void TestDemo2()
        {
            //CLR 为每个线程分配其自己的内存堆栈，以便局部变量保持独立。
            //在下一个示例中，我们使用局部变量定义一个方法，然后在主线程和新创建的线程上同时调用该方法：
            new Thread(Go).Start();//开启新线程

            Go();//在主线程执行

        }


        private static void WriteY()
        {
            for (int i = 0; i < 1000; i++) Console.Write("y");
        }

        private static void Go()
        {
            //局部变量
            for (int cycles = 0; cycles < 5; cycles++)
            {
                Console.Write("? ");
            }
        }

    }
}
