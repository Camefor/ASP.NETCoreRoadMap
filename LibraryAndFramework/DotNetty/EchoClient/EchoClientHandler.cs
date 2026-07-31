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
                Console.WriteLine($"Receive From Server:" + msg.ToString(Encoding.UTF8));
            }
        }


        public override void ChannelActive(IChannelHandlerContext context)
        {

        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            Console.WriteLine(exception);
            Console.WriteLine(exception?.Message);
            context.CloseAsync();
        }

    }
}
