using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyingResultManageSystem.Controllers
{
    public class FileManagerController : Controller
    {
        // GET: FileManager
        public ActionResult UpLoadFile(string fileInfoJson)
        {
            return View();
        }
    }
}