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
        private UnitInfoService unitInfoService;
        private FileTypeInfoService fileTypeInfoService;
        private ProjectTypeInfoService projectTypeInfoServices;
        private CoodinateSystemInfoService coodinateSystemInfoService;
        public CategoryController()
        {
            logInfoService = new LogInfoService();
            unitInfoService = new UnitInfoService();
            fileTypeInfoService = new FileTypeInfoService();
            projectTypeInfoServices = new ProjectTypeInfoService();
            coodinateSystemInfoService = new CoodinateSystemInfoService();
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
                bool success = false;
                if (category.kind == "测绘单位")
                {
                    tb_Unit unit = new tb_Unit();
                    List<tb_Unit> list = unitInfoService.FindAll(u => u.Value != null, "ID", false);
                    foreach (string item in category.nodes)
                    {
                        if (unitInfoService.Find(u => u.Value == item) == null)
                        {
                            unit.Value = item;
                            if (unitInfoService.Add(unit) != null)
                            {
                                success = true;
                            }
                        }
                    }
                    foreach(tb_Unit u in list)
                    {
                        if(!category.nodes.Exists((string str) => u.Value == str))
                        {
                            success = unitInfoService.Delete(u);
                        }
                    }
                    if (success)
                        return Content("编辑成功！");
                    else
                        return Content("编辑失败！");

                }
                else if (category.kind == "文件类型")
                {
                    tb_FileType file = new tb_FileType();
                    List<tb_FileType> list = fileTypeInfoService.FindAll(u => u.Value != null, "ID", false);
                    foreach (string item in category.nodes)
                    {
                        if(fileTypeInfoService.Find(u => u.Value == item) == null)
                        {
                            file.Value = item;
                            file.Type = "图形";
                            if (fileTypeInfoService.Add(file) != null)
                            {
                                success = true;
                            }
                        }
                    }
                    foreach (tb_FileType u in list)
                    {
                        if (!category.nodes.Exists((string str) => u.Value == str))
                        {
                            success = fileTypeInfoService.Delete(u);
                        }
                    }
                    if (success)
                        return Content("编辑成功！");
                    else
                        return Content("编辑失败！");
                }
                else if (category.kind == "项目类型")
                {
                    tb_ProjectType project = new tb_ProjectType();
                    List<tb_ProjectType> list = projectTypeInfoServices.FindAll(u => u.Value != null, "ID", false);
                    foreach (string item in category.nodes)
                    {
                        if(projectTypeInfoServices.Find(u => u.Value == item) == null)
                        {
                            project.Value = item;
                            if (projectTypeInfoServices.Add(project) != null)
                            {
                                success = true;
                            }
                        }
                    }
                    foreach (tb_ProjectType u in list)
                    {
                        if (!category.nodes.Exists((string str) => u.Value == str))
                        {
                            success = projectTypeInfoServices.Delete(u);
                        }
                    }
                    if (success)
                        return Content("编辑成功！");
                    else
                        return Content("编辑失败！");
                }
                else if (category.kind == "坐标系统")
                {
                    tb_CoodinateSystem cood = new tb_CoodinateSystem();
                    List<tb_CoodinateSystem> list = coodinateSystemInfoService.FindAll(u => u.Value != null, "ID", false);
                    foreach (string item in category.nodes)
                    {
                        if(coodinateSystemInfoService.Find(u => u.Value == item) == null)
                        {
                            cood.Value = item;
                            if (coodinateSystemInfoService.Add(cood) != null)
                            {
                                success = true;
                            }
                        }
                    }
                    foreach (tb_CoodinateSystem u in list)
                    {
                        if (!category.nodes.Exists((string str) => u.Value == str))
                        {
                            success = coodinateSystemInfoService.Delete(u);
                        }
                    }
                    if (success)
                        return Content("编辑成功！");
                    else
                        return Content("编辑失败！");
                }
                else
                {
                    return Content("编辑失败！");
                }
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return Content("编辑失败！");
            }
            return Content("编辑成功！");
        }
        /// <summary>
        /// 保存按钮
        /// </summary>
        /// <param name="select"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
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