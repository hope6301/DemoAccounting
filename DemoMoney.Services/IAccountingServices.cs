using DemoMoney.Models.Models;
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
        List<DemoMoneyTable> listQueryDateSelectAll(string users,string startdatevalue ,string finishdatevalue);

        LietDemoMoneyTable SelectAll();

        DemoMoneyTable SelectID(int id);
        ServiceResult<bool> Create(DemoMoneyTable demomoneytable);
        ServiceResult<bool> Edit(DemoMoneyTable demomoneytable);
        ServiceResult<bool> Delete(int id);
        ServiceResult<bool> UpFile(HttpPostedFileBase demomoneytable,string account);

        ServiceResult<bool> DownloadAll();
    }
}
