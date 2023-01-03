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

            //判斷用戶是否存在
            // true:用戶存在
            // false:用戶不存在
            if (usersDaos.UsersExist(uersmodel.Account))
            {
                result.Status = ServiceStatus.Failure;
                result.Result = false;
                result.Message = "帳號已被註冊";
                return result;
            }
            else
            {
                //以加密的密碼回傳給model，儲存進SQL
                uersmodel.Password = EncryptAndDecode.Base64Encrypt(uersmodel.Password);
                
                if (usersDaos.CreateUsers(uersmodel))
                {
                    //加密帳號，傳回前台給cookie使用
                    uersmodel.Account = EncryptAndDecode.Base64Encrypt(uersmodel.Account);

                    result.Status = ServiceStatus.Success;
                    result.Result = true;
                    result.Message = "註冊成功";
                    return result;
                }
                else
                {
                    result.Status = ServiceStatus.Failure;
                    result.Result = false;
                    result.Message = "註冊發生不明問題，請洽管理員";
                    return result;
                }
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

            //確認使用者是否註冊過
            if (usersDaos.UsersExist(acc))
            {
                //有註冊過：確認密碼是否正確

                //取回加密的密碼
                string EncryptPassword = usersDaos.RetrievePassword(acc);
                string DecryptPassword = EncryptAndDecode.Base64Decrypt(EncryptPassword);

                //密碼正確：登入成功
                if (uersmodel.Password == DecryptPassword)
                {
                    uersmodel.Account = EncryptAndDecode.Base64Encrypt(acc);
                    uersmodel.Password = EncryptPassword;

                    result.Status = ServiceStatus.Success;
                    result.Result = true;
                    result.Message = "登入成功";

                    return result;
                }
                //密碼錯誤：顯示錯誤
                else
                {
                    result.Status = ServiceStatus.Failure;
                    result.Result = false;
                    result.Message = "密碼不正確";
                    return result;
                }
            }
            else
            {
                //沒註冊過：跳去註冊頁
                result.Status = ServiceStatus.NotFound;
                result.Result = false;
                result.Message = "用戶不存在請註冊";
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

            userstable = usersDaos.QueryUserInformation(EncryptAndDecode.Base64Decrypt(Account));

            return userstable;
        }

        /// <summary>
        /// 修改使用者資訊
        /// </summary>
        /// <param name="usersTable"></param>
        /// <returns></returns>
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
        /// 查詢密碼是否正確
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="Password">密碼</param>
        /// <returns></returns>
        public bool QueryPassword(string Account, string Password)
        {
            UsersDAOs usersDAOs = new UsersDAOs();
            string DecodeAccount = EncryptAndDecode.Base64Decrypt(Account);
            string EncryptPassword = EncryptAndDecode.Base64Encrypt(Password);

            if(usersDAOs.QueryPassword(DecodeAccount, EncryptPassword))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 修改密碼
        /// </summary>
        /// <param name="Account">帳號</param>
        /// <param name="NewPassword">新密碼</param>
        /// <returns></returns>
        public ServiceResult<bool> EditPassword(string Account, string NewPassword)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();
            UsersDAOs usersDAOs = new UsersDAOs();

            bool EditYN = usersDAOs.EditPassword(EncryptAndDecode.Base64Decrypt(Account), EncryptAndDecode.Base64Encrypt(NewPassword));

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
