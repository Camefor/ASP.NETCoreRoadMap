namespace LambdaAndLINQ_GenericDelegate
{

    /// <summary>
    /// 泛型委托
    /// </summary>
    class Program_GenericDelegate
    {
        static void Main_GenericDelegate(string[] args)
        {
            MyDele<int> deleAdd = new MyDele<int>(Add);
            Console.WriteLine(deleAdd(100, 200));

            MyDele<double> deleMul = new MyDele<double>(Mul);
            Console.WriteLine(deleMul(2.2, 3.3));

            Console.WriteLine(deleAdd.GetType().IsClass);
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

    delegate T MyDele<T>(T a, T b);

}