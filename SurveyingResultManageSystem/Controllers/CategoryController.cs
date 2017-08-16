using BLL;
using BLL.Tools;
using Model;
using Newtonsoft.Json;
using SurveyingResultManageSystem.App_Start;
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
        [Authentication]
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
        [Authentication]
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
        [Authentication]
        [HttpGet]
        public ActionResult DeleteCategory(string kind)
        {
            bool success = MyXML.DeleteElement(kind);
            if (!success) return Content("删除失败！");
            return Content("删除成功！");
        }
        [Authentication]
        [HttpGet]
        public string GetCategory(string kind)
        {
            Category category = MyXML.GetElement(kind);
            string json = null;
            if (category != null)
            {
                try
                {
                    json = JsonConvert.SerializeObject(category);
                }
                catch(Exception e)
                {
                    Log.AddRecord(e);
                    return json;
                }
            }
            return json;
        }
        [Authentication]
        [HttpPost]
        public ActionResult EditCategory()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            try
            {
                Category category = JsonConvert.DeserializeObject<Category>(stream) as Category;
                Category newcategory = category;
                //取出新旧名
                string oldname = category.kind.Split('^')[0];
                Category oldcategory = MyXML.GetElement(oldname);
                string newname = category.kind.Split('^')[1];
                newcategory.kind = newname;
                newcategory.select = oldcategory.select;
                bool success = MyXML.EditElement(oldname, newcategory);
                if (!success) return Content("编辑失败！");
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return Content("编辑失败！");
            }
            return Content("编辑成功！");
        }
        [Authentication]
        [HttpGet]
        public ActionResult EditCategorySelect(string select,string kind)
        {
            try
            {
                Category oldcategory = MyXML.GetElement(kind);
                oldcategory.select = select;
                bool success = MyXML.EditElement(kind, oldcategory);
                if (!success) return Content("编辑失败！");
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return Content("编辑失败！");
            }
            return Content("编辑成功！");
        }
    }
}