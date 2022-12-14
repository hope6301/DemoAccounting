using DemoMoney.DAOs;
using DemoMoney.Models.Models;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using OfficeOpenXml;
using Org.BouncyCastle.Asn1.Ocsp;
using Grpc.Core;

namespace DemoMoney.Services
{
    public class AccountingServices : IAccountingServices
    {
        /// <summary>
        /// 查詢全部未刪除資料
        /// </summary>
        /// <returns></returns>
        public LietDemoMoneyTable SelectAll()
        {
            SqlDAOs dao = new SqlDAOs();
            LietDemoMoneyTable reader = dao.SelectAll();

            return reader;
        }



        /// <summary>
        /// 查詢指定ID資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DemoMoneyTable SelectID(int id)
        {
            DemoMoneyTable demomoneytable = new DemoMoneyTable();
            SqlDAOs dao = new SqlDAOs();
            demomoneytable = dao.SelectID(id);
            return demomoneytable;
        }

        /// <summary>
        /// 新增資料
        /// </summary>
        /// <param name="demomoneytable"></param>
        /// <returns></returns>
        public ServiceResult<bool> Create(DemoMoneyTable MoneyTable)
        {
            //新增資料的另一種寫法
            //DemoEntities2 content = new DemoEntities2();
            //System.Data.Entity.DbSet<accounting1> result1 = content.accounting1;
            //result1.Add(demomoneytable);
            //content.SaveChanges();

            ServiceResult<bool> batchResult = new ServiceResult<bool> { Status = ServiceStatus.Success, Result = true };
            SqlDAOs dao = new SqlDAOs();

            int SqlLength = dao.SelectAllLength();

            if (SqlLength<1)
            {
                SqlLength = 1;
            }

            var EndIdValue = dao.SelectEndId(SqlLength);

            MoneyTable.ID = EndIdValue + 1;
            MoneyTable.DeleteOrNot = "N";
            MoneyTable.users = EncryptAndDecode.Base64Decrypt(MoneyTable.users);


            int result= dao.Create(MoneyTable);

            if(result > 0)
            {
                batchResult.Status = ServiceStatus.Success;
                batchResult.Result = true;
                batchResult.Message = "資料新增成功";
            }
            else
            {
                batchResult.Status = ServiceStatus.NotFound;
                batchResult.Result = false;
                batchResult.Message = "新增資料失敗";
            }
            return batchResult;
        }

        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="demomoneytable"></param>
        /// <returns></returns>
        public ServiceResult<bool> Edit(DemoMoneyTable demomoneytable)
        {
            //修改資料的另一種寫法
            //DemoMoneyEntities content = new DemoMoneyEntities();
            //content.DemoMoneyTable.Attach(demomoneytable);
            //content.Entry(demomoneytable).State = System.Data.Entity.EntityState.Modified;
            //content.SaveChanges();

            ServiceResult<bool> batchResult = new ServiceResult<bool>();

            SqlDAOs dao = new SqlDAOs();
            int result =  dao.Edit(demomoneytable);
            if (result > 0)
            {
                batchResult.Status = ServiceStatus.Success;
                batchResult.Result = true;
                batchResult.Message = "資料修改成功";
            }
            else
            {
                batchResult.Status = ServiceStatus.Success;
                batchResult.Result = true;
                batchResult.Message = "修改失敗";
            }
            return batchResult;
        }

        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceResult<bool> Delete(int id)
        {
            ServiceResult<bool> batchResult = new ServiceResult<bool>();
            SqlDAOs dao = new SqlDAOs();
            int result = dao.Delete(id);

            if (result > 0)
            {
                batchResult.Status = ServiceStatus.Success;
                batchResult.Result = true;
                batchResult.Message = "刪除成功";
            }
            else
            {
                batchResult.Status = ServiceStatus.NotFound;
                batchResult.Result = false;
                batchResult.Message = "沒有找到刪除資料";
            }

            return batchResult;
        }

