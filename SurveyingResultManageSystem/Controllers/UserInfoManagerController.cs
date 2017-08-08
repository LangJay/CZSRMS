using BLL;
using BLL.Tools;
using Model;
using Newtonsoft.Json;
using SurveyingResultManageSystem.App_Start;
using SurveyingResultManageSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyingResultManageSystem.Controllers
{
    public class UserInfoManagerController : Controller
    {
        private UserInfoService userInfoService;
        private LogInfoService logInfoService;
        public UserInfoManagerController()
        {
            userInfoService = new UserInfoService();
            logInfoService = new LogInfoService();
        }
        // GET: UserInfoManager
        [AuthorityAuthentication]
        public ActionResult UserManager(int ? pageIndex,string keywords)
        {
            string operation = LogOperations.UploadFile() + LogOperations.DownloadFile() + LogOperations.DeleteFile();
            //获取消息滚动条数据，取当天的数据
            ViewBag.Data = logInfoService.FindLogListAndFirst(l => l.Time.Contains(DateTime.Now.ToString("d")) && operation.Contains(l.Operation));

            //-----分页内容---------//

            PageInfo<tb_UserInfo> pageInfo = new PageInfo<tb_UserInfo>();
            //第几页  
            pageInfo.pageIndex = pageIndex ?? 1;

            //每页显示多少条  pageInfo.pageSize

            //所有的记录 pageInfo.totalRecord;
            int totalRecord;
            //获取所有的用户 
            pageInfo.pageList = userInfoService.FindPageList(pageInfo.pageIndex,pageInfo.pageSize,out totalRecord, u => u.UserName != "","Levels",true);
            if (keywords != null && keywords != "")
            {
                //把keywords存到cookies中
                HttpCookie cook = new HttpCookie("keywords", keywords);
                //cook.Expires = DateTime.Now.AddDays(365);//一年，有种你一年不关浏览器。资料说不设置时间就是关闭页面（浏览器？）时清除
                Response.Cookies.Add(cook);
                pageInfo.keywords = keywords;
                //重新检索记录
                pageInfo.pageList = userInfoService.FindAll(u => u.UserName != "", "Levels", true);
                List<tb_UserInfo> list = new List<tb_UserInfo>();
                // || l.Operation.Contains(keywords) || l.UserName.Contains(keywords) || l.Operation.Contains(keywords) || l.FileName.Contains(keywords)
                IEnumerable<tb_UserInfo> iEn = pageInfo.pageList.Where(p => p.UserName.Contains(keywords) || p.LastLogintime.Contains(keywords) || p.Levels.Contains(keywords) || p.Unit.Contains(keywords));
                int index = 1;
                foreach (tb_UserInfo l in iEn)
                {
                    if ((pageIndex - 1) * pageInfo.pageSize < index && pageIndex * pageInfo.pageSize >= index)
                        list.Add(l);
                    index++;
                }
                pageInfo.pageList = list;
                totalRecord = index - 1;
            }
            else
            {
                //没有关键词的时候
                pageInfo.keywords = "";
            }
            pageInfo.totalRecord = totalRecord;
            double res = (totalRecord / 1.0) / pageInfo.pageSize;
            pageInfo.totalPage = (int)Math.Ceiling(res);
            return View(pageInfo);
        }
        [AuthorityAuthentication]
        [HttpPost]
        public ActionResult AddUser()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            tb_LogInfo log = new tb_LogInfo();
            log.UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
            log.Time = DateTime.Now.ToString();
            log.FileName = "";
            log.Operation = LogOperations.CreateUser();
            try
            {
                tb_UserInfo obj = JsonConvert.DeserializeObject<tb_UserInfo>(stream) as tb_UserInfo;
                if(obj == null)
                {
                    log.Explain = "创建用户失败!";
                    logInfoService.Add(log);
                    return Content("创建用户失败!");
                }
                else
                {
                    //检查是否存在相同用户名
                    tb_UserInfo findUserName = userInfoService.Find(u => u.UserName == obj.UserName);
                    if(findUserName != null)
                    {
                        log.Explain = "用户名已经存在！";
                        logInfoService.Add(log);
                        return Content("用户名已经存在！");
                    }
                    log.Explain = "创建用户成功!";
                    logInfoService.Add(log);
                    userInfoService.Add(obj);
                }
            }
            catch(Exception e)
            {
                Log.AddRecord(e);
                log.Explain = "创建用户失败!";
                logInfoService.Add(log);
                return Content("创建用户失败!");
            }
            return Content("创建成功!");
        }
        [AuthorityAuthentication]
        [HttpPost]
        public ActionResult DeleteUser()
        {
            var sr = new StreamReader(Request.InputStream);
            var username = sr.ReadToEnd();
            sr.Close();
            tb_LogInfo log = new tb_LogInfo();
            log.UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
            log.Time = DateTime.Now.ToString();
            log.FileName = "";
            log.Operation = LogOperations.DeleteUser();
            try
            {
                if (username != "")
                {
                    if(userInfoService.Delete(u => u.UserName == username))
                    {
                        log.Explain = "删除成功！";
                        logInfoService.Add(log);
                        return Content("删除成功！");
                    }
                    else
                    {
                        log.Explain = "删除失败！";
                        logInfoService.Add(log);
                        return Content("删除失败！");
                    }
                }
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                log.Explain = "删除失败！";
                logInfoService.Add(log);
                return Content("删除失败！");
            }
            return Content("删除成功！");
        }
        [AuthorityAuthentication]
        [HttpPost]
        public ActionResult ResetPassWords()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            tb_LogInfo log = new tb_LogInfo();
            log.UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
            log.Time = DateTime.Now.ToString();
            log.FileName = "";
            log.Operation = LogOperations.ResetPasswords();
            try
            {
                tb_UserInfo obj = JsonConvert.DeserializeObject<tb_UserInfo>(stream) as tb_UserInfo;
                //根据信息找到完整用户信息
                tb_UserInfo user = userInfoService.Find(u => u.UserName == obj.UserName);
                user.Password = ConfigurationManager.AppSettings["DefaultPwd"];
                if (user != null)
                {
                    if (userInfoService.Update(user))
                    {
                        log.Explain = "修改成功！";
                        logInfoService.Add(log);
                        return Content("修改成功！");
                    }
                    else
                    {
                        log.Explain = "修改失败！";
                        logInfoService.Add(log);
                        return Content("修改失败！");
                    }
                }
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                log.Explain = "修改失败！";
                logInfoService.Add(log);
                return Content("修改失败！");
            }
            return Content("删除成功！");
        }
        [AuthorityAuthentication]
        [HttpGet]
        public string GetUserLevels()
        {
            string username = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
            string levels = "";
            try
            {
                //根据信息找到完整用户信息
                tb_UserInfo user = userInfoService.Find(u => u.UserName == username);
                levels = user.Levels;
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                RedirectToAction("Error", "Home");
            }
            return levels;
        }

    }
}