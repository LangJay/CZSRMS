using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;

namespace SurveyingResultManageSystem.Controllers
{
    public class DroplistController : Controller
    {
        private UnitInfoService unitInfoService;
        private ProjectTypeInfoService projectTypeInfoService;
        private FileTypeInfoService fileTypeInfoService;
        private CoodinateSystemInfoService coodinateSystemInfoService;
        public DroplistController()
        {
            unitInfoService = new UnitInfoService();
            projectTypeInfoService = new ProjectTypeInfoService();
            fileTypeInfoService = new FileTypeInfoService();
            coodinateSystemInfoService = new CoodinateSystemInfoService();

        }
        // GET: Droplist
        public ActionResult Unitlist()
        {
            //2.1.查询出weight实体,并将其转成DTO类型  
            List<tb_Unit> uinitList = unitInfoService.FindAll(u => u.Value != "", "ID", true);
            //2.2返回json  
            return Json(uinitList, JsonRequestBehavior.AllowGet);
        }
    }
}