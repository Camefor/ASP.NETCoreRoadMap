using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Core.Interceptors;

namespace GrpcGreeterClient
{
    public class Program
    {
        //GrpcGreeterClient 类型是由生成进程自动生成的。 工具包 Grpc.Tools 基于 greet.proto 文件生成以下文件：
        //GrpcGreeterClient\obj\Debug\[TARGET_FRAMEWORK]\Protos\Greet.cs：用于填充、序列化和检索请求和响应消息类型的协议缓冲区代码。
        //GrpcGreeterClient\obj\Debug\[TARGET_FRAMEWORK]\Protos\GreetGrpc.cs：包含生成的客户端类。

        static async Task Main(string[] args)
        {
            //gRPC 客户端是通过通道创建的。 首先使用 GrpcChannel.ForAddress 创建一个通道，然后使用该通道创建 gRPC 客户端
            //配置客户端选项:https://docs.microsoft.com/zh-cn/aspnet/core/grpc/configuration?view=aspnetcore-6.0#configure-client-options
            using var channel = GrpcChannel.ForAddress("https://localhost:7042", new GrpcChannelOptions
            {
                MaxReceiveMessageSize = 5 * 1024 * 1024, // 5 MB
                MaxSendMessageSize = 2 * 1024 * 1024 // 2 MB
            });
            //客户端侦听器
            //var callInvoker = channel.Intercept(new LoggingInterceptor());


            //channel.ShutdownAsync().Wait();

            //var greeterClient = new Greeter.GreeterClient(channel);//编译生成项目之后。
            //var greeterClientResult = await greeterClient.SayHelloAsync(new HelloRequest { Name = "Rhys" });
            //Console.WriteLine(greeterClientResult.Message);
            //var reply = await greeterClient.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
            //Console.WriteLine("Greeting:    " + reply.Message);


            //var exampleClient = new Example.ExampleClient(channel);
            ////一元方法
            //for (int i = 0; i < 10; i++)
            //{
            //    var exampleServerReply = await exampleClient.UnaryCallAsync(new ExampleRequest { IsDescending = false, PageIndex = 1, PageSize = 100 });
            //    Console.WriteLine("Greeting:    " + exampleServerReply.Message);
            //}


            ////服务器流式处理方法
            //CancellationTokenSource tokenSource = new CancellationTokenSource();
            //var d = exampleClient.StreamingFromServer(new ExampleRequest { IsDescending = false, PageIndex = 1, PageSize = 100 }, cancellationToken: tokenSource.Token);
            //try
            //{
            //    tokenSource.CancelAfter(TimeSpan.FromSeconds(5));
            //    while (await d.ResponseStream.MoveNext(tokenSource.Token))
            //    //while (await d.ResponseStream.MoveNext(cancellationToken: tokenSource.Token))
            //    {
            //        var message = d.ResponseStream.Current;
            //        Console.WriteLine(message?.Message);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            //2:

            //var client = new Example.ExampleClient(channel);
            //using var call = client.StreamingFromServer(new ExampleRequest { PageIndex = 1, PageSize = 100, IsDescending = false });

            //await foreach (var response in call.ResponseStream.ReadAllAsync())
            //{
            //    Console.WriteLine("Greeting: " + response.Message);
            //    // "Greeting: Hello World" is written multiple times
            //}



            //客户端流式处理方法:
            //客户端无需发送消息即可开始客户端流式处理调用 。
            //客户端可选择使用 RequestStream.WriteAsync 发送消息。
            //客户端发送完消息后，应调用 RequestStream.CompleteAsync() 来通知服务。 服务返回响应消息时，调用完成。
            //var client = new Example.ExampleClient(channel);
            //using var call = client.StreamingFromClient();
            //for (int i = 0; i < 5; i++)
            //{
            //    Console.WriteLine("current index is " + i);
            //    await call.RequestStream.WriteAsync(new ExampleRequest { IsDescending = false, PageIndex = i, PageSize = 666 }); //输入参数
            //}
            //await call.RequestStream.CompleteAsync();
            //var response = await call;
            //Console.WriteLine(response.Message);

            //双向流式处理方法
            //var exampleClientResult = exampleClient.StreamingBothWays();
            //for (int i = 0; i < 5; i++)
            //{
            //    await exampleClientResult.RequestStream.WriteAsync(new ExampleRequest { IsDescending = true }); //输入参数
            //}
            //while (await exampleClientResult.ResponseStream.MoveNext())
            //{
            //    var message = exampleClientResult.ResponseStream.Current; //获取响应
            //    Console.WriteLine(message?.Message);
            //}

            var client = new Example.ExampleClient(channel);
            using var call = client.StreamingBothWays();
            Console.WriteLine("Starting background task to receive messages");

            var readTask = Task.Run(async () =>
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(response.Message);
                }
            });

            Console.WriteLine("Starting to send messages");
            Console.WriteLine("Type a message to echo then press enter.");

            while (true)
            {
                var result = Console.ReadLine();
                if (string.IsNullOrEmpty(result))
                {
                    break;
                }
                await call.RequestStream.WriteAsync(new ExampleRequest { IsDescending = false, PageIndex = 1, PageSize = 666 });
            }

            Console.WriteLine("Disconnecting");
            await call.RequestStream.CompleteAsync();
            await readTask;


            Console.WriteLine("Press any key to exit……");
            Console.ReadKey();

        }
    }
}