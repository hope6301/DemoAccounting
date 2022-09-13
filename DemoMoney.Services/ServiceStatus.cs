using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMoney.Services
{
    public enum ServiceStatus
    {
        Failure = 0,
        Success = 1,
        Duplicate = 2,
        NotFound = 3,
        MultipleAction = 4,
    }
}
