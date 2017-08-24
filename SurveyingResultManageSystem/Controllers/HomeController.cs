using BLL;
using BLL.Tools;
using Model;
using SurveyingResultManageSystem.App_Start;
using SurveyingResultManageSystem.Models;
using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using ArcServer;

namespace SurveyingResultManageSystem.Controllers
{
    public class HomeController : Controller
    {
        private LogInfoService logInfoService;
        private UserInfoService userInfoService;
        CZSRMS_DBEntities db = new CZSRMS_DBEntities();
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
                    //cook.Expires = DateTime.Now.AddDays(1);//不设置时间，离开浏览器就失效
                    Response.Cookies.Add(cook);
                    //更新最后登录时间
                    userInfo.LastLogintime = DateTime.Now.ToString();
                    userInfoService.Update(userInfo);
                    return RedirectToAction("FileManager", "Home");
                }
                else
                    ModelState.AddModelError("Password", "密码错误");
            }
            catch (Exception ex)
            {
                tb_LogInfo log = new tb_LogInfo();
                log.Time = DateTime.Now.ToString();
                log.UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
                log.Operation = LogOperations.SystemLog();
                log.FileName = null;
                log.Explain = ex.Message;
                logInfoService.Add(log);
                //跳转到错误页
                return RedirectToAction("Error", "Home");
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
            string operation = LogOperations.UploadFile() + LogOperations.DownloadFile() + LogOperations.DeleteFile();
            //获取消息滚动条数据，取当天的数据
            string date = DateTime.Now.ToString("d");
            ViewBag.Data = logInfoService.FindLogListAndFirst(l => l.Time.Contains(date) && operation.Contains(l.Operation));
            return View();
        }
        [Authentication]
        public ActionResult MapManager()
        {
            string operation = LogOperations.UploadFile() + LogOperations.DownloadFile() + LogOperations.DeleteFile();
            //获取消息滚动条数据，取当天的数据
            string date = DateTime.Now.ToString("d");
            ViewBag.Data = logInfoService.FindLogListAndFirst(l => l.Time.Contains(date) && operation.Contains(l.Operation));
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
        [HttpGet]
        public string GetUserName()
        {
            try
            {
                string username = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
                return username;
            }
            catch (Exception e)
            {
                Log.AddRecord(e);

                return null;
            }
        }
        [Authentication]
        [HttpPost]//王军军增加8.23
        public string GetUserinfo()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            tb_UserInfo user = userInfoService.Find(u => u.UserName == stream);
            string str1="";
            if (user!=null)
            {
                str1 = str1 + user.Unit+",";
                str1 = str1 + user.Levels;
            }
            return str1;
        }
        [Authentication]
        [HttpPost]//王军军增加8.23
        public bool delefeature()
        {
            FeatureItem1 item1 = new FeatureItem1();
            
            
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            item1.url = "http://localhost:6080/arcgis/rest/services/fwx1g/FeatureServer/0";
            bool tt = openauto.DeleFeature(item1.url, stream);
            return tt;
        }

    }
}