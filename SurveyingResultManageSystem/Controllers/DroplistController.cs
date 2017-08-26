using BLL;
using Model;
using System.Collections.Generic;
using System.Web.Mvc;

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
        [HttpGet]
        public JsonResult Unitlist()
        {
            //2.1.查询出weight实体,并将其转成DTO类型  
            List<tb_Unit> uinitList = unitInfoService.FindAll(u => u.Value != "", "ID", true);
            //2.2返回json  
            return Json(uinitList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult FiltTypelist()
        { 
            List<tb_FileType> fileTypeList = fileTypeInfoService.FindAll(u => u.Value != "", "ID", true);
            return Json(fileTypeList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ProjectTypelist()
        {
            List<tb_ProjectType> projectTypeList = projectTypeInfoService.FindAll(u => u.Value != "", "ID", true);
            return Json(projectTypeList, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult CoodinateSystemlist()
        {
            List<tb_CoodinateSystem> coodinateSystemList = coodinateSystemInfoService.FindAll(u => u.Value != "", "ID", true);
            return Json(coodinateSystemList, JsonRequestBehavior.AllowGet);
        }
      
    }
}