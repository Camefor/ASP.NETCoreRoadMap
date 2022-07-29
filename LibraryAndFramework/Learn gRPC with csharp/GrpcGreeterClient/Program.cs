using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;

namespace GrpcGreeterClient
{
    public class Program
    {
        //GrpcGreeterClient 类型是由生成进程自动生成的。 工具包 Grpc.Tools 基于 greet.proto 文件生成以下文件：
        //GrpcGreeterClient\obj\Debug\[TARGET_FRAMEWORK]\Protos\Greet.cs：用于填充、序列化和检索请求和响应消息类型的协议缓冲区代码。
        //GrpcGreeterClient\obj\Debug\[TARGET_FRAMEWORK]\Protos\GreetGrpc.cs：包含生成的客户端类。

        static async Task Main(string[] args)
        {
            using var channel = Grpc.Net.Client.GrpcChannel.ForAddress("https://localhost:7042");
            //channel.ShutdownAsync().Wait();

            //var greeterClient = new Greeter.GreeterClient(channel);//编译生成项目之后。
            //var greeterClientResult = await greeterClient.SayHelloAsync(new HelloRequest { Name = "Rhys" });
            //Console.WriteLine(greeterClientResult.Message);
            //var reply = await greeterClient.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
            //Console.WriteLine("Greeting:    " + reply.Message);


            var exampleClient = new Example.ExampleClient(channel);
            //一元方法
            for (int i = 0; i < 10; i++)
            {
                var exampleServerReply = await exampleClient.UnaryCallAsync(new ExampleRequest { IsDescending = false, PageIndex = 1, PageSize = 100 });
                Console.WriteLine("Greeting:    " + exampleServerReply.Message);
            }


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

            //客户端流式处理方法
            //for (int i = 0; i < 5; i++)
            //{
            //    var exampleClientResult = exampleClient.StreamingFromClient();
            //    Console.WriteLine("current index is " + i);
            //    await exampleClientResult.RequestStream.WriteAsync(new ExampleRequest { IsDescending = false, PageIndex = i, PageSize = 666 }); //输入参数
            //    Console.WriteLine((await exampleClientResult.ResponseAsync).Message);
            //}


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


            Console.WriteLine("Press any key to exit……");
            Console.ReadKey();

        }
    }
}