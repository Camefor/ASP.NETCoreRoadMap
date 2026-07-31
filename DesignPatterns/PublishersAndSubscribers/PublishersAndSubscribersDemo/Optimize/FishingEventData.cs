using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishersAndSubscribersDemo.Optimize
{

    /// <summary>
    ///   “钓鱼” 这个事件
    /// </summary>
    public class FishingEventData : EventData
    {
        public FishTypeEnum FishType { get; set; }

        public FishingMan FishingMan { get; set; }
        
    }
}
