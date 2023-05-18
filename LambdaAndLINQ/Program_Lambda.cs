namespace LambdaAndLINQ_Lambda
{

    /// <summary>
    /// Lambda表达式:
    /// 匿名方法
    /// Inline方法 内联式， (声明-调用)
    /// </summary>
    class Program_Lambda
    {
        static void Main_Lambda(string[] args)
        {
            //var func = new Func<int, int, int>((int a, int b) => { return a + b; });
            //简写：
            //Func<int, int, int> func = new Func<int, int, int>((a, b) => { return a + b; });
            Func<int, int, int> func = (a, b) => { return a + b; };

            // => :Lambda操作符 箭头函数
            int res = func(100, 200);
            Console.WriteLine(res);

            func = new Func<int, int, int>((int x, int y) => { return x * y; });
            res = func(3, 2);
            Console.WriteLine(res);


            Console.WriteLine("==========================================");

            //DoSomeCalc<int>((a, b) => { return a * b; }, 100, 200);
            //泛型委托 类型推断：
            DoSomeCalc((a, b) => { return a * b; }, 100, 200);


        }

        //泛型方法：
        //泛型委托类型当其中一个参数
        //泛型参数
        static void DoSomeCalc<T>(Func<T, T, T> func, T x, T y)
        {
            T res = func(x, y); //T 返回值类型
            Console.WriteLine(res);
        }

        //泛型方法，要提供足够的泛型类型：以下方法要求两种泛型类型：T,T2
        //static void DoSomeCalc<T>(Func<T2, T2, T> func, T2 x, T2 y)
        //{
        //    T res = func(x, y); //T 返回值类型
        //    Console.WriteLine(res);
        //}



    }
}