using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMoney.Services
{
    public enum ServiceStatus
    {
        /// <summary>
        /// 失敗
        /// </summary>
        Failure = 0,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 重複
        /// </summary>
        Duplicate = 2,

        /// <summary>
        /// 未找到
        /// </summary>
        NotFound = 3,

        /// <summary>
        /// 多重動作
        /// </summary>
        MultipleAction = 4,
    }
}
