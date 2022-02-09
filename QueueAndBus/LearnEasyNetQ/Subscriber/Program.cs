// See https://aka.ms/new-console-template for more information
using EasyNetQ;
using TextMessageLib;

//快速开始之订阅消息
using (var bus = RabbitHutch.CreateBus("host=localhost"))
{
    Console.WriteLine("Listening for messages. Hit <return> to quit.");

    //using Subscribers
    bus.PubSub.Subscribe<TextMessage>("test", message =>
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Got message: {0}", message.Text);
        Console.ResetColor();
    });



    //------------>我是分割线---------------->




    //using Send
    ISendReceive Ireceive = bus.SendReceive;
    Ireceive.Receive<TextMessage>("my.queue", message =>
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