        /// <summary>
        /// 檔案上傳
        /// </summary>
        /// <param name="file">檔案</param>
        /// <param name="account">帳號資訊</param>
        /// <returns></returns>
        public ServiceResult<bool> UpFile(HttpPostedFileBase file , string account)
        {
            ServiceResult<bool> batchResult = new ServiceResult<bool>();
            if (file != null && account != null)
            {
                //先註解，可以先把帳號傳進來後先解密，就不用每次新增一次解密一次。
                //string DecryptAccount = EncryptAndDecode.Base64Decrypt(account);

                Stream stream = file.InputStream;
                DataTable datatable = new DataTable();
                IWorkbook wb;
                ISheet sheet;
                IRow headerRow;
                int cellCount;

                try
                {
                    //依excel版本，NPOI載入檔案
                    if (file.FileName.ToUpper().EndsWith("XLSX"))
                        wb = new XSSFWorkbook(stream); // excel版本(.xlsx)
                    else
                        wb = new HSSFWorkbook(stream); // excel版本(.xls)

                    //取第一個頁籤   
                    sheet = wb.GetSheetAt(0);

                    //取第一個頁籤的第一列
                    headerRow = sheet.GetRow(0);

                    //計算出第一列共有多少欄位
                    cellCount = headerRow.LastCellNum;

                    //迴圈執行第一列的第一個欄位到最後一個欄位，將抓到的值塞進DataTable做完欄位名稱
                    for (int i = headerRow.FirstCellNum; i < cellCount; i++)
                    {
                        datatable.Columns.Add(new DataColumn(headerRow.GetCell(i).StringCellValue));
                    }

                    //int j; //計算每一列讀到第幾個欄位
                    int column = 1; //計算每一列讀到第幾個欄位

                    // 略過第零列(標題列)，一直處理至最後一列
                    for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                    {
                        //取目前的列(row)
                        IRow row = sheet.GetRow(i);

                        //若該列的第一個欄位無資料，break跳出
                        if (string.IsNullOrEmpty(row.Cells[0].ToString().Trim()))
                        {
                            break;
                        }
                        //宣告DataRow
                        DataRow dataRow = datatable.NewRow();
                        //宣告ICell
                        ICell cell;

                        try
                        {
                            //依先前取得，依每一列的欄位數，逐一設定欄位內容
                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                //計算每一列讀到第幾個欄位(秀在錯誤訊息上)
                                column = j + 1;

                                //設定cell為目前第j欄位
                                cell = row.GetCell(j);

                                if (cell != null) //若cell有值
                                {
                                    //用cell.CellType判斷資料的型別
                                    //再依照欄位屬性，用StringCellValue、DateCellValue、NumericCellValue、DateCellValue取值
                                    switch (cell.CellType)
                                    {
                                        //字串型態欄位
                                        case NPOI.SS.UserModel.CellType.String:
                                            //設定dataRow第j欄位的值，cell以字串型態取值
                                            dataRow[j] = cell.StringCellValue;
                                            break;

                                        //數字型態欄位
                                        case NPOI.SS.UserModel.CellType.Numeric:

                                            if (HSSFDateUtil.IsCellDateFormatted(cell)) //日期格式
                                            {
                                                //設定dataRow第j欄位的值，cell以日期格式取值
                                                dataRow[j] = DateTime.FromOADate(cell.NumericCellValue).ToString("yyyy/MM/dd HH:mm");
                                            }
                                            else //非日期格式
                                            {
                                                //設定dataRow第j欄位的值，cell以數字型態取值
                                                dataRow[j] = cell.NumericCellValue;
                                            }
                                            break;

                                        //布林值
                                        case NPOI.SS.UserModel.CellType.Boolean:

                                            //設定dataRow第j欄位的值，cell以布林型態取值
                                            dataRow[j] = cell.BooleanCellValue;
                                            break;

                                        //空值
                                        case NPOI.SS.UserModel.CellType.Blank:

                                            dataRow[j] = "";
                                            break;

                                        // 預設
                                        default:

                                            dataRow[j] = cell.StringCellValue;
                                            break;
                                    }
                                }
                            }
                            //DataTable加入dataRow
                            datatable.Rows.Add(dataRow);
                        }
                        catch (Exception ex)
                        {
                            //錯誤訊息
                            throw new Exception("第 " + i + "列第" + column + "欄，資料格式有誤:\r\r" + ex.ToString());
                        }
                    }
                    //釋放資源
                    sheet = null;
                    wb = null;
                    stream.Dispose();
                    stream.Close();
                }
                catch (Exception ex)
                {
                    sheet = null;
                    wb = null;
                    stream.Dispose();
                    stream.Close();
                    //ViewBag.Message = "匯入失敗";
                    batchResult.Message = ex.Message;
                    batchResult.Result = false;
                    batchResult.Status = ServiceStatus.Failure;
                    return batchResult;
                }
                //finally
                //{
                //    //釋放資源
                //    sheet = null;
                //    wb = null;
                //    stream.Dispose();
                //    stream.Close();
                //}

                //dataTable跑回圈，insert資料至DB
                foreach (DataRow dataRow in datatable.Rows)
                {
                    DemoMoneyTable demomoneytable = new DemoMoneyTable()
                    {
                        //傳入excel的資料給 viewmodel
                        //ID = int.Parse(dataRow["ID"].ToString()),
                        date = DateTime.Parse(dataRow["date"].ToString()),
                        money = int.Parse(dataRow["money"].ToString()),
                        category = dataRow["category"].ToString(),
                        remark = dataRow["remark"].ToString(),
                        InAndOut = dataRow["InAndOut"].ToString(),
                        users = account
                    };
                    this.Create(demomoneytable);
                }

                batchResult.Status = ServiceStatus.Success;
                batchResult.Result = true;
                batchResult.Message = "上傳成功";

                return batchResult;
            }
            else 
            {
                batchResult.Message = "要有檔案才能按上船喔!";
                batchResult.Result = false;
                batchResult.Status = ServiceStatus.Failure;

                return batchResult;
            }
        }

