// See https://aka.ms/new-console-template for more information
using EasyNetQ;
using EasyNetQ.Logging;
using TextMessageLib;

//快速开始之发布消息

//
LogProvider.SetCurrentLogProvider(ConsoleLogProvider.Instance);

using (var bus = RabbitHutch.CreateBus("host=localhost"))
{

    for (int i = 0; i < 800000; i++)
    {
        bus.PubSub.Publish(new TextMessage { Text = "这是测试消息：" + i });
        Console.WriteLine("Message published!");
    }
    var input = string.Empty;
    Console.WriteLine("Enter a message. 'Quit' to quit.");
    while ((input = Console.ReadLine()) != "Quit")
    {
        bus.PubSub.Publish(new TextMessage { Text = input });
        Console.WriteLine("Message published!");
    }
}


/****官网文档：
 * 
 连接RabbitMQ * Connecting-to-RabbitMQ:https://github.com/EasyNetQ/EasyNetQ/wiki/Connecting-to-RabbitMQ
 * 
 * 标准做法是在应用程序的整个生命周期内创建一个 IBus 实例。在您的应用程序关闭时处理它。
 * 大多数 EasyNetQ 操作都是 IBus 上的方法。您可以像这样创建一个 IBus 实例：
 * 
 * var bus = RabbitHutch.CreateBus(“host=myServer;virtualHost=myVirtualHost;username=mike;password=topsecret”);
 * 连接字符串参数说明:
        1，host（例如 host=localhost 或 host=192.168.2.56 或 host=myhost.mydomain.com）此字段是必需的。要指定要连接的端口，可以使用标准格式 host:port（例如 host=myhost.com:5673）。如果省略端口号，则使用默认 AMQP 端口 (5672)。要连接到 RabbitMQ 集群，
                    请指定以逗号分隔的每个集群节点（例如 host=myhost1.com,myhost2.com,myhost3.com）
        2，virtualHost 
        3，username 
        更多参考文档
                ……
        关闭连接 释放资源：
            bus.Dispose();


    日志： Logging
        LogProvider.SetCurrentLogProvider(ConsoleLogProvider.Instance);


*/////
