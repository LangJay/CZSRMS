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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace BLL.Tools
{
    public static class MyXML
    {
        private static string rootPath = HttpRuntime.AppDomainAppPath.ToString();//E:\项目\郴州测绘成果管理项目\郴州市测绘成果项目管理系统\SurveyingResultManageSystem\
        private static string path = rootPath + "/Data/FileCategory.xml";
        public static List<Category> GetAllCategory()
        {
            List<Category> list = new List<Category>();
            try
            {
                XElement xm = XElement.Load(path);
                IEnumerable<XElement> ie = from ele in xm.Elements("kind") select ele;
                foreach (XElement x in ie)
                {
                    Category category = new Category();
                    category.kind = x.Attribute("Type").Value;
                    category.select = x.Attribute("Select").Value;
                    //获取所有子节点
                    IEnumerable<XElement> nodes = x.Elements("node");
                    foreach (XElement n in nodes)
                    {
                        category.nodes.Add(n.Attribute("name").Value);
                    }
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
                XElement xm = XElement.Load(path);
                IEnumerable<XElement> ie = from ele in xm.Elements("kind") where ele.Attribute("Select").Value == "是" select ele;
                foreach (XElement x in ie)
                {
                    Category category = new Category();
                    category.kind = x.Attribute("Type").Value;
                    category.select = x.Attribute("Select").Value;
                    //获取所有子节点
                    IEnumerable<XElement> nodes = x.Elements("node");
                    foreach (XElement n in nodes)
                    {
                        category.nodes.Add(n.Attribute("name").Value);
                    }
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
                XElement xm = XElement.Load(path);
                IEnumerable<XElement> ie = from ele in xm.Elements("kind") where ele.Attribute("Select").Value == "否" select ele;
                foreach (XElement x in ie)
                {
                    Category category = new Category();
                    category.kind = x.Attribute("Type").Value;
                    category.select = x.Attribute("Select").Value;
                    //获取所有子节点
                    IEnumerable<XElement> nodes = x.Elements("node");
                    foreach (XElement n in nodes)
                    {
                        string value = n.Attribute("name").Value;
                        category.nodes.Add(n.Attribute("name").Value);
                    }
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
                XElement xm = XElement.Load(path);
                XElement record = new XElement(
                    new XElement("kind", new XAttribute("Type", category.kind), new XAttribute("Select", "是"))
                );
                foreach (string n in category.nodes)
                {
                    XElement node = new XElement(new XElement("node", new XAttribute("name", n)));
                    record.Add(node);
                }
                xm.Add(record);
                xm.Save(path);
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
                XElement xm = XElement.Load(path);
                IEnumerable<XElement> elements = from ele in xm.Elements("kind") where ele.Attribute("Type").Value == kind select ele;
                {
                    if (elements.Count() > 0)
                        elements.First().Remove();
                }
                xm.Save(path);
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
                XElement xm = XElement.Load(path);
                IEnumerable<XElement> elements = from ele in xm.Elements("kind") where ele.Attribute("Type").Value == oldkind select ele;
                {
                    if (elements.Count() > 0)
                    {
                        XElement x = elements.First();
                        x.Attribute("Type").Value = category.kind;
                        x.Attribute("Select").Value = category.select;
                        x.RemoveNodes();
                        foreach (string n in category.nodes)
                        {
                            XElement node = new XElement(new XElement("node", new XAttribute("name", n)));
                            x.Add(node);
                        }
                    }
                }
                xm.Save(path);
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
                XElement xm = XElement.Load(path);
                IEnumerable<XElement> elements = from ele in xm.Elements("kind") where ele.Attribute("Type").Value == kind select ele;
                {
                    foreach (XElement x in elements)
                    {
                        category.kind = x.Attribute("Type").Value;
                        category.select = x.Attribute("Select").Value;
                        //获取所有子节点
                        IEnumerable<XElement> nodes = x.Elements("node");
                        foreach (XElement n in nodes)
                        {
                            string value = n.Attribute("name").Value;
                            category.nodes.Add(n.Attribute("name").Value);
                        }
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
    }
}
