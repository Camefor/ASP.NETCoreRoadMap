using Grpc.Core;

namespace GrpcGreeter.Services
{
    public class ExampleService : Example.ExampleBase
    {
        private readonly ILogger<ExampleService> _logger;
        public ExampleService(ILogger<ExampleService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 一元方法(简单 RPC（Unary RPC）)
        ///  一般的rpc调用，传入一个请求对象，返回一个返回对象；一般的rpc调用，传入一个请求对象，返回一个返回对象
        /// 一元方法以参数的形式获取请求消息，并返回响应。 返回响应时，一元调用完成。
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ExampleResponse> UnaryCall(ExampleRequest request, ServerCallContext context)
        {
            var userAgent = context.RequestHeaders.GetValue("user-agent");
            _logger.LogInformation("this client's ua is " + userAgent);
            return Task.FromResult(new ExampleResponse { Message = "hello world from UnaryCall" });
        }

        /// <summary>
        /// 服务器流式处理方法以参数的形式获取请求消息。
        /// 由于可以将多个消息流式传输回调用方，因此可使用 responseStream.WriteAsync 发送响应消息。 当方法返回时，服务器流式处理调用完成。
        /// 传入一个请求对象，服务端可以返回多个结果对象
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task StreamingFromServer(ExampleRequest request, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            // 某些流式处理方法设计为永久运行
            //for (int i = 0; i < 5; i++)
            //{
            //模拟不断的有数据要发送给客户端
            //    await responseStream.WriteAsync(new ExampleResponse { Message = " this response from  UnaryCall； index: " + i });
            //    await Task.Delay(TimeSpan.FromSeconds(1));
            //}


            //对于连续流式处理方法，客户端可以在不再需要调用时将其取消。 当发生取消时，客户端会将信号发送到服务器，并引发 ServerCallContext.CancellationToken。 应在服务器上通过异步方法使用 CancellationToken 标记
            int i = 0;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("enter");
                i++;
                await responseStream.WriteAsync(new ExampleResponse { Message = "this response from   StreamingFromServer； index: " + i });
                await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
            }
            _logger.LogInformation("exit response");
        }

        //public override async Task<ExampleResponse> StreamingFromClient(IAsyncStreamReader<ExampleRequest> requestStream, ServerCallContext context)
        //{
        //    while (await requestStream.MoveNext())
        //    {
        //        var message = requestStream.Current;
        //        _logger.LogInformation($"receive a message from client use StreamingFromClient : {message}");
        //    }
        //    return new ExampleResponse { Message = "hello world   StreamingFromClient； " };
        //}

        public override async Task<ExampleResponse> StreamingFromClient(
            IAsyncStreamReader<ExampleRequest> requestStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                _logger.LogInformation($"receive a message from client use StreamingFromClient : {message}");
                return new ExampleResponse { Message = "hello world   StreamingFromClient； " };
            }
            return new ExampleResponse { Message = "hello world   StreamingFromClient； " };
        }

        public override async Task StreamingBothWays(IAsyncStreamReader<ExampleRequest> requestStream, IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            var counter = 0;
            await foreach (var message in requestStream.ReadAllAsync())
            {
                _logger.LogInformation($"receive a message from client use StreamingBothWays : {message}");
                await responseStream.WriteAsync(new ExampleResponse { Message = $"this response from   StreamingBothWays :{counter} " });
            }
        }

    }
}