        /// <summary>
        /// 下載EXCEL全部資料
        /// </summary>
        /// <returns></returns>
        public ServiceResult<bool> DownloadAll()
        {
            ServiceResult<bool> batchResult = new ServiceResult<bool>();
            IWorkbook workbook = new HSSFWorkbook(); //声明工作簿对象，可以创建xls或xlsx Excel文件
            ISheet sheet = workbook.CreateSheet("記帳資料"); //创建工作表

            //sheet.CreateRow(1);
            ////合併儲存格
            //sheet.AddMergedRegion(new CellRangeAddress(1, 1, 0, 6));
            //sheet.GetRow(1).CreateCell(0).SetCellValue("我是主標題");


            List<string> tt = new List<string>() { "日期", "類別", "金額", "備註", "收支" };

            sheet.CreateRow(0);
            for(int i = 0; i < tt.Count; i++)
            {
                sheet.GetRow(0).CreateCell(i).SetCellValue(tt[i]);
            }

            List<DemoMoneyTable> tables = new List<DemoMoneyTable>();
            SqlDAOs dao = new SqlDAOs();

            //先寫死
            string aa = "hope6301";
            tables = dao.QueryAllUsersData(aa);

            for(int i = 0; i < tables.Count; i++)
            {
                int t = i + 1;
                sheet.CreateRow(t);
                sheet.GetRow(t).CreateCell(0).SetCellValue(tables[i].date.ToString("yyyy/MM/dd"));
                sheet.GetRow(t).CreateCell(1).SetCellValue(tables[i].category);
                sheet.GetRow(t).CreateCell(2).SetCellValue(tables[i].money);
                sheet.GetRow(t).CreateCell(3).SetCellValue(tables[i].remark);
                sheet.GetRow(t).CreateCell(4).SetCellValue(tables[i].InAndOut);
            }
            string path = HttpContext.Current.Request.PhysicalApplicationPath.ToString();
            string strFilePath = path + string.Format("sample.xls");
            FileStream streamWriter = new FileStream(strFilePath, FileMode.Create, FileAccess.ReadWrite);

            //FileStream streamWriter = new FileStream(@"D:\code\測試.xls", FileMode.Create, FileAccess.ReadWrite);
            workbook.Write(streamWriter);
            streamWriter.Close();
            streamWriter.Dispose();


            batchResult.Status = ServiceStatus.Success;
            batchResult.Result = true;
            batchResult.Message = "上傳成功";

            return batchResult;
        }

