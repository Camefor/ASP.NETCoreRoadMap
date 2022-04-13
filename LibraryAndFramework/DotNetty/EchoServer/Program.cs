using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using EchoServer;
using System.Net;

class Program
{
    static async Task RunServerAsync()
    {
        IEventLoopGroup eventLoop;
        eventLoop = new MultithreadEventLoopGroup();
        try
        {
            ServerBootstrap b = new ServerBootstrap();
            b.Group(eventLoop);
            b.Channel<TcpServerSocketChannel>();
            b.ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
            {
                IChannelPipeline pipeline = channel.Pipeline;
                pipeline.AddLast(new EchoServerHandler());
            }));

            IChannel ch = await b.BindAsync(8080);

            Console.WriteLine("Server started and listen on: " + ((IPEndPoint)ch.LocalAddress).Port);
            Console.ReadLine();

            await ch.CloseAsync();
        }
        finally
        {
            await eventLoop.ShutdownGracefullyAsync();
        }
    }

    static void Main(string[] args) => RunServerAsync().Wait();

}