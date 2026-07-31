namespace LambdaAndLINQ_ActionAndFunc
{

    /// <summary>
    /// 使用 .NET 内置的类库 委托
    /// </summary>
    class Program_ActionAndFunc
    {
        static void Main_ActionAndFunc(string[] args)
        {
            //无返回值的内置的委托类型
            Action action = new Action(M1);
            action();

            //Action<string, string> sayHelloAction = new Action<string, string>(SayHello);
            var sayHelloAction = new Action<string, string>(SayHello);
            sayHelloAction("Rhys", "Kevin");

            Action<string, int> actionLoop = new Action<string, int>(SayHelloLoop);
            actionLoop("Rhys", 3);

            //有返回值
            Func<int, int, int> func = new Func<int, int, int>(Add);
            int res = func(100, 200);
            Console.WriteLine(res);



        }

        static void M1()
        {
            Console.WriteLine("M1 is called.");
        }

        static void SayHello(string name1, string name2)
        {
            Console.WriteLine($"Hello, {name1} and {name2}!");
        }

        static void SayHelloLoop(string name, int round)
        {
            for (int i = 0; i < round; i++)
            {
                Console.WriteLine($"Hello, {name} !");

            }
        }

        static int Add(int x, int y)
        {
            return x + y;
        }

        static double Mul(double x, double y)
        {
            return x * y;
        }
    }


}