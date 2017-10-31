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
using System.Web.Script.Serialization;
using ArcServer;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Ionic.Zip;
using System.Configuration;
using System.Transactions;
using System.Text;

namespace SurveyingResultManageSystem.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 排序字段，默认Finishtime
        /// </summary>
        private string ORDER_NAME = "Finishtime";
        /// <summary>
        /// 是否倒叙
        /// </summary>
        private bool DESC = false;
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
                tb_UserInfo userInfo = userInfoService.Find(u => u.UserName == user.UserName);
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
                    if (userInfo.Levels == "-1")
                    {
                        return RedirectToAction("UserManager", "UserInfoManager");
                    }
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
        public ActionResult Logout()
        {
            HttpCookie cookie = new HttpCookie("username", string.Empty)
            {
                Expires = DateTime.Now.AddMonths(-1)
            };
            Response.Cookies.Add(cookie);
            return RedirectToAction("Login", "Home");
        }
        [MapAuthentication]
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
        [MapAuthentication]
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
        public PartialViewResult GetFileView(string category, int? pageIndex, string keywords,string key)
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
            if (user.Levels == "0")// || user.Unit == "市局测绘队"
                unit = "";
            pageInfo.pageList = fileInfoService.FindPageList(pageInfo.pageIndex, pageInfo.pageSize, out totalRecord,
                f => f.PublicObjs.Contains(unit) && f.WasDeleted == false, ORDER_NAME, DESC);
            if (!string.IsNullOrEmpty(keywords) || key == "MyFile")
            {
                //把keywords存到cookies中
                HttpCookie cook = new HttpCookie("keywords", keywords);
                Response.Cookies.Add(cook);
                pageInfo.keywords = keywords;
                //重新检索
                pageInfo.pageList = fileInfoService.FindAll(f => f.PublicObjs.Contains(unit) && f.WasDeleted == false, ORDER_NAME, DESC);
                List<tb_FileInfo> list = new List<tb_FileInfo>();
                IEnumerable<tb_FileInfo> iEn;
                switch (key)
                {

                    case "MyFile":
                        iEn = pageInfo.pageList.Where(f => f.UserID == user.ID);
                        break;
                    case "SurveyingUnitName":
                        iEn = pageInfo.pageList.Where(f => f.SurveyingUnitName.Contains(keywords));
                        break;
                    case "CoodinateSystem":
                        iEn = pageInfo.pageList.Where(f => f.CoodinateSystem.Contains(keywords));
                        break;
                    case "ProjectName":
                        iEn = pageInfo.pageList.Where(f => f.ProjectName.Contains(keywords));
                        break;
                    case "FinishPerson":
                        iEn = pageInfo.pageList.Where(f => f.FinishPerson.Contains(keywords));
                        break;
                    case "Finishtime":
                        iEn = pageInfo.pageList.Where(f => f.Finishtime.Contains(keywords));
                        break;
                    case "UploadTime":
                        iEn = pageInfo.pageList.Where(f => f.UploadTime.Contains(keywords));
                        break;
                    case "FileName":
                        iEn = pageInfo.pageList.Where(f => f.FileName.Contains(keywords));
                        break;
                    default:
                        iEn = pageInfo.pageList.Where(f => f.FileName.Contains(keywords) || f.Directory.Contains(keywords) ||
                        f.CoodinateSystem.Contains(keywords) || f.FinishtimeInfo.Contains(keywords) || f.FinishPersonInfo.Contains(keywords) ||
                        f.Mark.Contains(keywords) || f.ProjectName.Contains(keywords) || f.FileType.Contains(keywords) || f.ProjectType.Contains(keywords) ||
                        f.CenterMeridian.Contains(keywords) || f.Finishtime.Contains(keywords) || f.FinishPerson.Contains(keywords) ||
                        f.SurveyingUnitName.Contains(keywords) || f.Explain.Contains(keywords) || f.UploadTime.Contains(keywords));
                        break;
                }
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
                pageInfo.pageList = fileInfoService.FindAll(f => f.PublicObjs.Contains(unit) && f.WasDeleted == false, ORDER_NAME, DESC);
                IEnumerable<tb_FileInfo> iEn = pageInfo.pageList.Where(f => f.CoodinateSystem.Contains(category) || f.ProjectType.Contains(category)
                || f.FileType.Contains(category) || f.SurveyingUnitName.Contains(category));
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
            catch (Exception e)
            {
                Log.AddRecord(e);

                return null;
            }
        }

        //前端调用
        public void Delete(int fileId)
        {
            if (DeleteFile(u => u.ID == fileId))
            {
                var response = new { code = 4, fileId = fileId };
                Response.Write(new JavaScriptSerializer().Serialize(response));
            }
        }
        /// <summary>
        /// 根据id删除所选文件，需要返回删除的id
        /// </summary>
        [HttpPost]
        public void Deletes()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            string[] arr = JsonConvert.DeserializeObject<string[]>(stream) as string[];
            sr.Close();
            List<string> delIds = new List<string>();
            if (arr == null)
            {
                Response.Write(new JavaScriptSerializer().Serialize(null));
                return;
            }
            foreach (string id in arr)
            {
                int idInt = int.Parse(id);
                if (DeleteFile(u => u.ID == idInt))
                {
                    delIds.Add(id);
                }
            }
            if (delIds.Count == 0)
                Response.Write(new JavaScriptSerializer().Serialize(null));
            else
                Response.Write(new JavaScriptSerializer().Serialize(delIds));
        }
        /// <summary>
        /// 根据id删除所选文件，需要返回删除的id
        /// </summary>
        [HttpPost]
        public void DeletesWithObjId()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            string[] arr = JsonConvert.DeserializeObject<string[]>(stream) as string[];
            sr.Close();
            List<string> delIds = new List<string>();
            if (arr == null)
            {
                Response.Write(new JavaScriptSerializer().Serialize(null));
                return;
            }
            foreach (string objId in arr)
            {
                if (DeleteFile(u => u.ObjectID.Contains(objId.Trim())))
                {
                    delIds.Add(objId);
                }
            }
            if (delIds.Count == 0)
                Response.Write(new JavaScriptSerializer().Serialize(null));
            else
                Response.Write(new JavaScriptSerializer().Serialize(delIds));
        }
        private bool DeleteFile(Expression<Func<tb_FileInfo, bool>> whereLamdba)
        {
            if (string.IsNullOrEmpty(whereLamdba.ToString()))
            {
                return false;
            }
            try
            {
                var file = fileInfoService.Find(whereLamdba);
                if (!AuthenLevel(file.SurveyingUnitName)) return false;//没有权限删除
                //记录删除
                tb_LogInfo log = new tb_LogInfo
                {
                    UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                    Time = DateTime.Now.ToString(),
                    Operation = LogOperations.DeleteFile()
                };
                if (file == null)
                {
                    log.FileName = "";
                    log.Explain = "文件不存在";
                    logInfoService.Add(log);
                    return false;
                }
                using (TransactionScope tran = new TransactionScope())
                { 
                    //删除数据库 
                    bool success = fileInfoService.Delete(file);
                    if (success)
                    {
                        //删除图形，考虑没有图形文件的情况
                        bool success1 = deleDWG(file.ObjectID);
                        log.FileName = file.FileName;
                        log.Explain = "删除成功！";
                        logInfoService.Add(log);
                        //删除文件
                        DirectoryInfo dreInfo = new DirectoryInfo(file.Directory);
                        dreInfo.Delete(true);
                        //提交事务
                        tran.Complete();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                //记录删除
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
                return false;
            }
        }
       
        //地图管理下载
        [Authentication]
        [HttpPost]
        public void DownloadsWithObjId()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            string[] ids = JsonConvert.DeserializeObject<string[]>(stream) as string[];
            sr.Close();
            try
            {
                List<tb_FileInfo> list = new List<tb_FileInfo>();
                tb_FileInfo file = new tb_FileInfo();
                for (int i = 0; i < ids.Length; i++)
                {
                    string objid = ids[i];
                    file = fileInfoService.Find(u => u.ObjectID == objid);
                    if (file != null)
                        list.Add(file);
                }
                if (list.Count > 0)
                {
                    string url = ZipFile(list);
                    Response.Write(url);
                }
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                Response.Write("");
            }
        } 
        /// <summary>
        /// code的数字带表意思：
        /// code=1：参数错误；
        /// code=2：文件不存在；
        /// code=3：服务器错误；
        /// code=4：下载成功；
        /// 文件管理下载
        /// </summary>
        [Authentication]
        [HttpPost]
        public void Downloads()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            string[] ids = JsonConvert.DeserializeObject<string[]>(stream) as string[];
            sr.Close();
            try
            {
                List<tb_FileInfo> list = new List<tb_FileInfo>();
                tb_FileInfo file = new tb_FileInfo();
                for (int i = 0; i < ids.Length; i++)
                {
                    int id = int.Parse(ids[i]);
                    file = fileInfoService.Find(u => u.ID == id);
                    if (file != null)
                        list.Add(file);
                }
                if (list.Count > 0)
                {
                    string url = ZipFile(list);
                    Response.Write(url);
                }
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                Response.Write("");
            }
        }
        private string ZipFile(List<tb_FileInfo> list)
        {
            string direct = HttpRuntime.AppDomainAppPath.ToString() + "/Home/Data/File/";
            string savePath = HttpRuntime.AppDomainAppPath.ToString() + "/Home/Data/File/下载.zip";//压缩文件保存路径
            if (!Directory.Exists(direct))
            {
                Directory.CreateDirectory(direct);
            }
            if (System.IO.File.Exists(savePath))
            {
                try
                {
                    System.IO.File.Delete(savePath);
                }
                catch (Exception e)
                {
                    savePath  = HttpRuntime.AppDomainAppPath.ToString() + "/Data/File/temp.zip";//压缩文件保存路径
                }
            }
            //把下载文件压缩成文件夹
            using (ZipFile zipFile = new ZipFile(Encoding.Default))
            {
                foreach (tb_FileInfo file in list)
                {
                    string safeFileName = file.FileName.Substring(0, file.FileName.IndexOf('.'));//压缩文件的文件名
                    zipFile.AddDirectory(Path.Combine(file.Directory, "原始文件"), safeFileName);
                }
                zipFile.Save(savePath);//太费时
            }
            return "Data/File/下载.zip";
        }
        private void DownloadTask(string savePath)
        {
            //以字符流的形式下载文件
            FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            Response.ContentType = "application/octet-stream";
            //通知浏览器下载文件而不是打开
            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode("下载.zip", Encoding.UTF8));
            Response.AddHeader("Content-Length", fs.Length.ToString());
            //还没有读取的文件内容长度
            long leftLength = fs.Length;
            //创建接收文件内容的字节数组
            byte[] buffer = new byte[1024 * 10];
            //每次读取的最大字节数
            int maxLength = buffer.Length;
            //每次实际返回的字节数长度
            int num = 0;
            //文件开始读取的位置
            int fileStart = 0;
            while (leftLength > 0)
            {
                //设置文件流的读取位置
                fs.Position = fileStart;
                //读取最大字节
                if (leftLength < maxLength)num = fs.Read(buffer, 0, Convert.ToInt32(leftLength));
                //读取剩余字节
                else num = fs.Read(buffer, 0, maxLength);
                if (num == 0) break;//读完
                fileStart += num;
                leftLength -= num;
                Response.BinaryWrite(buffer);
                Response.Flush();
            }
            //记录下载
            tb_LogInfo log = new tb_LogInfo
            {
                UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                FileName = "下载文件",
                Explain = "下载成功！",
                Time = DateTime.Now.ToString(),
                Operation = LogOperations.DownloadFile()
            };
            logInfoService.Add(log);
            fs.Close();
            Response.End();
        }
        private void AlertMsg(string msg)
        {
            Response.ContentType = "text/html";
            Response.Write("<script>alert('" + msg + "');</script>");
        }
        [AuthorityAuthentication]
        [HttpPost]//王军军增加8.23
        public bool delefeature()
        {
            var sr = new StreamReader(Request.InputStream);
            string stream = sr.ReadToEnd();
            sr.Close();
            bool tt1 = DeleteFile(u => u.ObjectID.Contains(stream.Trim()));
            return tt1;
        }
        public static string get_uft8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }
        /// <summary>
        /// 根据图形id删除图形
        /// </summary>
        /// <param name="objectid"></param>
        /// <returns></returns>
        private bool deleDWG(string objectid)
        {
            FeatureItem1 item1 = new FeatureItem1();
            item1.url = ConfigurationManager.AppSettings["serverurl"];
            bool tt = openauto.DeleFeature(item1.url, objectid);
            return tt;
        }
        [Authentication]
        [HttpPost]//王军军增加8.23
        public string GetUserinfo()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            tb_UserInfo user = userInfoService.Find(u => u.UserName == stream.Trim());
            string str1 = "";
            if (user != null)
            {
                str1 = str1 + user.Unit + ",";
                str1 = str1 + user.Levels;
            }
            return str1;
        }
        /// <summary>
        /// 获取用户ID
        /// </summary>
        /// <returns></returns>
        [Authentication]
        [HttpPost]
        public string GetUserID()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            tb_UserInfo user = userInfoService.Find(u => u.UserName == stream.Trim());
            if (user != null)
            {
                return user.ID.ToString();
            }
            return null;
        }
        [Authentication]
        [HttpPost]//王军军增加8.23
        public string getfileinfo()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            tb_FileInfo user = fileInfoService.Find(u => u.ObjectID.Contains(stream.Trim()));
            string tt=Newtonsoft.Json.JsonConvert.SerializeObject(user);
            return tt;
        }
        [Authentication]
        [HttpPost]//王军军增加8.23
        public string getimginfo()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            sr.Close();
            var baseurl = "http://" + Request.Url.Host+":"+Request.Url.Port;
            tb_FileInfo user = fileInfoService.Find(u => u.ObjectID.Contains(stream.Trim()));
            string path1 =user.Directory + "预览文件\\";
            DirectoryInfo dir = new DirectoryInfo(path1);
            var startindex = path1.IndexOf("\\Data\\File");
            var path2= path1.Substring(startindex);
            FileInfo[] inf = dir.GetFiles();
            var filename = "";
            foreach (FileInfo finf in inf)
            {
                if (finf.Extension.Equals(".jpg"))
                    //如果扩展名为“.xml”
                    path2.Replace("\\", "/");
                    filename = filename+ baseurl+ path2 + finf.Name + "%$%";
            }

            filename = filename.Substring(0, filename.Length - 3);
          
            return filename;
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
            }
            //完成时间信息
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
            featureItem2.Attributes.Add("PublicOB", fileInfo.PublicObjs);
            featureItem2.url = ConfigurationManager.AppSettings["serverurl"];
           
            bool tt1 = openauto.UpdateFeature(featureItem2.url, fileInfo.ObjectID, featureItem2);
            
            bool tt2 = fileInfoService.Update(fileInfo);//更新数据库
            return tt1 && tt2;
        }
        private bool AuthenLevel(string SurveyingUnitName)
        {
            var cook = System.Web.HttpContext.Current.Request.Cookies["username"];
            if (cook == null)
            {
                return false;
            }
            string username = cook.Value;
            tb_UserInfo user = userInfoService.Find(u => u.UserName == username);
            if (user == null)
            {
                return false;
            }
            else if (user.Levels == "0" || user.Unit == SurveyingUnitName)
            {
                return true;
            }
            return false;
        }
    }
}