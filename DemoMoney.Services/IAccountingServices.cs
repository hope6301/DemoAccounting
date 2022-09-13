﻿using DemoMoney.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DemoMoney.Services
{
    public interface IAccountingServices
    {
        ServiceResult<bool> Create(DemoMoneyTable demomoneytable);
        ServiceResult<bool> Edit(DemoMoneyTable demomoneytable);
        DemoMoneyTable Select(int id);
        ServiceResult<bool> Delete(int id);
        ServiceResult<bool> UpFile(HttpPostedFileBase demomoneytable);
    }
}
