using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoMoney.Models.Models;

namespace DemoMoney.Services
{
    public class EncryptAndDecodeServices
    {

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string Encrypt(string str)
        {
            str = EncryptAndDecode.Base64Encrypt(str);

            return str;
        }


        /// <summary>
        /// 一次傳入帳號&密碼做加密
        /// </summary>
        /// <param name="model">傳入帳號和密碼</param>
        /// <returns></returns>
        public UsersTableModel ModelEncryp(UsersTableModel model)
        {
            if(model.Account!="" && model.Account!=null && model.Account.Length !=0)
            {
                model.Account = EncryptAndDecode.Base64Encrypt(model.Account);
            }

            if (model.Password != "" && model.Password != null && model.Password.Length != 0)
            {
                model.Password = EncryptAndDecode.Base64Encrypt(model.Password);
            }

            return model;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string Decode(string str)
        {
            str = EncryptAndDecode.Base64Decrypt(str);

            return str;
        }


        public UsersTableModel ModelDecode(UsersTableModel model)
        {
            if (model.Account != "" && model.Account != null && model.Account.Length != 0)
            {
                model.Account = EncryptAndDecode.Base64Decrypt(model.Account);
            }

            if (model.Password != "" && model.Password != null && model.Password.Length != 0)
            {
                model.Password = EncryptAndDecode.Base64Decrypt(model.Password);
            }

            return model;
        }







    }
}
