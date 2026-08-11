namespace CSharpBasic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //decimal number = 125.456789m;
            decimal number = 2.25m;
            // 常见的用法:
            Console.WriteLine(number.ToString("F0"));
            Console.WriteLine(number.ToString("F1"));
            Console.WriteLine(number.ToString("F2"));

            // when number is 2.25
            Console.WriteLine(number.ToString("F1") == Math.Round(number, 1, MidpointRounding.ToEven).ToString());
            Console.WriteLine(number.ToString("F1") == Math.Round(number, 1, MidpointRounding.AwayFromZero).ToString());

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("ToEven");
            Console.WriteLine(Math.Round(number, 1, MidpointRounding.ToEven));
            Console.WriteLine("AwayFromZero");
            Console.WriteLine(Math.Round(number, 1, MidpointRounding.AwayFromZero));

        }

        //延伸的问题: 
        //https://stackoverflow.com/questions/14325214/incorrect-rounding-of-float-when-using-tostringf1
        //https://stackoverflow.com/questions/11085052/round-twice-error-in-nets-double-tostring-method

    }
}