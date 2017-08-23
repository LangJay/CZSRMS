/**************************************************
 *项目名称：BLL.Tools
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/8/14 8:46:59
 *更新时间：2017/8/14 8:46:59
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace BLL.Tools
{
    public static class MyXML
    {
        private static string upath = HttpRuntime.AppDomainAppPath.ToString() + "/Data/";
        private static UnitInfoService unitInfoService = new UnitInfoService();
        private static  ProjectTypeInfoService projectTypeInfoService = new ProjectTypeInfoService();
        private static  FileTypeInfoService fileTypeInfoService = new FileTypeInfoService();
        private static  CoodinateSystemInfoService coodinateSystemInfoService = new CoodinateSystemInfoService();
        private static string GetPath()
        {
            string username = HttpContext.Current.Request.Cookies["username"].Value;
            string userpath = upath + username + ".xml";
            return userpath;
        }
        public static bool CreateXML(string username)
        {
            string path = HttpRuntime.AppDomainAppPath.ToString() + "/Data/FileCategory.xml";
            try
            {
                string userpatn = upath + username + ".xml";
                File.Copy(path, userpatn);
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return false;
            }
            return true;
        }
        public static bool DeleteXML(string username)
        {
            try
            {
                string userpatn = upath + username + ".xml";
                File.Delete(userpatn);
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return false;
            }
            return true;
        }
        public static List<Category> GetAllCategory()
        {
            List<Category> list = new List<Category>();
            try
            {
                XElement xm = XElement.Load(GetPath());
                IEnumerable<XElement> ie = from ele in xm.Elements("kind") select ele;
                foreach (XElement x in ie)
                {
                    Category category = new Category()
                    {
                        kind = x.Attribute("Type").Value,
                        select = x.Attribute("Select").Value
                    };
                    //获取所有子节点

                    category.nodes = GetListByKind(category.kind);
                    list.Add(category);
                }
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return list;
            }
            return list;
        }
        public static List<Category> GetSelectedCategory()
        {
            List<Category> list = new List<Category>();
            try
            {
                XElement xm = XElement.Load(GetPath());
                IEnumerable<XElement> ie = from ele in xm.Elements("kind") where ele.Attribute("Select").Value == "是" select ele;
                foreach (XElement x in ie)
                {
                    Category category = new Category()
                    {
                        kind = x.Attribute("Type").Value,
                        select = x.Attribute("Select").Value
                    };
                    //获取所有子节点

                    category.nodes = GetListByKind(category.kind);
                    list.Add(category);
                }
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return list;
            }
            return list;
        }
        public static List<Category> GetNoSelectedCategory()
        {
            List<Category> list = new List<Category>();
            try
            {
                XElement xm = XElement.Load(GetPath());
                IEnumerable<XElement> ie = from ele in xm.Elements("kind") where ele.Attribute("Select").Value == "否" select ele;
                foreach (XElement x in ie)
                {
                    Category category = new Category()
                    {
                        kind = x.Attribute("Type").Value,
                        select = x.Attribute("Select").Value
                    };
                    //获取所有子节点

                    category.nodes = GetListByKind(category.kind);
                    list.Add(category);
                }
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return list;
            }
            return list;
        }
        public static bool AddElement(Category category)
        {
            try
            {
                XElement xm = XElement.Load(GetPath());
                XElement record = new XElement(
                    new XElement("kind", new XAttribute("Type", category.kind), new XAttribute("Select", "是"))
                );
                xm.Add(record);
                xm.Save(GetPath());
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return false;
            }
            return true;
        }
        public static bool DeleteElement(string kind)
        {
            try
            {
                XElement xm = XElement.Load(GetPath());
                IEnumerable<XElement> elements = from ele in xm.Elements("kind") where ele.Attribute("Type").Value == kind select ele;
                {
                    if (elements.Count() > 0)
                        elements.First().Remove();
                }
                xm.Save(GetPath());
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return false;
            }
            return true;
        }
        public static bool EditElement(string oldkind,Category category)
        {
            try
            {
                XElement xm = XElement.Load(GetPath());
                IEnumerable<XElement> elements = from ele in xm.Elements("kind") where ele.Attribute("Type").Value == oldkind select ele;
                {
                    if (elements.Count() > 0)
                    {
                        XElement x = elements.First();
                        x.Attribute("Type").Value = category.kind;
                        x.Attribute("Select").Value = category.select;
                        x.RemoveNodes();
                    }
                }
                xm.Save(GetPath());
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return false;
            }
            return true;
        }
        public static Category GetElement(string kind)
        {
            Category category = new Category();
            try
            {
                XElement xm = XElement.Load(GetPath());
                IEnumerable<XElement> elements = from ele in xm.Elements("kind") where ele.Attribute("Type").Value == kind select ele;
                {
                    foreach (XElement x in elements)
                    {
                        category.kind = x.Attribute("Type").Value;
                        category.select = x.Attribute("Select").Value;
                        //获取所有子节点
                        category.nodes = GetListByKind(category.kind);
                        return category;
                    }
                }
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return null;
            }
            return null;
        }
        private static List<string> GetListByKind(string kind)
        {
            List<string> list = new List<string>();
            if (kind == "测绘单位")
            {
                List<tb_Unit> unit = unitInfoService.FindAll(u => u.Value != "", "ID", true);
                foreach(var item in unit)
                {
                    list.Add(item.Value);
                }
            }
            else if (kind == "文件类型")
            {
                List<tb_FileType> file = fileTypeInfoService.FindAll(u => u.Value != "", "ID", true);
                foreach (var item in file)
                {
                    list.Add(item.Value);
                }
            }
            else if (kind == "项目类型")
            {
                List<tb_ProjectType> projec = projectTypeInfoService.FindAll(u => u.Value != "", "ID", true);
                foreach (var item in projec)
                {
                    list.Add(item.Value);
                }
            }
            else
            {
                List<tb_CoodinateSystem> coodinateSystem = coodinateSystemInfoService.FindAll(u => u.Value != "", "ID", true);
                foreach (var item in coodinateSystem)
                {
                    list.Add(item.Value);
                }
            }
            return list;
        }
    }
}
