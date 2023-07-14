namespace HelloCSharp
{
    public class MyException : Exception
    {

        /// <summary>
        /// 错误消息（支持 Object 对象）
        /// </summary>
        public object ErrorMessage { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MyException() : base()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        public MyException(string message) : base(message)
        {
            ErrorMessage = message;
        }

    }

    internal class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine("运行结束……");
            Console.ReadKey();
        }

    }
}