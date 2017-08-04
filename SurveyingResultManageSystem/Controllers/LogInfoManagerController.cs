using BLL;
using Model;
using SurveyingResultManageSystem.App_Start;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyingResultManageSystem.Controllers
{
    public class LogInfoManagerController : Controller
    {
        private LogInfoService logInfoService;
        public LogInfoManagerController()
        {
            logInfoService = new LogInfoService();
        }
        // GET: LogInfoManager
        [Authentication]
        public ActionResult MoreLogInfo(int ? pageIndex,string keywords)
        {
            //获取消息滚动条数据
            ViewBag.Data = logInfoService.FindLogListWithTime(DateTime.Now.ToString("d"));

            //-----分页内容---------//

            PageInfo<tb_LogInfo> pageInfo = new PageInfo<tb_LogInfo>();
            //第几页  
            pageInfo.pageIndex = pageIndex ?? 1;

            //每页显示多少条  pageInfo.pageSize

            //所有的记录 pageInfo.totalRecord;
            int totalRecord;
            //获取所有的上传、下载、删除的操作记录 
            string operation = "删除下载上传";
            pageInfo.pageList = logInfoService.FindPageList(pageInfo.pageIndex, pageInfo.pageSize, out totalRecord, u => operation.Contains(u.Operation), "Time", false);
            if (keywords != null && keywords != "")
            {
                //把keywords存到cookies中
                HttpCookie cook = new HttpCookie("keywords", keywords);
                //cook.Expires = DateTime.Now.AddDays(365);//一年，有种你一年不关浏览器
                Response.Cookies.Add(cook);
                pageInfo.keywords = keywords;
                //重新检索记录
                pageInfo.pageList = logInfoService.FinLogListWithOperation(operation);
                List<tb_LogInfo> list = new List<tb_LogInfo>();
                // || l.Operation.Contains(keywords) || l.UserName.Contains(keywords) || l.Operation.Contains(keywords) || l.FileName.Contains(keywords)
                IEnumerable<tb_LogInfo> iEn = pageInfo.pageList.Where(p => p.Time.Contains(keywords) || p.UserName.Contains(keywords) || p.Operation.Contains(keywords) || p.FileName.Contains(keywords));
                int index = 1; 
                foreach (tb_LogInfo l in iEn)
                {
                    if((pageIndex - 1) * pageInfo.pageSize < index && pageIndex * pageInfo.pageSize >= index)
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
        [Authentication]
        public ActionResult LogInfoManager(int? pageIndex, string keywords)
        {
            //获取消息滚动条数据
            ViewBag.Data = logInfoService.FindLogListWithTime(DateTime.Now.ToString("d"));

            //-----分页内容---------//

            PageInfo<tb_LogInfo> pageInfo = new PageInfo<tb_LogInfo>();
            //第几页  
            pageInfo.pageIndex = pageIndex ?? 1;

            //每页显示多少条  pageInfo.pageSize

            //所有的记录 pageInfo.totalRecord;
            int totalRecord;
            //获取所有的操作记录 
            string operation = "";
            pageInfo.pageList = logInfoService.FindPageList(pageInfo.pageIndex, pageInfo.pageSize, out totalRecord, u => u.Operation.Contains(operation), "Time", false);
            if (keywords != null && keywords != "")
            {
                //把keywords存到cookies中
                HttpCookie cook = new HttpCookie("keywords", keywords);
                cook.Expires = DateTime.Now.AddDays(365);//一年，有种你一年不关浏览器
                Response.Cookies.Add(cook);
                pageInfo.keywords = keywords;
                //重新检索记录
                pageInfo.pageList = logInfoService.FindAll(u => u.Time.Contains(""), "Time", false);
                List<tb_LogInfo> list = new List<tb_LogInfo>();
                IEnumerable<tb_LogInfo> iEn = pageInfo.pageList.Where(p => p.Time.Contains(keywords)  || p.UserName.Contains(keywords) || p.Operation.Contains(keywords) || p.FileName.Contains(keywords) || p.Explain.Contains(keywords));
                int index = 1;
                foreach (tb_LogInfo l in iEn)
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