using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoServer
{
    /// <summary>
    ///  因为服务器只需要响应传入的消息，所以只需要实现ChannelHandlerAdapter就可以了
    /// </summary>
    public class EchoServerHandler : ChannelHandlerAdapter
    {
        /// <summary>
        /// 每个传入消息都会调用
        /// 处理传入的消息需要复写这个方法
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="msg"></param>
        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            // ChannelHandlerContext
            //保存Channel相关的所有上下文信息，同时关联一个ChannelHandler。

            if (msg != null)
            {
                IByteBuffer? message = msg as IByteBuffer;
                Console.WriteLine("Receive From Client:" + message?.ToString(Encoding.UTF8));
            }
        }


        /// <summary>
        /// 客户端连接
        /// </summary>
        /// <param name="ctx"></param>
        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            var connectId = ctx.Channel.Id.AsShortText();
            Console.WriteLine($"客户端连接：{connectId}");
            var msgStr = $"连接成功.你的id:{connectId}";
            var bytes = Encoding.UTF8.GetBytes(msgStr);
            ctx.Channel.WriteAndFlushAsync(Unpooled.WrappedBuffer(bytes));
        }

        /// <summary>
        /// 客户端断开连接
        /// </summary>
        /// <param name="ctx"></param>
        public override void ChannelInactive(IChannelHandlerContext ctx)
        {
            Console.WriteLine($"客户端断开连接：{ctx.Channel.Id.AsShortText()}");
            ctx.Channel.CloseAsync();
        }
        /// <summary>
        /// 批量读取中的最后一条消息已经读取完成
        /// </summary>
        /// <param name="context"></param>
        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }
        /// <summary>
        /// 发生异常
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine(exception);
            Console.WriteLine(exception?.Message);
            context.Channel.CloseAsync();
            context.CloseAsync();

        }
    }
}
