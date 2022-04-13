using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using EchoClient;
using System.Net;

//https://zhuanlan.zhihu.com/p/181239748
//https://blog.csdn.net/nxy_wuhao/category_9469658.html
class Program
{
    static async Task RunClientAsync()
    {
        var group = new MultithreadEventLoopGroup();
        try
        {
            var bootstrap = new Bootstrap();
            bootstrap
                .Group(group)
                .Channel<TcpSocketChannel>()
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    pipeline.AddLast(new EchoClientHandler());
                }));
            IChannel clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));
            Console.ReadLine();
            await clientChannel.CloseAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            await group.ShutdownGracefullyAsync();
        }
    }

    static void Main(string[] args) => RunClientAsync().Wait();
    
}