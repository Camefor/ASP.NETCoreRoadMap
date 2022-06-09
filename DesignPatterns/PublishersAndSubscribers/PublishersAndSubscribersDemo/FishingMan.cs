using PublishersAndSubscribersDemo.Optimize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishersAndSubscribersDemo
{
    /// <summary>
    /// 垂钓者 （观察者）
    /// </summary>
    public class FishingMan : IEventHandler<IEventData>
    {

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="name"></param>
        public FishingMan(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public int FishCount { get; set; }

        /// <summary>
        /// 鱼竿
        /// </summary>
        public FishingRod FishingRod { get; set; }

        public void Fishing()
        {
            FishingRod.ThrowHook(this);
        }

        public void HandleEvent(IEventData eventData)
        {

            if (eventData is FishingEventData)
            {
                FishCount++;
                Console.WriteLine("{0}：钓到一条[{2}]，已经钓到{1}条鱼了！", Name, FishCount, ((FishingEventData)eventData).FishType);
            }
            else
            {
                //其他事件处理
            }
        }

        



    }
}
