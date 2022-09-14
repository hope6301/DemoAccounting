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

        public LietDemoMoneyTable SelectAll()
        {
            SqlConnection conn = new SqlConnection(sqlstring);
            conn.Open();

            string sql = string.Format(@"select * from [dbo].[DemoMoneyTable]");

            SqlCommand cmd = new SqlCommand(sql, conn);

            //取得SQL資料
            //SqlDataReader dr = cmd.ExecuteReader();
            var reader = cmd.ExecuteReader();
            DataTable schemaTable = reader.GetSchemaTable();

            LietDemoMoneyTable aaa = new LietDemoMoneyTable();
            List<DemoMoneyTable> gg = new List<DemoMoneyTable>();
            int ww = 0;

            

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                   int rr = (int)reader[0];
                   string tt = reader[2].ToString();
                    ww = ww + 1;
                    gg.Add(new DemoMoneyTable() { ID = Convert.ToInt32(reader["ID"]), money = Convert.ToInt32(reader["money"]) });
                    //aaa.listdemoMoneyTables = new List<DemoMoneyTable>()
                    //{
                        //new DemoMoneyTable(){ID=Convert.ToInt32(reader["ID"]),money=Convert.ToInt32(reader["money"])}
                    //};
                }
            }

            int aa = ww;

            var model1 = new LietDemoMoneyTable();
            model1.listdemoMoneyTables = gg;
            //foreach (DataRow row in schemaTable.Rows)
            {
                
                //foreach (DataColumn column in schemaTable.Columns)
                {
                    //Console.WriteLine(column);
                }
            }


            cmd.Cancel();
            conn.Close();
            conn.Dispose();

            return model1;
        }

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
        public DemoMoneyTable Select(int id)
        {
            SqlConnection conn = new SqlConnection(sqlstring);
            conn.Open();

            string sql = string.Format(@"select * from [dbo].[DemoMoneyTable] WHERE ID={0}", id);

            SqlCommand cmd = new SqlCommand(sql, conn);

            //取得SQL資料
            SqlDataReader dr = cmd.ExecuteReader();

            DemoMoneyTable demomoneytable = new DemoMoneyTable();

            if (dr.Read())
            {
                demomoneytable.ID = Convert.ToInt32(dr["ID"]);
                demomoneytable.date = Convert.ToDateTime(dr["date"]);
                demomoneytable.money = Convert.ToInt32(dr["ID"]);
                demomoneytable.category = dr["category"].ToString();
                demomoneytable.remark = dr["remark"].ToString();
                demomoneytable.InAndOut = dr["InAndOut"].ToString();
            }

            cmd.Cancel();
            conn.Close();
            conn.Dispose();

            return demomoneytable;
        }

        public void Create(DemoMoneyTable demomoneytable)
        {
            int ID = demomoneytable.ID;
            DateTime date = demomoneytable.date;
            string category = demomoneytable.category;
            int money = demomoneytable.money;
            string remark = demomoneytable.remark;
            string InAndOut = demomoneytable.InAndOut;
            string DeleteOrNot = "N";

            SqlConnection conn = new SqlConnection(sqlstring);
            conn.Open();

            string sql = string.Format(@"INSERT INTO [dbo].[DemoMoneyTable]
                                            ([ID],[date],[category],[money],[remark],[InAndOut],[DeleteOrNot])
                                            VALUES({0},'{1}','{2}',{3},'{4}','{5}','{6}')", ID, date.ToString("yyyy/MM/dd"), category, money, remark, InAndOut, DeleteOrNot);

            SqlCommand cmd = new SqlCommand(sql, conn);

            //取得SQL資料
            //SqlDataReader dr = cmd.ExecuteReader();
            var aa = cmd.ExecuteNonQuery();

            ///目前沒用到
            ///
            //DemoMoneyTable xx = new DemoMoneyTable();

            //while (dr.Read())
            //{
            //    xx.ID = Convert.ToInt32(dr["ID"]);
            //    xx.date = Convert.ToDateTime(dr["date"]);
            //    xx.money = Convert.ToInt32(dr["ID"]);
            //    xx.category = dr["category"].ToString();
            //    xx.remark = dr["remark"].ToString();
            //    xx.InAndOut = dr["InAndOut"].ToString();
            //}

            cmd.Cancel();
            conn.Close();
            conn.Dispose();

        }

        ///public List<DemoMoneyTable> Dbqq(int id)

        public int Delete(int id)
        {
            string sql = string.Format(@"DELETE FROM [dbo].[DemoMoneyTable]
                                         WHERE ID={0}", id);
            int result = NonQuery(sql);
            return result;
        }
    }
}
