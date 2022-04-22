using DotNetty.Buffers;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using EchoClient;
using System.Net;
using System.Text;
using System.Threading.Tasks;

//https://zhuanlan.zhihu.com/p/181239748
//https://blog.csdn.net/nxy_wuhao/category_9469658.html
class Program
{

    private static async Task NewMethod()
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

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            //await group.ShutdownGracefullyAsync();
        }
    }

    static void Main(string[] args)
    {

        for (int i = 0; i < 1000; i++)
        {
            Console.WriteLine($"第{i + 1}次连接");
            //_ = NewMethod();
            //NewMethod().ConfigureAwait(false).GetAwaiter().GetResult(); //await
            NewMethod().ConfigureAwait(false);
        }
    }

}