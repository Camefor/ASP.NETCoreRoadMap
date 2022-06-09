using PublishersAndSubscribersDemo;

//https://www.cnblogs.com/sheng-jie/p/6970091.html


var fishingRod = new FishingRod();

var man = new FishingMan("钓鱼人");

man.FishingRod = fishingRod;

fishingRod.FishingEvent += man.Update;

while (man.FishCount < 5)
{
    man.Fishing();
    Console.WriteLine("-------------------------------------");

    Thread.Sleep(5000);
}