using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using DemoMoney.DAOs;
using DemoMoney.Models.Models;
using DemoMoney.Services;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace DemoMoney.Controllers
{
    public class DemoMoneyController : Controller
    {
        // GET: DemoMoney
        public ActionResult Index()
        {
            //DemoMoneyEntities content = new DemoMoneyEntities();
            //var result = content.DemoMoneyTable;

            //AccountingServices services = new AccountingServices();
            //List<DemoMoneyTable> demoMoneyTables = services.listSelectAll();
            //ViewBag.Message = TempData["Message"] as string;

            if (Session["account"] == null || string.IsNullOrWhiteSpace(Session["account"].ToString()))
            {
                List<DemoMoneyTable> demoMoneyTables = new List<DemoMoneyTable>();
                if(TempData["Message"] as string == "" || TempData["Message"] as string == null )
                {
                    ViewBag.Message = "請登入";
                }
                else
                {
                    ViewBag.Message = TempData["Message"] as string;
                }
                return View(demoMoneyTables);
            }
            else
            {
                AccountingServices services = new AccountingServices();
                List<DemoMoneyTable> demoMoneyTables = services.listSelectAll(Session["account"].ToString());
                ViewBag.Message = TempData["Message"] as string;
                return View(demoMoneyTables);
            }
        }

        [HttpPost]
        public ActionResult Index(int id)
        {
            return View();
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
            if (Session["account"] == null || string.IsNullOrWhiteSpace(Session["account"].ToString()))
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
        public ActionResult Create(FormCollection collection, DemoMoneyTable demomoneytable)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();
            string account =  Session["account"].ToString();
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    AccountingServices services = new AccountingServices();
                    result = services.Create(demomoneytable, account);
                }
                else
                {
                    return View(demomoneytable);
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
                    result = services.UpFile(file, Session["account"].ToString());

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
            UsersDAOs usersDAOs = new UsersDAOs();
            bool aa =  usersDAOs.EditUsers(usersTable);

            ServiceResult<bool> batchResult = new ServiceResult<bool>();

            if (aa)
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

    }
}
