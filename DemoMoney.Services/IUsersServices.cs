using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoMoney.Models.Models;
using DemoMoney.DAOs;

namespace DemoMoney.Services
{
    public interface IUsersServices
    {
        /// <summary>
        /// 新增使用者
        /// </summary>
        /// <param name="uersmodel"></param>
        /// <returns></returns>
        ServiceResult<bool> CreateUsers(UsersTableModel uersmodel);

        ServiceResult<bool> UsersLogin(UsersTableModel uersmodel);

        UsersTableModel UserInformation(string Account);

        ServiceResult<bool> EditUser(UsersTableModel usersTable);

        ServiceResult<bool> EditPassword(string account,string NewPassword);

    }


}
