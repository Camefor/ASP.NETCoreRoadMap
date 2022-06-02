using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedLockSample.Contract
{
    public interface IContributionService
    {
        Task AddContributionWithoutDLM(int value);


        Task AddContributionWithDLM(int value);
    }
}
