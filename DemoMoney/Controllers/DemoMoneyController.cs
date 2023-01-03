using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoMoney.DAOs;
using DemoMoney.Models.Models;
using DemoMoney.Services;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using PagedList;

namespace DemoMoney.Controllers
{
    public class DemoMoneyController : Controller
    {
        // GET: DemoMoney
        public ActionResult Index(string startdatevalue,string finishdatevalue , int Page = 1)
        {

            // true = 沒登入
            // false = 有登入 顯示資料
            if (Request.Cookies["UserKeepLogin"] == null || string.IsNullOrWhiteSpace(Request.Cookies["UserKeepLogin"].Value))
            {
                List<DemoMoneyTable> demoMoneyTables = new List<DemoMoneyTable>();

                if (TempData["Message"] as string == "" || TempData["Message"] as string == null )
                {
                    ViewBag.Message = "請登入";
                    Session["startdatevalue"] = "";
                    Session["finishdatevalue"] = "";
                }
                else
                {
                    ViewBag.Message = TempData["Message"] as string;
                }
                return View(demoMoneyTables);
            }
            else
            {

                //初始查詢，預設傳回過去30天的資料
                if (string.IsNullOrWhiteSpace(startdatevalue) || string.IsNullOrWhiteSpace(finishdatevalue))
                {
                    startdatevalue = DateTime.Now.AddDays(-30).ToString("yyyy/MM/dd");
                    finishdatevalue = DateTime.Now.ToString("yyyy/MM/dd");

                    if (!string.IsNullOrWhiteSpace(Session["startdatevalue"].ToString()) || !string.IsNullOrWhiteSpace(Session["finishdatevalue"].ToString()))
                    {
                        startdatevalue = Session["startdatevalue"].ToString();
                        finishdatevalue = Session["finishdatevalue"].ToString();
                    }
                }
                else
                {
                    //如果使用者選擇時間段，就記錄使用者選擇的日期
                    Session["startdatevalue"] = startdatevalue;
                    Session["finishdatevalue"] = finishdatevalue;
                }
                AccountingServices services = new AccountingServices();

                //列出所有記帳資料
                //List<DemoMoneyTable> demoMoneyTables = services.listSelectAll(Request.Cookies["UserKeepLogin"].Value);
                ViewBag.Message = TempData["Message"] as string;

                //顯示五欄
                const int pageSize = 10;

                //傳回view分頁後的資料
                //var demoMoneyTablesPagedList  = services.listSelectAll(Request.Cookies["UserKeepLogin"].Value).ToPagedList(Page,pageSize);
                var demoMoneyTablesPagedList = services.listQueryDateSelectAll(Request.Cookies["UserKeepLogin"].Value,startdatevalue,finishdatevalue).ToPagedList(Page, pageSize);

                return View(demoMoneyTablesPagedList);
            }
        }

        /// <summary>
        /// 查詢時間內資料傳回Index顯示
        /// </summary>
        /// <param name="startdatevalue">開始時間</param>
        /// <param name="finishdatevalue">結束時間</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string startdatevalue, string finishdatevalue)
        {
            string str_startdatevalue = DateTime.Parse(startdatevalue).ToString("yyyy/MM/dd");
            string str_finishdatevalue = DateTime.Parse(finishdatevalue).ToString("yyyy/MM/dd");

            return RedirectToAction("Index", new { startdatevalue = str_startdatevalue, finishdatevalue = str_finishdatevalue });
        }

        [Authorize]
        public ActionResult Index2()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index2(int id)
        {
            return View();
        }


        // GET: DemoMoney/Details/5
        // GET: Demo/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Demo/Create
        public ActionResult Create()
        {
            if (Request.Cookies["UserKeepLogin"] == null || string.IsNullOrWhiteSpace(Request.Cookies["UserKeepLogin"].Value))
            {
                TempData["Message"] = "沒有登入，不能新增，請登入";
                return RedirectToAction("Index");
            }
            else
            {
                var model = new DemoMoneyTable();
                return View(model);
            }
        }

        // POST: Demo/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, DemoMoneyTable MoneyTable)
        {
            
            EncryptAndDecodeServices EADS = new EncryptAndDecodeServices();

            string account = EADS.Decode(Session["account"].ToString());

            ServiceResult<bool> result = new ServiceResult<bool>();
            try
            {
                //將cookies的帳戶資訊傳入model
                MoneyTable.users = Request.Cookies["UserKeepLogin"].Value;

                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    AccountingServices services = new AccountingServices();
                    result = services.Create(MoneyTable);
                }
                else
                {
                    return View(MoneyTable);
                }
                TempData["Message"] = result.Message;
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = result.Message;
                return View();
            }
        }

        // GET: Demo/Edit/5
        public ActionResult Edit(int id)
        {
            //DemoMoneyEntities content = new DemoMoneyEntities();
            //var result = content.DemoMoneyTable.FirstOrDefault(model => model.ID == id);

            AccountingServices services = new AccountingServices();
            DemoMoneyTable moneyTable = new DemoMoneyTable();
            moneyTable = services.SelectID(id);

            return View(moneyTable);
        }

