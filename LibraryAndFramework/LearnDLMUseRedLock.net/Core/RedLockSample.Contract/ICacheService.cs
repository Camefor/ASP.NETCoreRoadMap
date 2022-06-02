using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLockSample.Contract
{
    public interface ICacheService
    {
        Task<LockProcessResult> DoActionWithLockAsync(string lockKey, Func<Task> processor);

        Task<LockProcessResult<TInput>> DoActionWithLockAsync<TInput>(string lockKey, TInput parameter, Func<TInput, Task> processor);

        Task<TEntity> GetAsync<TEntity>(string key);
        Task SetAsync<TEntity>(string key, TEntity value);
    }
}
