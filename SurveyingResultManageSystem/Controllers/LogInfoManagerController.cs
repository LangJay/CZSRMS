using BLL;
using Model;
using System;
using System.Web.Mvc;

namespace SurveyingResultManageSystem.Controllers
{
    public class LogInfoManagerController : Controller
    {
        private CZSRMS_DB db = new CZSRMS_DB();
        private LogInfoService logInfoService;
        public LogInfoManagerController()
        {
            logInfoService = new LogInfoService();
        }
        // GET: LogInfoManager
        public ActionResult MoreLogInfo()
        {
            //获取消息滚动条数据
            ViewBag.Data = logInfoService.FindLogListWithTime(DateTime.Now.ToString("yyyy-MM-dd"));
            //获取所有的上传、下载、删除的操作记录
            ViewBag.Logs = logInfoService.FinLogListWithOperation("上传下载删除");
            return View();
        }
    }
}