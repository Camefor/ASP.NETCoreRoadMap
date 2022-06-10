using PublishersAndSubscribersDemo.Optimize;
using PublishersAndSubscribersDemo.Optimize2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public FishingRod()
        {

            try
            {
                //统一注册事件
                Assembly assembly = Assembly.GetExecutingAssembly();
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IEventHandler).IsAssignableFrom(type))//判断当前类型是否实现了IEventHandler接口
                    {
                        if (type.Name == typeof(FishingEventHandler).Name)
                        {
                            Type handlerInterface = type.GetInterface(type.Name);//获取该类实现的泛型接口
                            if (handlerInterface == null)
                            {
                                handlerInterface = typeof(IEventHandler<FishingEventData>);
                            }
                            Type eventDataType = handlerInterface.GetGenericArguments()[0]; // 获取泛型接口指定的参数类型

                            //如果参数类型是FishingEventData，则说明事件源匹配
                            if (eventDataType.Equals(typeof(FishingEventData)))
                            {
                                //创建实例
                                var handler = Activator.CreateInstance(type) as IEventHandler<FishingEventData>;
                                //注册事件
                                FishingEvent += handler.HandleEvent;
                            }
                        }

                    }
                }

            }
            catch (Exception)
            {
            }
        }

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
                        FishingMan = man,
                        EventSource = this
                    });
            }
        }
    }
}
