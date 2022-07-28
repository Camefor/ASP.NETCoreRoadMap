using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcGreeter;
using GrpcGreeterClient;

namespace GrpcGreeterClient
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = Grpc.Net.Client.GrpcChannel.ForAddress("https://localhost:7042");
            var greeterClient = new Greeter.GreeterClient(channel);//编译生成项目之后。
            var exampleClient = new Example.ExampleClient(channel);

            //GrpcGreeterClient 类型是由生成进程自动生成的。 工具包 Grpc.Tools 基于 greet.proto 文件生成以下文件：
            //GrpcGreeterClient\obj\Debug\[TARGET_FRAMEWORK]\Protos\Greet.cs：用于填充、序列化和检索请求和响应消息类型的协议缓冲区代码。
            //GrpcGreeterClient\obj\Debug\[TARGET_FRAMEWORK]\Protos\GreetGrpc.cs：包含生成的客户端类。
            int cancellationTokenConditionCounter = 0;
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken cancellationToken = source.Token;
            //CancellationToken cancellationToken = new CancellationToken();
            Console.WriteLine("call StreamingFromServer ");

            var d = exampleClient.StreamingFromServer(new ExampleRequest { IsDescending = false, PageIndex = 1, PageSize = 100 });
            while (await d.ResponseStream.MoveNext(cancellationToken))
            {
                if (cancellationTokenConditionCounter == 4)
                {
                    source.Cancel();
                }
                cancellationTokenConditionCounter++;
                var message = d.ResponseStream.Current;
                Console.WriteLine(message.Message);
            }

            for (int i = 0; i < 10000; i++)
            {
                //var reply = await greeterClient.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
                //Console.WriteLine("Greeting:    " + reply.Message);

                //var exampleServerReply = await exampleClient.UnaryCallAsync(new ExampleRequest { IsDescending = false, PageIndex = 1, PageSize = 100 });
                //Console.WriteLine("Greeting:    " + exampleServerReply.Message);



            }
            Console.WriteLine("Press any key to exit……");
            Console.ReadKey();

        }
    }
}