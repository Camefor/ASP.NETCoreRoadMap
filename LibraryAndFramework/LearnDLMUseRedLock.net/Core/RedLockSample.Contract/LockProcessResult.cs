using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLockSample.Contract
{
    public class LockProcessResult
    {

        public void SetException(Exception ex)
        {
            this.Exception = ex;
        }

        public bool IsSucessfullProcessd => Exception == null;
        public Exception Exception { get; set; }
    }

    public class LockProcessResult<TInput> : LockProcessResult
    {

    }
}
