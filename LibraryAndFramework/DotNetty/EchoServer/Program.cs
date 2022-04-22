using DotNetty.Buffers;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using EchoServer;
using System.Net;
using System.Text;

class Program
{
    static async Task RunServerAsync()
    {
        //日志配置
        InternalLoggerFactory.DefaultFactory.AddProvider(new EventSourceLoggerProvider());

        //可以理解为线程池
        IEventLoopGroup eventLoop = new MultithreadEventLoopGroup();
        try
        {

            //https://juejin.cn/post/6844904008541274119


            //ServerBootstrap是服务端引导类
            ServerBootstrap serverBootstrap = new ServerBootstrap();

            // TcpServerSocketChannel
            // Channel
            //Netty网络通信的组件，用于网络IO操作。
            //通过Channel可以获得当前王略连接的通道的状态与网络配置参数。
            //Channel提供异步的网络IO操作，调用后立即返回ChannelFuture，通过注册监听，或者同步等待，最终获取结果。
            serverBootstrap
                .Group(eventLoop)
                .Channel<TcpServerSocketChannel>()
                .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
            {
                //ChannelHandler
                //ChannelHandler属于业务的核心接口，用于处理IO事件或者拦截IO操作，并将其转发到ChannelPipeline（业务处理链）

                // Pipeline与ChannelPipeline
                //ChannelPipeline是一个handler的集合，它负责处理和拦截出站和入站的事件和操作。
                //ChannelPipeline实现了拦截过滤器模式，使用户能控制事件的处理方式。
                //在Netty中，每个Channel都有且只有一个ChannelPipeline与之对应。
                IChannelPipeline pipeline = channel.Pipeline;
                pipeline.AddLast(new EchoServerHandler());
            }));

            IChannel channel = await serverBootstrap.BindAsync(8080);

            Console.WriteLine("Server started and listen on: " + ((IPEndPoint)channel.LocalAddress).Port);
            Console.ReadLine();
            await channel.CloseAsync();
        }
        finally
        {
            await eventLoop.ShutdownGracefullyAsync();
        }
    }

    static void Main(string[] args) => RunServerAsync().Wait();

}