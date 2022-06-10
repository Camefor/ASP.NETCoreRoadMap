using PublishersAndSubscribersDemo;
using PublishersAndSubscribersDemo.Optimize;
using PublishersAndSubscribersDemo.Optimize2;
using System.Reflection;

//https://www.cnblogs.com/sheng-jie/p/6970091.html

//test code

var fishingRod = new FishingRod();

var man = new FishingMan("钓鱼人");

man.FishingRod = fishingRod;

//fishingRod.FishingEvent += new FishingEventHandler().HandleEvent; //FishingRod 注册

while (man.FishCount < 5)
{
    man.Fishing();
    Console.WriteLine("-------------------------------------");

    Thread.Sleep(5000);
}