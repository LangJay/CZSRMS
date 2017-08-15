using BLL;
using BLL.Tools;
using Model;
using Newtonsoft.Json;
using SurveyingResultManageSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SurveyingResultManageSystem.Controllers
{
    public class CategoryController : Controller
    {
        private LogInfoService logInfoService;
        public CategoryController()
        {
            logInfoService = new LogInfoService();
        }

        // GET: Category
        public ActionResult CategoryIndex()
        {
            string operation = LogOperations.UploadFile() + LogOperations.DownloadFile() + LogOperations.DeleteFile();
            //获取消息滚动条数据，取当天的数据
            string date = DateTime.Now.ToString("d");
            ViewBag.Data = logInfoService.FindLogListAndFirst(l => l.Time.Contains(date) && operation.Contains(l.Operation));
            //获取分类数据
            ViewBag.Selected = MyXML.GetSelectedCategory();
            ViewBag.NoSelected = MyXML.GetNoSelectedCategory();
            ViewBag.All = MyXML.GetAllCategory();
            return View();
        }
        [HttpPost]
        public ActionResult AddCategory()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            try
            {
                Category category = JsonConvert.DeserializeObject<Category>(stream) as Category;
                //判断是否同名
                List<Category> list = MyXML.GetAllCategory();
                foreach(Category c in list)
                {
                    if (c.kind == category.kind)
                        return Content("存在");
                }
                if(category == null) return Content("添加失败！");
                bool success = MyXML.AddElement(category);
                if(!success) return Content("添加失败！");
            }
            catch(Exception e)
            {
                Log.AddRecord(e);
                return Content("添加失败！");
            }
            return Content("添加成功！");
        }
    }
}