using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishersAndSubscribersDemo.Optimize
{

    /// <summary>
    /// 事件源：描述事件信息，用于参数传递
    /// </summary>
    public class EventData : IEventData
    {
        public EventData()
        {
            EventTime = DateTime.Now;
        }
        public DateTime EventTime { get; set; }

        public object EventSource { get; set; }
    }
}
