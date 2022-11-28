using Polly;

namespace PollyDemo
{

    public class MyCusException : Exception
    {
        public new string Message { get; set; }
    }


    internal class Program
    {
        static async Task Main(string[] args)
        {

            var listData = await DoAsync<List<string>, MyCusException>(
              () =>
              {
                  Run();
                  var list = new List<string>();
                  return Task.FromResult(list);
              },
              TimeSpan.FromSeconds(10),
              c =>
              {
                  //异常过滤器
                  Console.WriteLine("进入 异常过滤器 ");
                  return true;
              }, 2);


            Console.ReadKey();
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
            Console.WriteLine("Hello, World!");
            throw new MyCusException();
        }

    }
}