        /// <summary>
        /// 列出查詢日期的所有資料
        /// </summary>
        /// <param name="Account">用戶名稱</param>
        /// <param name="startdatevalue">查詢的起始日期</param>
        /// <param name="finishdatevalue">查詢的結束日期</param>
        /// <returns></returns>
        public List<DemoMoneyTable> listQueryDateSelectAll(string Account, string startdatevalue, string finishdatevalue)
        {
            SqlDAOs dao = new SqlDAOs();
            List<DemoMoneyTable> reader = dao.QueryDateUsersData(EncryptAndDecode.Base64Decrypt(Account), startdatevalue, finishdatevalue);
            //List<DemoMoneyTable> reader1 = dao.QueryAllUsersData(EncryptAndDecode.Base64Decrypt(Account));

            return reader;
        }

        public ServiceResult<bool> UploadUserList(HttpPostedFileBase file ,string account)
        {
            ServiceResult<bool> batchResult = new ServiceResult<bool>();

            using (var excel = new ExcelPackage(file.InputStream))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                List<DemoMoneyTable> RowData = new List<DemoMoneyTable>();
                var tbl = new DataTable();
                DataTable datatable = new DataTable();
                var ws = excel.Workbook.Worksheets.First();
                var hasHeader = true;  // adjust accordingly
                                       // add DataColumns to DataTable

                foreach (var firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                {
                    tbl.Columns.Add(hasHeader ? firstRowCell.Text : String.Format("Column {0}", firstRowCell.Start.Column));
                }

                // add DataRows to DataTable
                int startRow = hasHeader ? 2 : 1;
                for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                {
                    //cells 儲存格超做起始值是 1
                    //(從第幾行開始(往下數),從第幾列開始(往右數),結束行,結束列)
                    //sheet1.Cells[3, 3, 5, 5].Value // 從 (3, 3) 一路框到 (5, 5)，包含頭尾
                    var wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                    DataRow row = tbl.NewRow();
                    foreach (var cell in wsRow)
                    {
                        var aa = cell.GetType();
                        var bb = cell.Value;
                        var cc = cell.Text;

                        var dd = cell.Start.Column;

                        try
                        {
                            string aaa = "";
                            if (cell.Text.Length == 0 && string.IsNullOrWhiteSpace(cell.Text))
                            {
                                aaa = "null";
                            }
                            else
                            {
                                aaa = cell.Value.GetType().Name;
                            }
                            
                            switch (aaa)
                            {
                                //字串型態欄位
                                case "String":
                                    //設定dataRow第j欄位的值，cell以字串型態取值
                                    row[cell.Start.Column - 1] = cell.Text;
                                    break;

                                //日期型態欄位
                                case "DateTime":

                                    //設定dataRow第j欄位的值，cell以日期格式取值
                                    var tt = DateTime.Parse(cell.Text).ToString("yyyy/MM/dd");
                                    row[cell.Start.Column - 1] = DateTime.Parse(cell.Text).ToString("yyyy/MM/dd");

                                    break;

                                //數字型態欄位
                                case "Double":

                                    //設定dataRow第j欄位的值，cell以布林型態取值
                                    row[cell.Start.Column - 1] = cell.Text;
                                    break;

                                //空值
                                case "null":

                                    row[cell.Start.Column - 1] = "";
                                    break;

                                // 預設
                                default:

                                    row[cell.Start.Column - 1] = cell.Text;
                                    break;
                            }
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                        catch (Exception ex)
                        {
                            //錯誤訊息
                            throw new Exception("第 " + "i" + "列第" + "column" + "欄，資料格式有誤:\r\r" + ex.ToString());
                        }

                    }
                    tbl.Rows.Add(row);

                }
                var msg = String.Format("DataTable successfully created from excel-file. Colum-count:{0} Row-count:{1}",
                                        tbl.Columns.Count, tbl.Rows.Count);

                foreach(DataRow aa in tbl.Rows)
                {
                    DemoMoneyTable demomoneytable = new DemoMoneyTable()
                    {
                        //傳入excel的資料給 viewmodel
                        //ID = int.Parse(dataRow["ID"].ToString()),
                        date = DateTime.Parse(aa["date"].ToString()),
                        money = int.Parse(aa["money"].ToString()),
                        category = aa["category"].ToString(),
                        remark = aa["remark"].ToString(),
                        users = account,
                        InAndOut = "OUT"
                        //InAndOut = aa["InAndOut"].ToString()
                    };
                    this.Create(demomoneytable);
                }
            }

            return batchResult;
        }







    }
}
