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
            ServiceResult<bool> result = new ServiceResult<bool>();
            try
            {
                
                UsersServices services = new UsersServices();
                result = services.CreateUsers(usersmodel);
                if (result.Result)
                {
                    Session["account"] = usersmodel.Account;
                    FormsAuthentication.SetAuthCookie(usersmodel.Account,false);

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
            ServiceResult<bool> result = new ServiceResult<bool>();
            UsersServices services = new UsersServices();
            result = services.UsersLogin(usersmodel);

            if (result.Result)
            {
                //將使用者資訊存放到伺服器端
                Session["account"] = usersmodel.Account;
                Session["password"] = usersmodel.Password;

                FormsAuthentication.SetAuthCookie(usersmodel.Account, false);
                
                //記入 cookie
                HttpCookie ckUserKeepLogin = new HttpCookie("UserKeepLogin");
                ckUserKeepLogin.Value = usersmodel.Account;
                ckUserKeepLogin.Expires = DateTime.Now.AddDays(1);
                //避免cookie被js存取
                ckUserKeepLogin.HttpOnly = true;
                Response.Cookies.Add(ckUserKeepLogin);

                return RedirectToAction("Index", "DemoMoney");
            }
            else
            {
                if(result.Status == 0)
                {
                    ViewBag.Message = result.Message;
                    return View();
                }

                TempData["Message"] = result.Message;
                return RedirectToAction("SignUp");
            }
        }
        
        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult SignOut()
        {
            Session.RemoveAll();
            //記入 cookie

            int limit0 = Request.Cookies.Count;
            HttpCookie ckUserKeepLogin = new HttpCookie("UserKeepLogin");
            ckUserKeepLogin.Expires = DateTime.Now.AddDays(-5);
            ckUserKeepLogin.Values.Clear();
            ckUserKeepLogin.Values.Remove("UserKeepLogin");
            Response.Cookies.Add(ckUserKeepLogin);

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

            //string account = Session["account"].ToString();

            string account = "hope6301";

            UsersTableModel usersTable = usersServices.UserInformation(account);

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
            ServiceResult<bool> result = new ServiceResult<bool>();

            UsersServices usersServices = new UsersServices();
            result = usersServices.EditUser(usersTable);

            if (result.Result)
            {
                result.Status = ServiceStatus.Success;
                result.Result = true;
                result.Message = "下載成功";

                return Json(result);
            }
            else
            {
                result.Status = ServiceStatus.Failure;
                result.Result = false;
                result.Message = "下載失敗";
                return Json(result);
            }
        }

        /// <summary>
        /// 變更密碼
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            int i = 1;

            return View();
        }

        /// <summary>
        /// 變更密碼
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangePassword(string currentpsw,string newpsw,string repeatnewpsw)
        {
            ServiceResult<bool> result = new ServiceResult<bool>() { Status = ServiceStatus.Failure, Result = false, Message = "未登入" };
            try
            {
                string account = Session["account"].ToString();
                string password = Session["password"].ToString();

                //判斷密碼不能為空
                if (currentpsw != null && currentpsw != "")
                {
                    if(password == currentpsw)
                    {
                        if(currentpsw != newpsw)
                        {
                            if (newpsw == repeatnewpsw)
                            {
                                UsersServices usersServices = new UsersServices();
                                result = usersServices.EditPassword(account, newpsw);
                                if (result.Result)
                                {
                                    ViewBag.Message = result.Message;
                                    Session["password"] = newpsw;
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
                                result.Status = ServiceStatus.Failure;
                                result.Result = false;
                                result.Message = "新密碼兩次輸入不同";
                                ViewBag.Message = result.Message;
                                //重複密碼錯誤
                                return Json(result);
                            }
                        }
                        else
                        {
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
                        result.Status = ServiceStatus.Failure;
                        result.Result = false;
                        result.Message = "密碼錯誤";
                        ViewBag.Message = result.Message;
                        //密碼錯誤
                        return Json(result);
                    }
                }
                else
                {
                    result.Status = ServiceStatus.Failure;
                    result.Result = false;
                    result.Message = "請輸入現有密碼";
                    ViewBag.Message = result.Message;
                    //密碼不能為空
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
