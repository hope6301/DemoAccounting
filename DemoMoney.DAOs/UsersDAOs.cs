using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoMoney.Models.Models;
using Microsoft.Exchange.WebServices.Data;

namespace DemoMoney.DAOs
{
    public class UsersDAOs
    {
        private readonly string sqlstring = "Data Source=DESKTOP-FE43T7V;Initial Catalog=DemoMoney;Integrated Security=True";

        /// <summary>
        /// 新增使用者
        /// </summary>
        /// <param name="usersmodel"></param>
        /// <returns></returns>
        public bool CreateUsers(UsersTableModel usersmodel)
        {
            //判斷用戶是否存在
            bool exist =   this.UsersExist(usersmodel.Account);

            //用戶存在傳回flase
            if (exist)
            {
                //新增用戶失敗
                return false;
            }
            //用戶不存在，新增用戶。
            else
            {
                usersmodel.DeletOrNotUser = "N";
                var InsertSql = @"INSERT INTO [dbo].[UsersTable]
                                           (
                                            [Account]
                                           ,[Password]
                                           ,[Last_name]
                                           ,[First_name]
                                           ,[DeletOrNotUser]
                                            )
                                            VALUES
                                            (
                                                @Account,
                                                @Password,
                                                @Last_name,
                                                @First_name,
                                                @DeletOrNotUser
                                            )";
                using (var conn = new SqlConnection(sqlstring))
                {
                    var result =  conn.Execute(InsertSql, usersmodel);
                }
                //新增用戶成功
                return true;
            }
        }

        /// <summary>
        /// 查詢使用者全部資訊
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public UsersTableModel QueryUserInformation(string account)
        {
            var sql = @"select * from dbo.userstable where [Account] = @account";

            using (var conn = new SqlConnection(sqlstring))
            {
                var result = conn.QueryFirstOrDefault<UsersTableModel>(sql, new { account = account });
                return result;
            }
        }

        /// <summary>
        /// 判斷用戶是否存在
        /// </summary>
        /// <param name="account">帳號</param>
        /// <returns></returns>
        public bool UsersExist(string account)
        {
            var sql = @"select top 1 1 from dbo.userstable where [Account] = @account";

            using (var conn = new SqlConnection(sqlstring))
            {
                var result = conn.QueryFirstOrDefault<int>(sql,new { account = account });
                //用戶是否存在
                if (result > 0)
                {
                    //用戶存在傳回 true
                    return true;
                }
                else
                {
                    //用戶不存在傳回 false
                    return false;
                }
            }
        }

        /// <summary>
        /// 判斷用戶是否存在，並帳號密碼是否正確
        /// </summary>
        /// <param name="account">帳號</param>
        /// <param name="password">密碼</param>
        /// <returns></returns>
        public int UsersExist(string account,string password)
        {
            var sqlExist = @"select top 1 1 from dbo.userstable where [Account] = @account AND [Password] = @password";

            //判斷帳戶密碼 是否都正常
            //是：登入成功
            //否：請下一步判斷帳號使否註冊過
            
            //判斷帳號是否註冊過
            //是：密碼錯誤
            //否：請註冊帳號

            using (var conn = new SqlConnection(sqlstring))
            {

                var result = conn.QueryFirstOrDefault<int>(sqlExist,new { account= account, password = password });

                //登入是否成功
                if (result>0)
                {
                    ////登入成功
                    return 1;
                }
                //登入出錯
                else
                {
                    //判斷帳號是否存在
                    bool rxist = this.UsersExist(account);

                    //用戶存在
                    if (rxist)
                    {
                        return 0;
                    }
                    //用戶不存在
                    else
                    {
                        return 3;
                    }
                }
            }
        }

        /// <summary>
        /// 修改會員資料
        /// </summary>
        /// <param name="usersTable">需修改的會員資料</param>
        /// <returns></returns>
        public bool EditUsers(UsersTableModel usersTable)
        {
            var sql = @"UPDATE [dbo].UsersTable
                                           SET [Account] = @Account
                                              ,[Last_name] = @Last_name
                                              ,[First_name] = @First_name
                                         WHERE Account = @Account";

            using (var conn = new SqlConnection(sqlstring))
            {
                var result = conn.Execute(sql, usersTable);
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public bool EditPassword(string account,string newpassword)
        {
            var sql = @"UPDATE [dbo].UsersTable
                                           SET [Password] = @Password
                                         WHERE Account = @Account";
            
            using (var conn = new SqlConnection(sqlstring))
            {
                var result = conn.Execute(sql, new { Account = account, Password = newpassword });
                //密碼是否修改成功
                if (result > 0)
                {
                    //密碼修改成功 true
                    return true;
                }
                else
                {
                    //密碼修改失敗 false
                    return false;
                }
            }
        }



    }

}
