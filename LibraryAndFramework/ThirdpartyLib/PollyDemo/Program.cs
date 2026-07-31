using Polly;
using static System.Net.Mime.MediaTypeNames;

namespace PollyDemo
{

    public class MyCusException : Exception
    {
        public new string Message { get; set; }
    }


    internal class Program
    {
        static int Counter = 0;

        static async Task Main(string[] args)
        {

            var retryDur = new[]
                {
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(8),
                    TimeSpan.FromSeconds(8)
                };

            var res = await CommonRetryDoAsync<string, MyCusException>
                (RunAsync, retryDur, (e, t) =>
                {
                    Counter++;
                    Console.WriteLine($"on retry: 开始第{Counter}次重试");
                });


            //Policy.Handle<MyCusException>().WaitAndRetry(new[]
            //    {
            //        TimeSpan.FromSeconds(3),
            //        TimeSpan.FromSeconds(8),
            //        TimeSpan.FromSeconds(8)

            //    }, (e, t) =>
            //    {
            //        Counter++;
            //        Console.WriteLine($"on retry :第{Counter}次重试");
            //    }).Execute(() =>
            //    {
            //        //do something
            //        Run();
            //    });

            //var listData = await DoAsync<List<string>, MyCusException>(
            //  () =>
            //  {
            //      Run();
            //      var list = new List<string>();
            //      return Task.FromResult(list);
            //  },
            //  TimeSpan.FromSeconds(10),
            //  c =>
            //  {
            //      //异常过滤器
            //      Console.WriteLine("进入 异常过滤器 ");
            //      return true;
            //  }, 2);


            Console.WriteLine("程序运行结束");
            Console.ReadKey();
        }


        /// <summary>
        /// 封装具有重试策略的异步方法调用
        /// </summary>
        /// <typeparam name="T">返回值类型 业务方法定义</typeparam>
        /// <typeparam name="TEx">特定的异常类型（Exception 类的子类）触发重试</typeparam>
        /// <param name="action">执行的业务方法</param>
        /// <param name="sleepDurations">重试策略 重试间隔</param>
        /// <param name="onRetry">触发重试过滤器， 在里面可以执行一些其他逻辑，比如可以记录日志使用</param>
        /// <returns></returns>
        public static async Task<T> CommonRetryDoAsync<T, TEx>
            (Func<Task<T>> action,
            IEnumerable<TimeSpan> sleepDurations,
            Action<Exception, TimeSpan> onRetry) where TEx : Exception
        {

            var policyResult = await Policy
                .Handle<TEx>()
                .WaitAndRetryAsync(sleepDurations, onRetry).ExecuteAndCaptureAsync(action);
            if (policyResult.Outcome == OutcomeType.Failure)
            {
                throw policyResult.FinalException; //抛出业务方法异常
            }
            return policyResult.Result;
        }


        public static async Task<T> DoAsync<T, TEx>
            (Func<Task<T>> action, TimeSpan retryWait,
            Func<TEx, bool> exceptionFilter,
            int retryCount = 1) where TEx : Exception
        {
            var policyResult = await Policy
                .Handle<TEx>(exceptionFilter)
                .WaitAndRetryAsync(retryCount, retryAttempt => retryWait)
                .ExecuteAndCaptureAsync(action);

            if (policyResult.Outcome == OutcomeType.Failure)
            {
                throw policyResult.FinalException;
            }

            return policyResult.Result;
        }

        public static async Task<T> Do2Async<T, TEx>(Func<Task<T>> action, TimeSpan retryWait, Func<TEx, bool> exceptionFilter, int retryCount = 0) where TEx : Exception
   => await Policy
        .Handle<TEx>(exceptionFilter)
        .WaitAndRetryAsync(retryCount, retryAttempt => retryWait)
        .ExecuteAsync(action);


        private static void Run()
        {

            Console.WriteLine("hi working \r\n");
            if (Counter >= 3)
            {
                return;
            }
            throw new MyCusException();
        }

        private static async Task<string> RunAsync()
        {
            await Task.Delay(500);
            if (Counter >= 3)
            {
                Console.WriteLine("sucess,hello world \r\n");
                return "sucess,hello world";
            }
            Console.WriteLine("hi working \r\n");
            throw new MyCusException();
        }

    }
}