using BLL;
using BLL.Tools;
using Model;
using SurveyingResultManageSystem.App_Start;
using SurveyingResultManageSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyingResultManageSystem.Controllers
{
    public class SetPasswordsController : Controller
    {
        private LogInfoService logInfoService;
        private UserInfoService userInfoService;
        public SetPasswordsController()
        {
            logInfoService = new LogInfoService();
            userInfoService = new UserInfoService();
        }
        [Authentication]
        public ActionResult Index()
        {
            string operation = LogOperations.UploadFile() + LogOperations.DownloadFile() + LogOperations.DeleteFile();
            //获取消息滚动条数据，取当天的数据
            ViewBag.Data = logInfoService.FindLogListAndFirst(l => l.Time.Contains(DateTime.Now.ToString("d")) && operation.Contains(l.Operation));
            return View(ViewBag);
        }
        [Authentication]
        [HttpPost]
        public void SetPasswords()
        {
            var sr = new StreamReader(Request.InputStream);
            var pwd = sr.ReadToEnd();
            sr.Close();
            //从cookie取得用户信息，更改后更新用户信息，重新登录
            string username = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
            tb_LogInfo log = new tb_LogInfo();
            log.UserName = username;
            log.Time = DateTime.Now.ToString();
            log.FileName = "";
            log.Operation = LogOperations.SetPasswords();
            try
            {
                //根据信息找到完整用户信息
                tb_UserInfo user = userInfoService.Find(u => u.UserName == username);
                user.Password = pwd;
                if (user != null)
                {
                    if (userInfoService.Update(user))
                    {
                        log.Explain = "修改成功！";
                        logInfoService.Add(log);
                    }
                    else
                    {
                        log.Explain = "修改失败！";
                        logInfoService.Add(log);
                    }
                }
            }
            catch (Exception e)
            {
                Log.AddRecord(e.Message);
                log.Explain = "修改失败！";
                logInfoService.Add(log);
            }
            return;
        }
        [HttpGet]
        public string GetPassWords()
        {
            string username = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
            string pwd = "";
            try
            {
                tb_UserInfo user = userInfoService.Find(u => u.UserName == username);
                if (user != null)
                    pwd = user.Password;
            }
            catch(Exception e)
            {
                Log.AddRecord(e.Message);
                return pwd;
            }
            return pwd;
        }
    }
}