        // POST: Demo/Edit/5
        [HttpPost]
        public ActionResult Edit(DemoMoneyTable demomoneytable, int id, DateTime date, string category, int money, string remark, string InAndOut)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();
            try
            {
                if (ModelState.IsValid)
                {
                    DemoMoneyTable value = new DemoMoneyTable()
                    {
                        ID = id,
                        date = date,
                        category = category,
                        money = money,
                        remark = remark,
                        InAndOut = InAndOut,
                        DeleteOrNot = "N"
                    };
                    // TODO: Add update logic here
                    AccountingServices services = new AccountingServices();
                    result = services.Edit(value);
                    if (result.Result == true)
                    {
                        TempData["Message"] = result.Message;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Message"] = result.Message;
                        return View("Create", demomoneytable);
                    }
                }
                else
                {
                    TempData["Message"] = result.Message;
                    return View("Create", demomoneytable);
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Demo/Delete/5
        public ActionResult Delete(int id)
        {
            AccountingServices services = new AccountingServices();
            DemoMoneyTable demomoneytable = new DemoMoneyTable();
            demomoneytable = services.SelectID(id);

            return View(demomoneytable);
        }

        // POST: Demo/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                AccountingServices services = new AccountingServices();
                ServiceResult<bool> result = services.Delete(id);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public ActionResult Import()
        {
            if (Session["account"] == null || string.IsNullOrWhiteSpace(Session["account"].ToString()))
            {
                TempData["Message"] = "沒有登入，不能上傳，請登入";
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file)
        {
            ViewBag.Message = "匯入成功";
            try
            {
                ServiceResult<bool> result = new ServiceResult<bool>();
                //if (Session["account"] == null || string.IsNullOrWhiteSpace(Session["account"].ToString()))
                //{
                //    ViewBag.Message = "沒有登入，不能上傳請登入";
                //}


                if (file != null)
                {

                    AccountingServices services = new AccountingServices();

                    result = services.UpFile(file, Request.Cookies["UserKeepLogin"].Value);

                    if (result.Result == false)
                    {
                        ViewBag.Message = result.Message;
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = "沒有上傳檔案";
                    return View();
                }
                TempData["Message"] = result.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "匯入失敗：" + ex.Message;
                return View();
            }

        }

        [HttpPost]
        public ActionResult Button_Click(string sender, int location, EventArgs e)
        {
            //傳好玩的
            string ee = sender;
            //傳好玩的
            int aa = location;

            AccountingServices services = new AccountingServices();

            ServiceResult<bool> batchResult = services.DownloadAll();

            if (batchResult.Result)
            {

                batchResult.Status = ServiceStatus.Success;
                batchResult.Result = true;
                batchResult.Message = "下載成功";

                return Json(batchResult);
            }
            else
            {

                batchResult.Status = ServiceStatus.Failure;
                batchResult.Result = false;
                batchResult.Message = "下載失敗";

                return Json(batchResult);
            }
        }

        protected class resultModel
        {
            public bool status { get; set; }

            public string code { get; set; }

            public string data { get; set; }
        }

        [HttpPost]
        public ActionResult textapi(UsersTableModel usersTable ,EventArgs e)
        {
            //UsersDAOs usersDAOs = new UsersDAOs();
            //bool aa =  usersDAOs.EditUsers(usersTable);

            ServiceResult<bool> batchResult = new ServiceResult<bool>();

            EncryptAndDecodeServices services = new EncryptAndDecodeServices();
            usersTable.Account = "hope6301";
            usersTable.Password = "Aa123456";


            usersTable =  services.ModelEncryp(usersTable);



            if (true)
            {

                batchResult.Status = ServiceStatus.Success;
                batchResult.Result = true;
                batchResult.Message = "下載成功";

                return Json(batchResult);
            }
            else
            {
                batchResult.Status = ServiceStatus.Failure;
                batchResult.Result = false;
                batchResult.Message = "下載失敗";
                return Json(batchResult);
            }
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                //我是存檔

                //var fileName = Path.GetFileName(file.FileName);
                //var path = Path.Combine(Server.MapPath("~/FileUploads"), fileName);
                //file.SaveAs(path);

                AccountingServices services = new AccountingServices();
                services.UploadUserList(file, Request.Cookies["UserKeepLogin"].Value);

                //以下是讀檔
                if (false) {
                    using (var excel = new ExcelPackage(file.InputStream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        List<DemoMoneyTable> RowData = new List<DemoMoneyTable>();
                        var tbl = new DataTable();
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
                                row[cell.Start.Column - 1] = cell.Text;
                            }
                            tbl.Rows.Add(row);

                        }
                        var msg = String.Format("DataTable successfully created from excel-file. Colum-count:{0} Row-count:{1}",
                                                tbl.Columns.Count, tbl.Rows.Count);
                    }


                }
            }

            return RedirectToAction("Index");
        }
        }
}
