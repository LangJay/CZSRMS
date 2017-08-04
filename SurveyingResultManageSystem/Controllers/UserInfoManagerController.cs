using BLL;
using Model;
using SurveyingResultManageSystem.App_Start;
using System;
using System.Collections.Generic;
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
        [Authentication]
        public ActionResult UserManager(int ? pageIndex,string keywords)
        {
            //获取消息滚动条数据
            ViewBag.Data = logInfoService.FindLogListWithTime(DateTime.Now.ToString("d"));

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
    }
}