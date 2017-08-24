
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Model;
using SurveyingResultManageSystem.App_Start;
using BLL.Tools;
using System;

namespace SurveyingResultManageSystem.Controllers
{
    public class FileManagerController : Controller
    {
        [Authentication]
        [HttpPost]
        public ActionResult UpLoadFile(string fileInfoJson)
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            tb_FileInfo obj = JsonConvert.DeserializeObject<tb_FileInfo>(stream) as tb_FileInfo;
            return View();
        }
        [Authentication]
        [HttpGet]
        public string GetSelectedCategory()
        {
            List<Category> list = MyXML.GetSelectedCategory();
            string json = null;
            if (list.Count() > 0)
            {
                try
                {
                    json = JsonConvert.SerializeObject(list);
                }
                catch (Exception e)
                {
                    Log.AddRecord(e);
                    return json;
                }
            }
            return json;
        }
    }
}