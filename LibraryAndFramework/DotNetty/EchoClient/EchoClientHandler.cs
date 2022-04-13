using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoClient
{

    public class EchoClientHandler : SimpleChannelInboundHandler<IByteBuffer>
    {
        public static int i = 0;

        /// <summary>
        /// Read0是DotNetty特有的对于Read方法的封装
        /// 封装实现了：
        /// 1. 返回的message的泛型实现
        /// 2. 丢弃非该指定泛型的信息
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="msg"></param>
        protected override void ChannelRead0(IChannelHandlerContext ctx, IByteBuffer msg)
        {
            if (msg != null)
            {
                i++;
                Console.WriteLine($"Receive From Server {i}:" + msg.ToString(Encoding.UTF8));
            }
            ctx.WriteAsync(Unpooled.CopiedBuffer(msg));
        }

        public override void ChannelActive(IChannelHandlerContext context)
        {
            Console.WriteLine($"发送客户端消息");
            for (int i = 0; i < 100; i++)
            {
                context.WriteAndFlushAsync(Unpooled.CopiedBuffer(Encoding.UTF8.GetBytes($"客户端消息:{i}!")));
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine(exception);
            Console.WriteLine(exception?.Message);
            context.CloseAsync();
        }

    }
}
