// See https://aka.ms/new-console-template for more information
using EasyNetQ;
using TextMessageLib;

//快速开始之订阅消息
using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
    Console.WriteLine("Listening for messages. Hit <return> to quit.");

    //using Subscribers
    //bus.PubSub.Subscribe<TextMessage>("test", message =>
    //{
    //    Console.ForegroundColor = ConsoleColor.Blue;
    //    Console.WriteLine("Got message: {0}", message.Text);
    //    Console.ResetColor();
    //});


    //消息路由（Topic Based Routing）: https://www.cnblogs.com/panzi/p/6337568.html#!comments
    bus.PubSub.Subscribe<TextMessage>("my_id", message =>
     {
         Console.ForegroundColor = ConsoleColor.Yellow;
         Console.WriteLine("Got message: {0}", message.Text);
         Console.ResetColor();
     },

     // 消息订阅方可以通过路由来过滤相应的消息。
     //x => x.WithTopic("*.B")
     x => x.WithTopic("X.*")
     );


    //------------>我是分割线---------------->




    //using Send
    bus.SendReceive.Receive<TextMessage>("my.queue", message =>
    {
        //HandleTextMessage(message);
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Got message: {0}", message.Text);
        Console.ResetColor();
    });
    Console.ReadLine();
}

Console.ReadKey();


static void HandleTextMessage(TextMessage textMessage)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Got message: {0}", textMessage.Text);
    Console.ResetColor();
}
