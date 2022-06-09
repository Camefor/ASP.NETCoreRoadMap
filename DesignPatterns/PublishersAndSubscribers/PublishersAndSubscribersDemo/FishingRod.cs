using PublishersAndSubscribersDemo.Optimize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishersAndSubscribersDemo
{

    /// <summary>
    /// 鱼竿（被观察者）
    /// </summary>
    public class FishingRod
    {
        public delegate void FishingHandler(FishingEventData eventData);//声明委托
        public event FishingHandler? FishingEvent;//声明事件

        /// <summary>
        /// FishingMan 的 “ 钓鱼” 方法调用   抛竿
        /// </summary>
        /// <param name="man"></param>
        public void ThrowHook(FishingMan man)
        {
            Console.WriteLine("开始下钩……");

            //用随机数模拟鱼咬钩，若随机数为偶数，则为鱼咬钩
            if (new Random().Next() % 2 == 0)
            {
                var type = (FishTypeEnum)new Random().Next(0, 5);
                Console.WriteLine("铃铛：叮叮叮，鱼儿咬钩了");
                if (FishingEvent != null)
                    FishingEvent(new FishingEventData
                    {
                        FishType = type,
                        FisingMan = man,
                        EventSource = this
                    });
            }
        }
    }
}
