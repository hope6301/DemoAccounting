using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoMoney.Models.Models;
using DemoMoney.DAOs;
using DemoMoney.Services;
using System.Web.Security;

namespace DemoMoney.Controllers
{
    public class UsersController : Controller
    {
        // GET: Users/Create
        public ActionResult SignUp()
        {
            ViewBag.Message = TempData["Message"] as string;
            return View();
        }

        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="usersmodel"></param>
        /// <returns></returns>
        // POST: Users/Create
        [HttpPost]
        public ActionResult SignUp(FormCollection collection,UsersTableModel usersmodel)
        {
            try
            {
                UsersServices services = new UsersServices();
                ServiceResult<bool> result = services.CreateUsers(usersmodel);

                if (result.Result)
                {

                    Session["account"] = usersmodel.Account;
                    Session["startdatevalue"] = "";
                    Session["finishdatevalue"] = "";

                    FormsAuthentication.SetAuthCookie(usersmodel.Account, false);

                    //記入 cookie
                    HttpCookie ckUserKeepLogin = new HttpCookie("UserKeepLogin");
                    ckUserKeepLogin.Value = usersmodel.Account;
                    ckUserKeepLogin.Expires = DateTime.Now.AddDays(1);
                    //避免cookie被js存取
                    ckUserKeepLogin.HttpOnly = true;
                    Response.Cookies.Add(ckUserKeepLogin);

                    TempData["Message"] = "開始第一筆記賬";
                    return RedirectToAction("Index", "DemoMoney");
                }
                else
                {
                    ViewBag.Message = result.Message;
                    return View();
                }
            }
            catch(Exception e)
            {
                ViewBag.Message = e.Message;
                return View();
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="usersmodel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection collection,UsersTableModel usersmodel)
        {
            try
            {
                ServiceResult<bool> result = new ServiceResult<bool>();
                UsersServices services = new UsersServices();
                result = services.UsersLogin(usersmodel);

                if (result.Result)
                {
                    //將使用者資訊存放到伺服器端
                    Session["account"] = usersmodel.Account;
                    Session["startdatevalue"] = "";
                    Session["finishdatevalue"] = "";

                    FormsAuthentication.SetAuthCookie(usersmodel.Account, createPersistentCookie:false);

                    //記入 cookie
                    HttpCookie ckUserKeepLogin = new HttpCookie("UserKeepLogin");

                    //存入cookie的值
                    ckUserKeepLogin.Value = usersmodel.Account;

                    //設置 cookie的紀錄時間
                    //不設置時間 瀏覽器關閉就會清除(跟隨瀏覽器的生命週期)
                    //ckUserKeepLogin.Expires = DateTime.Now.AddDays(1);

                    //避免cookie被js存取
                    ckUserKeepLogin.HttpOnly = true;
                    Response.Cookies.Add(ckUserKeepLogin);

                    TempData["Message"] = result.Message;
                    return RedirectToAction("Index", "DemoMoney");
                }
                else
                {
                    //密碼不正確
                    if (result.Status == 0)
                    {
                        ViewBag.Message = result.Message;
                        return View();
                    }

                    //找不到使用者，未註冊過
                    TempData["Message"] = result.Message;
                    return RedirectToAction("SignUp");
                }
            }
            catch(Exception e)
            {
                ViewBag.Message = e.Message;
                return View();
            }
        }
        
        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOut()
        {
            Session.RemoveAll();

            //HttpCookie ckUserKeepLogin = new HttpCookie("UserKeepLogin");
            //ckUserKeepLogin.Expires = DateTime.Now.AddDays(-5);
            //ckUserKeepLogin.Values.Clear();
            //ckUserKeepLogin.Values.Remove("UserKeepLogin");
            //Response.Cookies.Add(ckUserKeepLogin);


            HttpContext.Response.Cookies["UserKeepLogin"].Expires = DateTime.Now.AddDays(-1);

            FormsAuthentication.SignOut();

            TempData["Message"] = "登出成功";

            return RedirectToAction("Index", "DemoMoney");
        }

        /// <summary>
        /// 修改會員資料
        /// </summary>
        /// <returns></returns>
        public ActionResult MemberCentre()
        {
            UsersServices usersServices = new UsersServices();

            UsersTableModel usersTable = usersServices.UserInformation(Session["account"].ToString());

            return View(usersTable);
        }

        /// <summary>
        /// 修改會員資料
        /// </summary>
        /// <param name="usersTable">會員資訊table</param>
        /// <param name="e"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditUsers(UsersTableModel usersTable, EventArgs e)
        {
            try
            {
                ServiceResult<bool> result = new ServiceResult<bool>();
                UsersServices usersServices = new UsersServices();
                result = usersServices.EditUser(usersTable);
                return Json(result);
            }
            catch(Exception error)
            {
                TempData["Message"] = error.Message;
                return RedirectToAction("MemberCentre");
            }
        }

        /// <summary>
        /// 變更密碼
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            return View();
        }


        /// <summary>
        /// 變更密碼
        /// </summary>
        /// <param name="OldPassword">舊密碼</param>
        /// <param name="newpsw">新密碼</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePassword(string OldPassword, string NewPassword)
        {
            ServiceResult<bool> result = new ServiceResult<bool>() { Status = ServiceStatus.Failure, Result = false, Message = "未登入" };
            try
            {
                EncryptAndDecodeServices EADS = new EncryptAndDecodeServices();
                UsersServices usersServices = new UsersServices();
                string account = Session["account"].ToString();

                //傳入密碼到services 做判斷，舊密碼是否正確
                if(usersServices.QueryPassword(account, OldPassword))
                {
                    if (OldPassword != NewPassword)
                    {
                        //可以修改密碼
                        result = usersServices.EditPassword(account, NewPassword);
                        if (result.Result)
                        {
                            ViewBag.Message = result.Message;
                            //密碼修改成功
                            return Json(result);
                        }
                        else
                        {
                            return Json(result);
                        }
                    }
                    else
                    {
                        //新舊密碼不能一樣
                        result.Status = ServiceStatus.Failure;
                        result.Result = false;
                        result.Message = "新舊密碼不能一樣";
                        ViewBag.Message = result.Message;
                        //重複密碼錯誤
                        return Json(result);
                    }
                }
                else
                {
                    //密碼錯誤
                    result.Status = ServiceStatus.Failure;
                    result.Result = false;
                    result.Message = "密碼錯誤";
                    ViewBag.Message = result.Message;
                    //密碼錯誤
                    return Json(result);
                }
            }
            catch(Exception e)
            {
                ViewBag.Message = e.Message;
                return Json(result);
            }

        }

    }
}
