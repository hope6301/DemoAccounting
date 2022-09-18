using DemoMoney.Models.Models;
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
        // ExecuteNonQuery( ) 
        // 主要用來執行INSERT、UPDATE、DELETE和其他沒有返回值得SQL命令。

        //ExecuteScalar( )
        //返回結果集為：第一列的第一行。
        //經常用來執行SQL的COUNT、AVG、MIN、MAX 和 SUM 函數。
        //PS: ExecuteScalar 返回為Object類型，必須強置轉型。

        //ExecuteReader( )
        //快速的對資料庫進行查詢並得到結果。
        //返回為DataReader物件，如果在SqlCommand物件中調用，則返回SqlDataReader。
        //對SqlDataReader.Read的每次調用都會從結果集中返回一行。

        string sqlstring = "Data Source=DESKTOP-FE43T7V;Initial Catalog=DemoMoney;Integrated Security=True";

        /// <summary>
        /// 執行新增修改刪除 方法
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

            //cmd.Cancel();
            //conn.Close();
            //conn.Dispose();

            return lietDemoMoneyTable;
        }

        /// <summary>
        /// 查詢目前全部資料有多少筆
        /// </summary>
        /// <returns></returns>
        public int SelectAllLength()
        {
            SqlConnection conn = new SqlConnection(sqlstring);
            conn.Open();

            string sql = string.Format(@"select COUNT(*) from [dbo].[DemoMoneyTable]");

            SqlCommand cmd = new SqlCommand(sql, conn);

            //取得SQL資料
            //SqlDataReader dr = cmd.ExecuteReader();
            var SqlAllLength = cmd.ExecuteScalar();
            
            cmd.Cancel();
            conn.Close();
            conn.Dispose();

            return (int)SqlAllLength;
        }

        /// <summary>
        /// 查詢最後一筆ID為多少
        /// </summary>
        /// <param name="idalllength"></param>
        /// <returns></returns>
        public int SelectEndId(int idalllength)
        {
            SqlConnection conn = new SqlConnection(sqlstring);
            conn.Open();

            string sql = string.Format(@" select *
                                         from dbo.DemoMoneyTable
                                         order by ID
                                         offset {0}-1 row                                 
                                         fetch next 1 rows only",idalllength);

            SqlCommand cmd = new SqlCommand(sql, conn);

            //取得SQL資料
            //SqlDataReader dr = cmd.ExecuteReader();
            var endidvalue = cmd.ExecuteScalar();
            if(endidvalue == null)
            {
                endidvalue = 0;
            }

            cmd.Cancel();
            conn.Close();
            conn.Dispose();

            return (int)endidvalue;
        }

        /// <summary>
        /// 透過 ID 查詢資料庫資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DemoMoneyTable SelectID(int id)
        {
            SqlConnection conn = new SqlConnection(sqlstring);
            conn.Open();

            string sql = string.Format(@"select * from [dbo].[DemoMoneyTable] WHERE (ID={0} and DeleteOrNot = 'N')", id);

            SqlCommand cmd = new SqlCommand(sql, conn);

            //取得SQL資料
            SqlDataReader dr = cmd.ExecuteReader();

            DemoMoneyTable demomoneytable = new DemoMoneyTable();

            if (dr.Read())
            {
                demomoneytable.ID = Convert.ToInt32(dr["ID"]);
                demomoneytable.date = Convert.ToDateTime(dr["date"]);
                demomoneytable.money = Convert.ToInt32(dr["money"]);
                demomoneytable.category = dr["category"].ToString();
                demomoneytable.remark = dr["remark"].ToString();
                demomoneytable.InAndOut = dr["InAndOut"].ToString();
                demomoneytable.DeleteOrNot = dr["DeleteOrNot"].ToString();
            }

            cmd.Cancel();
            conn.Close();
            conn.Dispose();

            return demomoneytable;
        }

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="demomoneytable"></param>
        public int Create(DemoMoneyTable demomoneytable)
        {
            int ID = demomoneytable.ID;
            DateTime date = demomoneytable.date;
            string category = demomoneytable.category;
            int money = demomoneytable.money;
            string remark = demomoneytable.remark;
            string InAndOut = demomoneytable.InAndOut;
            string DeleteOrNot = "N";

            string sql = string.Format(@"INSERT INTO [dbo].[DemoMoneyTable]
                                            ([ID],[date],[category],[money],[remark],[InAndOut],[DeleteOrNot])
                                            VALUES({0},'{1}','{2}',{3},'{4}','{5}','{6}')", ID, date.ToString("yyyy/MM/dd"), category, money, remark, InAndOut, DeleteOrNot
                                            );

            int result = NonQuery(sql);
            return (int)result;
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            //string sql = string.Format(@"DELETE FROM [dbo].[DemoMoneyTable] WHERE ID={0}", id);
            string sql = string.Format(@"UPDATE [dbo].[DemoMoneyTable]
                                            SET [DeleteOrNot] = 'Y'
                                            WHERE ID = {0}", id);

            int result = NonQuery(sql);
            return result;
        }
        
        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="demomoneytable"></param>
        /// <returns></returns>
        public int Edit(DemoMoneyTable demomoneytable)
        {
            //string sql = string.Format(@"DELETE FROM [dbo].[DemoMoneyTable] WHERE ID={0}", id);

            int ID = demomoneytable.ID;
            DateTime date = demomoneytable.date;
            string category = demomoneytable.category;
            int money = demomoneytable.money;
            string remark = demomoneytable.remark;
            string InAndOut = demomoneytable.InAndOut;
            string DeleteOrNot = demomoneytable.DeleteOrNot;

            string sql = string.Format(@"UPDATE [dbo].[DemoMoneyTable]
                                           SET [ID] = {0}
                                              ,[date] = '{1}'
                                              ,[category] = '{2}'
                                              ,[money] = {3}
                                              ,[remark] = '{4}'
                                              ,[InAndOut] = '{5}'
                                              ,[DeleteOrNot] = '{6}'
                                         WHERE ID = {0}", ID, date.ToString("yyyy/MM/dd"), category,money,remark,InAndOut,DeleteOrNot);

            int result = NonQuery(sql);
            return result;
        }

    }
}
