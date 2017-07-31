using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace SurveyingResultManageSystem.Controllers
{
    public class FileManagerController : Controller
    {
        [HttpPost]
        public ActionResult UpLoadFile(string fileInfoJson)
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            tb_FileInfo obj = JsonConvert.DeserializeObject<tb_FileInfo>(stream) as tb_FileInfo;
            return View();


        }
    }
}