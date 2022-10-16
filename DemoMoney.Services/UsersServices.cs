using DemoMoney.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoMoney.DAOs;

namespace DemoMoney.Services
{
    public class UsersServices : IUsersServices
    {
        /// <summary>
        /// 新增使用者
        /// </summary>
        /// <param name="uersmodel"></param>
        /// <returns></returns>
        public ServiceResult<bool> CreateUsers(UsersTableModel uersmodel)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();
            UsersDAOs usersDaos = new UsersDAOs();
            if (usersDaos.CreateUsers(uersmodel))
            {
                result.Status = ServiceStatus.Success;
                result.Result = true;
                result.Message = "註冊成功";
                return result;
            }
            else
            {
                result.Status = ServiceStatus.Failure;
                result.Result = false;
                result.Message = "帳號已被註冊";
                return result;
            }
        }

        /// <summary>
        /// 登入使用者是否正確
        /// </summary>
        /// <param name="uersmodel">使用者資訊</param>
        /// <returns></returns>
        public ServiceResult<bool> UsersLogin(UsersTableModel uersmodel)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();
            UsersDAOs usersDaos = new UsersDAOs();

            string acc = uersmodel.Account;
            string psw = uersmodel.Password;

            switch (usersDaos.UsersExist(acc, psw))
            {
                //用戶登入成功
                case 1: 
                    {
                        result.Status = ServiceStatus.Success;
                        result.Result = true;
                        result.Message = "登入成功";
                        return result;
                    }
                //用戶存在，密碼不對
                case 0:
                    {
                        result.Status = ServiceStatus.Failure;
                        result.Result = false;
                        result.Message = "密碼不正確";
                        return result;
                    }
                //用戶不存在，請註冊
                case 3:
                    {
                        result.Status = ServiceStatus.NotFound;
                        result.Result = false;
                        result.Message = "用戶不存在請註冊";
                        return result;
                    }
                default:
                    result.Status = ServiceStatus.Failure;
                    result.Result = false;
                    result.Message = "不知名錯誤";
                    return result;
            }

        }

        /// <summary>
        /// 查詢使用者資訊
        /// </summary>
        /// <param name="Account">帳號名稱</param>
        /// <returns></returns>
        public UsersTableModel UserInformation(string Account)
        {
            UsersTableModel userstable = new UsersTableModel();
            UsersDAOs usersDaos = new UsersDAOs();

            userstable = usersDaos.QueryUserInformation(Account);



            return userstable;
        }

        public ServiceResult<bool> EditUser(UsersTableModel usersTable)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();
            UsersDAOs usersDaos = new UsersDAOs();

            bool editresult = usersDaos.EditUsers(usersTable);

            if (editresult)
            {

                result.Status = ServiceStatus.Success;
                result.Result = true;
                result.Message = "修改成功";
                return result;
            }
            else
            {
                result.Status = ServiceStatus.Failure;
                result.Result = false;
                result.Message = "修改失敗";
                return result;
            }
        }
        /// <summary>
        /// 修改密碼
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ServiceResult<bool> EditPassword(string account, string NewPassword)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();
            UsersDAOs usersDAOs = new UsersDAOs();

            bool EditYN = usersDAOs.EditPassword(account, NewPassword);

            if (EditYN)
            {
                result.Status = ServiceStatus.Success;
                result.Result = true;
                result.Message = "密碼修改成功";
                return result;
            }
            else
            {
                result.Status = ServiceStatus.Failure;
                result.Result = false;
                result.Message = "密碼修改失敗+請洽管理員";
                return result;
            }
        }
    }
}
