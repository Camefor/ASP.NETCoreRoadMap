﻿using PublishersAndSubscribersDemo.Optimize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishersAndSubscribersDemo.Optimize2
{
    /// <summary>
    /// 钓鱼事件处理程序
    /// </summary>
    public class FishingEventHandler : IEventHandler<FishingEventData>
    {
        public void HandleEvent(FishingEventData eventData)
        {
            eventData.FishingMan.FishCount++;

            Console.WriteLine("{0}：钓到一条[{2}]，已经钓到{1}条鱼了！",
                eventData.FishingMan.Name, eventData.FishingMan.FishCount, eventData.FishType);

        }
    }
}
