﻿using BLL;
using BLL.Tools;
using Model;
using SurveyingResultManageSystem.App_Start;
using SurveyingResultManageSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ArcServer;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace SurveyingResultManageSystem.Controllers
{
    public class HomeController : Controller
    {
        private LogInfoService logInfoService;
        private UserInfoService userInfoService;
        private FileInfoService fileInfoService;
        CZSRMS_DBEntities db = new CZSRMS_DBEntities();
        public HomeController()
        {
            logInfoService = new LogInfoService();
            userInfoService = new UserInfoService();
            fileInfoService = new FileInfoService();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(tb_UserInfo user)
        {
            try
            {
                tb_UserInfo userInfo = userInfoService.FindUserInfoWithUserName(user.UserName);
                if (userInfo == null)
                    ModelState.AddModelError("UserName", "用户名不存在");
                else if (userInfo.Password == user.Password)
                {
                    //把登陆用户名存到cookies中
                    HttpCookie cook = new HttpCookie("username", user.UserName);
                    //cook.Expires = DateTime.Now.AddDays(1);//不设置时间，离开浏览器就失效
                    Response.Cookies.Add(cook);
                    //更新最后登录时间
                    userInfo.LastLogintime = DateTime.Now.ToString();
                    userInfoService.Update(userInfo);
                    return RedirectToAction("FileManager", "Home");
                }
                else
                    ModelState.AddModelError("Password", "密码错误");
            }
            catch (Exception ex)
            {
                tb_LogInfo log = new tb_LogInfo()
                {
                    Time = DateTime.Now.ToString(),
                    UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                    Operation = LogOperations.SystemLog(),
                    FileName = null,
                    Explain = ex.Message
                };
                logInfoService.Add(log);
                //跳转到错误页
                return RedirectToAction("Error", "Home");
            }
            return View();

        }
        [Authentication]
        public ActionResult Logout()
        {
            HttpCookie cookie = new HttpCookie("username", string.Empty)
            {
                Expires = DateTime.Now.AddMonths(-1)
            };
            Response.Cookies.Add(cookie);
            return RedirectToAction("Login", "Home");
        }
        [Authentication]
        [HttpGet]
        public ActionResult FileManager()
        {
            string operation = LogOperations.UploadFile() + LogOperations.DownloadFile() + LogOperations.DeleteFile();
            //获取消息滚动条数据，取当天的数据
            string date = DateTime.Now.ToString("d");
            ViewBag.Data = logInfoService.FindLogListAndFirst(l => l.Time.Contains(date) && operation.Contains(l.Operation));

            return View();
        }
        [Authentication]
        public ActionResult MapManager()
        {
            string operation = LogOperations.UploadFile() + LogOperations.DownloadFile() + LogOperations.DeleteFile();
            //获取消息滚动条数据，取当天的数据
            string date = DateTime.Now.ToString("d");
            ViewBag.Data = logInfoService.FindLogListAndFirst(l => l.Time.Contains(date) && operation.Contains(l.Operation));
            return View();
        }
        [Authentication]
        [HttpGet]
        public PartialViewResult GetFileView(string category, int? pageIndex, string keywords)
        {
            //分类分页数据
            PageInfo<tb_FileInfo> pageInfo = new PageInfo<tb_FileInfo>()
            {
                pageIndex = pageIndex ?? 1
            };
            int totalRecord;
            //用户是否可以看到
            string username = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
            tb_UserInfo user = userInfoService.Find(u => u.UserName == username);
            string unit = user.Unit;
            if (user.Levels == "0")
                unit = "";
            pageInfo.pageList = fileInfoService.FindPageList(pageInfo.pageIndex, pageInfo.pageSize, out totalRecord,
                f => f.PublicObjs.Contains(unit), "ID", false);
            if (keywords != null && keywords != "")
            {
                //把keywords存到cookies中
                HttpCookie cook = new HttpCookie("keywords", keywords);
                Response.Cookies.Add(cook);
                pageInfo.keywords = keywords;
                //重新检索
                pageInfo.pageList = fileInfoService.FindAll(u => u.FileName != "", "id", false);
                List<tb_FileInfo> list = new List<tb_FileInfo>();
                IEnumerable<tb_FileInfo> iEn = pageInfo.pageList.Where(f => f.FileName.Contains(keywords) || f.Directory.Contains(keywords) ||
                f.CoodinateSystem.Contains(keywords) || f.FinishtimeInfo.Contains(keywords) || f.FinishPersonInfo.Contains(keywords) ||
                f.Mark.Contains(keywords) || f.ProjectName.Contains(keywords) || f.FileType.Contains(keywords) || f.ProjectType.Contains(keywords) ||
                f.CenterMeridian.Contains(keywords) || f.Finishtime.Contains(keywords) || f.FinishPerson.Contains(keywords) ||
                f.SurveyingUnitName.Contains(keywords) || f.Explain.Contains(keywords) || f.UploadTime.Contains(keywords));
                int index = 1;
                foreach (tb_FileInfo l in iEn)
                {
                    if ((pageIndex - 1) * pageInfo.pageSize < index && pageIndex * pageInfo.pageSize >= index)
                        list.Add(l);
                    index++;
                }
                pageInfo.pageList = list;
                totalRecord = index - 1;
            }
            else
            {
                //没有关键词的时候
                pageInfo.keywords = "";
            }
            //根据分类检索
            if (category != "" && category != null)
            {
                List<tb_FileInfo> list = new List<tb_FileInfo>();
                //重新检索
                pageInfo.pageList = fileInfoService.FindAll(u => u.FileName != "", "id", false);
                IEnumerable<tb_FileInfo> iEn = pageInfo.pageList.Where(f => f.CoodinateSystem.Contains(category) || f.ProjectType.Contains(category)
                || f.FileType.Contains(category) || f.PublicObjs.Contains(category));
                int index = 1;
                foreach (tb_FileInfo l in iEn)
                {
                    if ((pageIndex - 1) * pageInfo.pageSize < index && pageIndex * pageInfo.pageSize >= index)
                        list.Add(l);
                    index++;
                }
                pageInfo.pageList = list;
                totalRecord = index - 1;
            }
            pageInfo.totalRecord = totalRecord;
            double res = (totalRecord / 1.0) / pageInfo.pageSize;
            pageInfo.totalPage = (int)Math.Ceiling(res);
            return PartialView(pageInfo);
        }
        public ActionResult Error()
        {
            return View();
        }
        [HttpGet]
        public string GetUserName()
        {
            try
            {
                string username = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
                return username;
            }
            catch(Exception e)
            {
                Log.AddRecord(e);

                return null;
            }
        }

       
        [Authentication]
        public void Delete(int fileId)
        {
             DeleteFile(u => u.ID == fileId);
        }
        private void DeleteFile(Expression<Func<tb_FileInfo, bool>> whereLamdba)
        {
            if (string.IsNullOrEmpty(whereLamdba.ToString()))
            {
                throw new ArgumentNullException("参数错误!");
            }
            try
            {
                var file = fileInfoService.Find(whereLamdba);
                //记录下载
                tb_LogInfo log = new tb_LogInfo
                {
                    UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                    Time = DateTime.Now.ToString(),
                    Operation = LogOperations.DeleteFile()
                };
                logInfoService.Add(log);
                if (file == null)
                {
                    log.FileName = null;
                    log.Explain = "文件不存在";
                    AlertMsg("文件不存在");
                    return;
                }
                System.IO.File.Delete(file.Directory);
                bool success = fileInfoService.Delete(file);
                if (success)
                {
                    log.FileName = file.FileName;
                    log.Explain = "删除成功！";
                    logInfoService.Add(log);
                    var response = new { code = 4, fileId = file.ID };
                    Response.Write(new JavaScriptSerializer().Serialize(response));
                }
            }
            catch (Exception e)
            {
                //记录下载
                tb_LogInfo log = new tb_LogInfo
                {
                    UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                    FileName = null,
                    Explain = e.Message,
                    Time = DateTime.Now.ToString(),
                    Operation = LogOperations.DeleteFile()
                };
                logInfoService.Add(log);
                Log.AddRecord(e);
                AlertMsg("删除失败！");
            }
        }
        [Authentication]
        public void Download(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("fileId is errror");
            }
            int id = Convert.ToInt32(fileId);
            var file = fileInfoService.Find(u => u.ID == id);
            if (file == null)
            {
                AlertMsg("文件不存在");
                return;
            }
            try
            {
                //记录下载
                tb_LogInfo log = new tb_LogInfo
                {
                    UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                    FileName = file.FileName,
                    Explain = "请求下载文件！",
                    Time = DateTime.Now.ToString(),
                    Operation = LogOperations.DownloadFile()
                };
                logInfoService.Add(log);
                //以字符流的形式下载文件
                FileStream fs = new FileStream(file.Directory, FileMode.Open, FileAccess.ReadWrite);
                byte[] bytes = new byte[(int)fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
                Response.ContentType = "application/octet-stream";
                //通知浏览器下载文件而不是打开
                Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(file.FileName, System.Text.Encoding.UTF8));
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.End();
            }
            catch(System.IO.FileNotFoundException)
            {
                
            }
            catch(Exception e)
            {
                //记录下载
                tb_LogInfo log = new tb_LogInfo
                {
                    UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                    FileName = file.FileName,
                    Explain = "下载失败！",
                    Time = DateTime.Now.ToString(),
                    Operation = LogOperations.DownloadFile()
                };
                logInfoService.Add(log);
                Log.AddRecord(e);
            }
        }
        private void AlertMsg(string msg)
        {
            Response.ContentType = "text/html";
            Response.Write("<script>alert('" + msg + "');</script>");
        }
        [Authentication]
        [HttpPost]//王军军增加8.23
        public bool delefeature()
        {
            FeatureItem1 item1 = new FeatureItem1();


            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            item1.url = "http://localhost:6080/arcgis/rest/services/BX/FeatureServer/0";
            bool tt = openauto.DeleFeature(item1.url, stream);
            return tt;
        }

        [Authentication]
        [HttpPost]//王军军增加8.23
        public string GetUserinfo()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            tb_UserInfo user = userInfoService.Find(u => u.UserName == stream);
            string str1 = "";
            if (user != null)
            {
                str1 = str1 + user.Unit + ",";
                str1 = str1 + user.Levels;
            }
            return str1;
        }
        [Authentication]
        [HttpPost]//王军军增加8.23
        public string getfileinfo()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            //int idh = int.Parse(stream);
            tb_FileInfo user = fileInfoService.Find(u => u.ObjectID == stream);
            string tt=Newtonsoft.Json.JsonConvert.SerializeObject(user);
            return tt;
        }
        [Authentication]
        [HttpPost]//王军军增加8.23
        public Boolean setfileinfo()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();

            tb_FileInfo fileInfo = JsonConvert.DeserializeObject<tb_FileInfo>(stream) as tb_FileInfo;
            //int idh = int.Parse(stream);
            FeatureItem1 featureItem2 = new FeatureItem1();
            featureItem2.Attributes = new Dictionary<string, object>();
            featureItem2.Attributes.Add("FileName", fileInfo.FileName);//文件名
            
            featureItem2.Attributes.Add("CoodSystem", fileInfo.CoodinateSystem);//坐标框架信息
            if (fileInfo.Finishtime.Trim() != "")
            {
              
                featureItem2.Attributes.Add("FinishTime", fileInfo.Finishtime);
            }//完成时间信息
            featureItem2.Attributes.Add("FshPerson", fileInfo.FinishPerson);//完成人信息
            featureItem2.Attributes.Add("MinCood", fileInfo.MinCoodinate);//最小坐标
            featureItem2.Attributes.Add("MaxCood", fileInfo.MaxCoodinate);//最大坐标
            featureItem2.Attributes.Add("ObjectNum", fileInfo.ObjectNum);//文件中对象数量
            featureItem2.Attributes.Add("Mark", fileInfo.Mark);// 备注信息
            featureItem2.Attributes.Add("ProName", fileInfo.ProjectName);// 所属项目名称
            featureItem2.Attributes.Add("FileType", fileInfo.FileType);// 文件类型，宗地图、供地红线图、报批红线图、地籍图、勘测定界报告、竣工验收测绘报告
            featureItem2.Attributes.Add("ProType", fileInfo.ProjectType);// 所属项目类型，供地、报批、竣工验收
            featureItem2.Attributes.Add("CenterMed", fileInfo.CenterMeridian);// 中央子午线
            featureItem2.Attributes.Add("Yoffset", fileInfo.Yoffset);// 纵坐标偏移值
            featureItem2.Attributes.Add("Xoffset", fileInfo.Xoffset);// 水平坐标偏移值
            featureItem2.Attributes.Add("SUnitName", fileInfo.SurveyingUnitName);// 测绘单位名称，北湖区测绘队、苏仙区测绘队、市局测绘队
            featureItem2.Attributes.Add("Memo", fileInfo.Explain);// 成果说明
            if (fileInfo.UploadTime.Trim() != "")
            {
           
                featureItem2.Attributes.Add("UploadTime", fileInfo.UploadTime);
            }
           // featureItem2.Attributes.Add("UploadTime", fileInfo.UploadTime);// 上传时间
            featureItem2.url = "http://localhost:6080/arcgis/rest/services/BX/FeatureServer/0";
            string idh = fileInfo.ObjectID.Trim();
           var tt1 = openauto.UpdateFeature(featureItem2.url, idh, featureItem2);
            return tt1;
        }
    }
}