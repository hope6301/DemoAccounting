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
            DemoMoneyEntities content = new DemoMoneyEntities();
            var result = content.DemoMoneyTable;

            SqlDAOs sqlDAOs = new SqlDAOs();

            LietDemoMoneyTable demoMoneyTables = sqlDAOs.SelectAll();
           

            return View(demoMoneyTables);
        }

        [HttpPost]
        public ActionResult Index(int id)
        {
            int aaa = id;
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
                    AccountingServices createServices = new AccountingServices();
                    result = createServices.Create(demomoneytable);
                }
                else
                {
                    return View(demomoneytable);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Demo/Edit/5
        public ActionResult Edit(int id)
        {
            int bbb = id;
            DemoMoneyEntities content = new DemoMoneyEntities();
            var result = content.DemoMoneyTable.FirstOrDefault(model => model.ID == id);
            return View(result);
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
                        InAndOut = InAndOut
                    };
                    // TODO: Add update logic here
                    AccountingServices createServices = new AccountingServices();
                    result = createServices.Edit(value);
                    if (result.Result == true)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        return View("Create", demomoneytable);
                    }
                }
                else
                {
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
            demomoneytable = services.Select(id);

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
                ServiceResult<bool> result = new ServiceResult<bool>() { Status = ServiceStatus.Success, Result = true };
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
