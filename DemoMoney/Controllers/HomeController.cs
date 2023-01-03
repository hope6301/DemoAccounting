using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoMoney.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "歡迎你嘗試使用，如果使用有問題請聯絡管理員。";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "使用問題歡迎寫信給管理員。";

            return View();
        }
    }
}