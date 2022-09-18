using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoMoney.DAOs;
using DemoMoney.Models;
using DemoMoney.Models.Models;
using DemoMoney.Services;

namespace DemoMoney.Controllers
{
    public class DemoMoneyController : Controller
    {
        // GET: DemoMoney
        public ActionResult Index()
        {
            //DemoMoneyEntities content = new DemoMoneyEntities();
            //var result = content.DemoMoneyTable;

            AccountingServices services = new AccountingServices();
            LietDemoMoneyTable demoMoneyTables = services.SelectAll();
            ViewBag.Message = TempData["Message"] as string;


            return View(demoMoneyTables);
        }

        [HttpPost]
        public ActionResult Index(int id)
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
            var model = new DemoMoneyTable();
            

            return View(model);
        }

        // POST: Demo/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, DemoMoneyTable demomoneytable)
        {
            ServiceResult<bool> result = new ServiceResult<bool>();
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    AccountingServices services = new AccountingServices();
                    result = services.Create(demomoneytable);
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
            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase file)
        {
            ViewBag.Message = "匯入成功";
            try
            {
                ServiceResult<bool> result = new ServiceResult<bool>();
                if (file != null)
                {
                    AccountingServices services = new AccountingServices();
                    result = services.UpFile(file);

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
    }
}
