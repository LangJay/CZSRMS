﻿using BLL;
using Model;
using SurveyingResultManageSystem.App_Start;
using System;
using System.Web;
using System.Web.Mvc;

namespace SurveyingResultManageSystem.Controllers
{
    public class HomeController : Controller
    {
        private LogInfoService logInfoService;
        private UserInfoService userInfoService;
        CZSRMS_DB db = new CZSRMS_DB();
        public HomeController()
        {
            logInfoService = new LogInfoService();
            userInfoService = new UserInfoService();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(tb_UserInfo user)
        {
            try
            {
                tb_UserInfo userInfo = userInfoService.FindUserInfoWithUserName(user.UserName);
                if (userInfo == null)
                    ModelState.AddModelError("UserName", "用户名不存在");
                else if (userInfo.Password == user.Password)
                {
                    //把登陆用户名存到cookies中
                    HttpCookie cook = new HttpCookie("username", user.UserName);
                    cook.Expires = DateTime.Now.AddDays(1);//一天
                    Response.Cookies.Add(cook);
                    return RedirectToAction("FileManager", "Home");
                }
                else
                    ModelState.AddModelError("Password", "密码错误");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("登录", "服务器故障！");
            }
            return View();

        }
        [Authentication]
        public ActionResult Logout()
        {
            HttpCookie cookie = new HttpCookie("username", string.Empty);
            cookie.Expires = DateTime.Now.AddMonths(-1);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Login", "Home");
        }
        [Authentication]
        public ActionResult FileManager()
        {
            //获取消息滚动条数据，取当天的数据
            ViewBag.Data = logInfoService.FindLogListWithTime(DateTime.Now.ToString("yyyy-MM-dd"));
            return View(ViewBag);
        }
        [Authentication]
        public ActionResult MapManager()
        {
            return View();
        }
    }
}