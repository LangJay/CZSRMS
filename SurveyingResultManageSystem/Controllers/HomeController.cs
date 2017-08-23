using BLL;
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
        [HttpPost]
        public void UpLoadFile()
        {
            var files = Request.Files;
            if (files.Count > 0)
            {
                var file = files[0];
                string fileName = file.FileName.Split('\\').Last();
                Stream inputStream = file.InputStream;
                string fileSaveFolder = HttpRuntime.AppDomainAppPath.ToString() + "/Data/File/";
                //如果目标不存在，则创建
                if (!Directory.Exists(fileSaveFolder))
                {
                    Directory.CreateDirectory(fileSaveFolder);

                }
                long lenth = inputStream.Length;
                byte[] buffer = new byte[lenth];
                inputStream.Read(buffer, 0, buffer.Length);
                //string strFileMd5 = MD5Helper.GetMD5FromFile(buffer);
                string fileSavePath = Path.Combine(fileSaveFolder, fileName);
                file.SaveAs(fileSavePath);
                inputStream.Close();
                tb_FileInfo fileInfo = FormClass.FormToClass<tb_FileInfo>(Request.Form);
                //tb_FileInfo obj = JsonConvert.DeserializeObject<tb_FileInfo>(stream) as tb_FileInfo;
                fileInfo.Directory = fileSavePath;
                fileInfo.FileName = fileName;
                var username = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
                fileInfo.UserID = userInfoService.Find(u => u.UserName == username).ID;
                fileInfo.UploadTime = DateTime.Now.ToString();
                fileInfo.FileSize = lenth / 1024.00 / 1024.00;
                fileInfo.PublicObjs = fileInfo.PublicObjs.Replace(",", "|");

                if (fileInfoService.Add(fileInfo) != null)
                {
                    LogInfoService logInfoService = new LogInfoService();
                    tb_LogInfo log = new tb_LogInfo()
                    {
                        UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                        Time = DateTime.Now.ToString(),
                        Operation = LogOperations.UploadFile(),
                        FileName = fileInfo.FileName,
                        Explain = fileInfo.Explain
                    };
                    logInfoService.Add(log);
                    AlertMsg("上传成功！");
                }
                else
                {
                    AlertMsg("上传失败！");
                }
            }

        }
        [Authentication]
        public void Delete(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("fileId is errror");
            }
            try
            {
                int id = Convert.ToInt32(fileId);
                var file = fileInfoService.Find(u => u.ID == id);
                if (file == null)
                {
                    AlertMsg("文件不存在");
                    return;
                }
                System.IO.File.Delete(file.Directory);
            }
            catch(Exception e)
            {
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
            string filePath = Request.MapPath(file.Directory);
            //以字符流的形式下载文件
            FileStream fs = new FileStream(filePath, FileMode.Open);
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
        private void AlertMsg(string msg)
        {
            Response.ContentType = "text/html";
            Response.Write("<script>alert('" + msg + "');</script>");
        }
    }
}