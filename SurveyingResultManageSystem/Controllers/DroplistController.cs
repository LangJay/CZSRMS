using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyingResultManageSystem.Controllers
{
    public class DroplistController : Controller
    {
        // GET: Droplist
        public ActionResult Unitlist()
        {
            //2.1.查询出weight实体,并将其转成DTO类型  
            List<tb_Unit> uinitList = weightBLL.LoadEnities().ToList().Select(s => s.ToDto()).ToList();
            //2.2返回json  
            return Json(weightList, JsonRequestBehavior.AllowGet);
        }
    }
}