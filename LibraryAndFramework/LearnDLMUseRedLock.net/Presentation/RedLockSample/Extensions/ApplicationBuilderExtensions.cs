using RedLockSample.Caching.Redis;

namespace RedLockSample.Extensions
{
    public static class ApplicationBuilderExtensions
    {

        /// <summary>
        /// 创建一个扩展，旨在将 LockFactory 对象注册到应用程序的处置列表中
        /// （创建的静态 RedLockFactory 应在应用程序停止时处置（Redis 的打开连接并释放获取的锁））。
        /// </summary>
        /// <param name="lifeTime"></param>
        public static void DisposeLockFactory(this IHostApplicationLifetime lifeTime)
        {
            lifeTime.ApplicationStopping.Register(() =>
            {
                RedLockProvider.RedLockFactoryObject.Dispose();
            });
        }
    }
}
