using Dapper;
using DemoMoney.Models.Models;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Formula.PTG;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMoney.DAOs
{
    public class SqlDAOs
    {
        // ADO.NET 執行 用法
        // ExecuteNonQuery( ) 
        // 主要用來執行INSERT、UPDATE、DELETE和其他沒有返回值得SQL命令

        //ExecuteScalar( )
        //返回結果集為：第一列的第一行。
        //經常用來執行SQL的COUNT、AVG、MIN、MAX 和 SUM 函數。
        //PS: ExecuteScalar 返回為Object類型，必須強置轉型。

        //ExecuteReader( )
        //快速的對資料庫進行查詢並得到結果。
        //返回為DataReader物件，如果在SqlCommand物件中調用，則返回SqlDataReader。
        //對SqlDataReader.Read的每次調用都會從結果集中返回一行。

        /// <summary>
        /// 連線字串
        /// </summary>
        private readonly string sqlstring = "Data Source=DESKTOP-FE43T7V;Initial Catalog=DemoMoney;Integrated Security=True";

        /// <summary>
        /// ADO.NET 執行新增修改刪除 方法
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int NonQuery(string sql)
        {
            SqlConnection conn = new SqlConnection(sqlstring);
            conn.Open();
            SqlCommand cmd = new SqlCommand(sql, conn);
            var result =  cmd.ExecuteNonQuery();
            cmd.Cancel();
            conn.Close();
            conn.Dispose();

            return (int)result;
        }

        /// <summary>
        /// 查詢全部未刪除資料
        /// </summary>
        /// <returns></returns>
        public LietDemoMoneyTable SelectAll()
        {
            SqlConnection conn = new SqlConnection(sqlstring);
            conn.Open();

            string sql = string.Format(@"select * from [dbo].[DemoMoneyTable] WHERE DeleteOrNot = 'N'");

            SqlCommand cmd = new SqlCommand(sql, conn);

            //取得SQL資料
            //SqlDataReader dr = cmd.ExecuteReader();
            var reader = cmd.ExecuteReader();

            List<DemoMoneyTable> listdemomodel = new List<DemoMoneyTable>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    listdemomodel.Add(new DemoMoneyTable() { ID = Convert.ToInt32(reader["ID"]), 
                                                             date = Convert.ToDateTime(reader["date"]),
                                                             money = Convert.ToInt32(reader["money"]),
                                                             category = reader["category"].ToString(),
                                                             remark = reader["remark"].ToString(),
                                                             InAndOut = reader["InAndOut"].ToString(),
                                                             DeleteOrNot = reader["DeleteOrNot"].ToString()
                    });
                }
            }
            var lietDemoMoneyTable = new LietDemoMoneyTable();
            lietDemoMoneyTable.listdemoMoneyTables = listdemomodel;

            cmd.Cancel();
            conn.Close();
            conn.Dispose();

            return lietDemoMoneyTable;
        }

        /// <summary>
        /// 查詢需下載資料
        /// </summary>
        /// <returns></returns>
        public List<DemoMoneyTable> QueryAllUsersData(string users)
        {
            SqlConnection conn = new SqlConnection(sqlstring);
            conn.Open();

            string sql = string.Format(@"select * from [dbo].[DemoMoneyTable] WHERE DeleteOrNot = 'N' AND users = '{0}'",users);

            SqlCommand cmd = new SqlCommand(sql, conn);

            //取得SQL資料
            //SqlDataReader dr = cmd.ExecuteReader();
            var reader = cmd.ExecuteReader();

            List<DemoMoneyTable> listdemomodel = new List<DemoMoneyTable>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    listdemomodel.Add(new DemoMoneyTable()
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        date = Convert.ToDateTime(reader["date"]),
                        money = Convert.ToInt32(reader["money"]),
                        category = reader["category"].ToString(),
                        remark = reader["remark"].ToString(),
                        InAndOut = reader["InAndOut"].ToString(),
                        DeleteOrNot = reader["DeleteOrNot"].ToString()
                    });
                }
            }
            var lietDemoMoneyTable = new LietDemoMoneyTable();
            lietDemoMoneyTable.listdemoMoneyTables = listdemomodel;

            cmd.Cancel();
            conn.Close();
            conn.Dispose();

            return listdemomodel;
        }

        /// <summary>
        /// 查詢日期範圍內資料
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public List<DemoMoneyTable> QueryDateUsersData(string users, string start, string finish)
        {
            SqlConnection conn = new SqlConnection(sqlstring);
            conn.Open();

            string sql = string.Format(@"select ID
                                                ,date
                                                ,category
                                                ,money
                                                ,remark
                                                ,InAndOut from [dbo].[DemoMoneyTable] 
                                        WHERE DeleteOrNot = 'N' 
                                        AND users = '{0}'
                                        AND date >= '{1}'
                                        AND date <= '{2}'", users,start,finish);

            SqlCommand cmd = new SqlCommand(sql, conn);

            //取得SQL資料
            //SqlDataReader dr = cmd.ExecuteReader();
            var reader = cmd.ExecuteReader();

            List<DemoMoneyTable> listdemomodel = new List<DemoMoneyTable>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    listdemomodel.Add(new DemoMoneyTable()
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        date = Convert.ToDateTime(reader["date"]),
                        money = Convert.ToInt32(reader["money"]),
                        category = reader["category"].ToString(),
                        remark = reader["remark"].ToString(),
                        InAndOut = reader["InAndOut"].ToString(),
                    });
                }
            }
            var lietDemoMoneyTable = new LietDemoMoneyTable();

            //lietDemoMoneyTable.listdemoMoneyTables = listdemomodel;

            cmd.Cancel();
            conn.Close();
            conn.Dispose();

            return listdemomodel;
        }









        /// <summary>
        /// 查詢目前全部資料有多少筆
        /// </summary>
        /// <returns></returns>
        public int SelectAllLength()
        {
            //Dapper 查詢寫法
            var sql = @"select COUNT(*) from [dbo].[DemoMoneyTable]";

            using (var conn = new SqlConnection(sqlstring))
            {
                var result = conn.QueryFirstOrDefault<int>(sql);
                return result;
            }
        }

        /// <summary>
        /// 查詢最後一筆ID為多少
        /// </summary>
        /// <param name="idalllength"></param>
        /// <returns></returns>
        public int SelectEndId(int idalllength)
        {

            string sql = string.Format(@" select *
                                         from dbo.DemoMoneyTable
                                         order by ID
                                         offset {0}-1 row                                 
                                         fetch next 1 rows only", idalllength);

            using (var conn = new SqlConnection(sqlstring))
            {
                var result = conn.QueryFirstOrDefault<int>(sql);
                return (int)result;
            }


            //SqlConnection conn = new SqlConnection(sqlstring);
            //conn.Open();

            //string sql = string.Format(@" select *
            //                             from dbo.DemoMoneyTable
            //                             order by ID
            //                             offset {0}-1 row                                 
            //                             fetch next 1 rows only",idalllength);

            //SqlCommand cmd = new SqlCommand(sql, conn);

            ////取得SQL資料
            ////SqlDataReader dr = cmd.ExecuteReader();
            //var endidvalue = cmd.ExecuteScalar();
            //if(endidvalue == null)
            //{
            //    endidvalue = 0;
            //}

            //cmd.Cancel();
            //conn.Close();
            //conn.Dispose();
        }

        /// <summary>
        /// 透過 ID 查詢資料庫資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DemoMoneyTable SelectID(int id)
        {
            //SqlConnection conn = new SqlConnection(sqlstring);
            //conn.Open();

            //string sql = string.Format(@"select * from [dbo].[DemoMoneyTable] WHERE (ID={0} and DeleteOrNot = 'N')", id);

            //SqlCommand cmd = new SqlCommand(sql, conn);

            ////取得SQL資料
            //SqlDataReader dr = cmd.ExecuteReader();

            //DemoMoneyTable demomoneytable = new DemoMoneyTable();

            //if (dr.Read())
            //{
            //    demomoneytable.ID = Convert.ToInt32(dr["ID"]);
            //    demomoneytable.date = Convert.ToDateTime(dr["date"]);
            //    demomoneytable.money = Convert.ToInt32(dr["money"]);
            //    demomoneytable.category = dr["category"].ToString();
            //    demomoneytable.remark = dr["remark"].ToString();
            //    demomoneytable.InAndOut = dr["InAndOut"].ToString();
            //    demomoneytable.DeleteOrNot = dr["DeleteOrNot"].ToString();
            //}

            //cmd.Cancel();
            //conn.Close();
            //conn.Dispose();

            string sql = string.Format(@"select * from [dbo].[DemoMoneyTable] WHERE (ID={0} and DeleteOrNot = 'N')", id);

            using (var conn = new SqlConnection(sqlstring))
            {
                var demomoneytable = conn.QueryFirstOrDefault<DemoMoneyTable>(sql);
                return demomoneytable;
            }
        }

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="demomoneytable"></param>
        public int Create(DemoMoneyTable MoneyTable)
        {
            var sql = @"INSERT INTO [dbo].[DemoMoneyTable]
                        (
                            [ID],
                            [date],
                            [category],
                            [money],
                            [remark],
                            [InAndOut],
                            [DeleteOrNot],
                            [users]
                        )
                        VALUES
                        (
                            @id,
                            @DATE,
                            @CATEGORY,
                            @MONEY,
                            @remark,
                            @InAndOut,
                            @DeleteOrNot,
                            @users
                        )";

            using (var conn = new SqlConnection(sqlstring))
            {
                var result = conn.Execute(sql, MoneyTable);
                return result;
            }
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            string sql = string.Format(@"UPDATE [dbo].[DemoMoneyTable]
                                            SET [DeleteOrNot] = 'Y'
                                            WHERE ID = {0}", id);

            using (var conn = new SqlConnection(sqlstring))
            {
                var result = conn.Execute(sql);
                return result;
            }
        }
        
        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="demomoneytable"></param>
        /// <returns></returns>
        public int Edit(DemoMoneyTable demomoneytable)
        {
            var sql = @"UPDATE [dbo].[DemoMoneyTable]
                                           SET [ID] = @id
                                              ,[date] = @date
                                              ,[category] = @category
                                              ,[money] = @money
                                              ,[remark] = @remark
                                              ,[InAndOut] = @InAndOut
                                              ,[DeleteOrNot] = @DeleteOrNot
                                         WHERE ID = @id";

            using (var conn = new SqlConnection(sqlstring))
            {
                var result = conn.Execute(sql, demomoneytable);
                return result;
            }
        }

    }
}
