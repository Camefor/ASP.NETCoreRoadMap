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
            //    await responseStream.WriteAsync(new ExampleResponse { Message = "hello world from UnaryCall； index: " + i });
            //    await Task.Delay(TimeSpan.FromSeconds(1));
            //}


            //对于连续流式处理方法，客户端可以在不再需要调用时将其取消。 当发生取消时，客户端会将信号发送到服务器，并引发 ServerCallContext.CancellationToken。 应在服务器上通过异步方法使用 CancellationToken 标记
            int i = 0;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("enter");
                i++;
                await responseStream.WriteAsync(new ExampleResponse { Message = "hello world from UnaryCall； index: " + i });
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            _logger.LogInformation("exit");
        }

    }
}
