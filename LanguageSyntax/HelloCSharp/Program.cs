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
            try
            {
                var result = TestTryCatch();


                Console.WriteLine();
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            Console.WriteLine("运行结束……");
            Console.ReadKey();
        }

        static string TestTryCatch()
        {
            string res_msg = string.Empty;
            try
            {
                if (new Random().Next(1, 2) != 2)
                {
                    res_msg = "模拟异常 即将抛出异常";

                    throw new MyException("模拟异常");
                }
                res_msg = "我是返回的数据";
                return "我是返回的数据";
            }
            catch (MyException mex)
            {
                throw;

            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                Console.WriteLine("finally \r\n");
                Console.WriteLine("finally after " + res_msg);
            }

        }
    }
}