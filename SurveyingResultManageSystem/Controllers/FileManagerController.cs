using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Model;
using SurveyingResultManageSystem.App_Start;
using BLL.Tools;
using System;
using BLL;
using System.Web;
using SurveyingResultManageSystem.Models;
using System.Text;
using FilePackageLib;
using ArcServer;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Transactions;

namespace SurveyingResultManageSystem.Controllers
{
    public class FileManagerController : Controller
    {
        /// <summary>
        /// 排序字段，默认Finishtime
        /// </summary>
        private string ORDER_NAME = "Finishtime";
        /// <summary>
        /// 是否倒叙
        /// </summary>
        private bool DESC = false;
        private FileInfoService fileInfoService;
        private UserInfoService userInfoService;
        private LogInfoService logInfoService;
        public FileManagerController()
        {
            userInfoService = new UserInfoService();
            fileInfoService = new FileInfoService();
            logInfoService = new LogInfoService();
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
        [Authentication]
        [HttpPost]
        public string UpLoadFile()
        {
            var files = Request.Files;
            var json = Request.Form["fileinfo"];
            if (files.Count > 0)
            {
                var file = files[0];
                string fileName = file.FileName.Split('\\').Last();
                string fileSaveFolder = HttpRuntime.AppDomainAppPath.ToString() + "Data\\File\\" + DateTime.Now.ToFileTime().ToString() + "\\";
                tb_FileInfo fileInfo = new tb_FileInfo();
                string fileSavePath = null;//带文件名路径
                //读取文件并保存
                try
                {
                    if (!Directory.Exists(fileSaveFolder)) Directory.CreateDirectory(fileSaveFolder);
                    fileInfo = JsonConvert.DeserializeObject<tb_FileInfo>(json) as tb_FileInfo;
                    fileSavePath = Path.Combine(fileSaveFolder, fileName);
                    file.SaveAs(fileSavePath);
                }
                catch
                {
                    return "读取文件错误！";
                }
                try
                {
                    //解压该文件
                    // string zipFileSavePath = Path.Combine(fileSaveFolder, fileInfo.ProjectName);//解压到该目录
                    //创建该目录i
                    Directory.CreateDirectory(fileSaveFolder);
                    FileCompressExtend fce = new FileCompressExtend();
                    fce.DecompressDirectory(fileSavePath, fileSaveFolder);
                }
                catch
                {
                    return "文件格式不正确！";
                }
                //赋值模型
                fileInfo.Directory = fileSaveFolder;//重新赋值路径
                fileInfo.FileName = fileName;
                //fileInfo.MD5 = md5;
                var username = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
                tb_UserInfo user = userInfoService.Find(u => u.UserName == username);
                fileInfo.UserID = user.ID;
                fileInfo.UploadTime = DateTime.Now.ToString();
                //fileInfo.FileSize =  / 1024.00 / 1024.00;
                //若没有本身测绘队，则应该增加
                if (!fileInfo.PublicObjs.Contains(user.Unit))
                {
                    if (!string.IsNullOrEmpty(fileInfo.PublicObjs))
                    {
                        fileInfo.PublicObjs += "|" + user.Unit;
                    }
                    else
                    {
                        fileInfo.PublicObjs = user.Unit;
                    }
                }
                if (!string.IsNullOrEmpty(fileInfo.PublicObjs))
                {
                    fileInfo.PublicObjs = fileInfo.PublicObjs.Replace(",", "|");
                }
                //发布地图
                string upObjectId = PublishMap(fileInfo);
                if (upObjectId != null)
                {
                    //写入数据库
                    fileInfo.WasDeleted = false;
                    if (fileInfoService.Add(fileInfo) != null)
                    {
                        tb_LogInfo log = new tb_LogInfo()
                        {
                            UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                            Time = DateTime.Now.ToString(),
                            Operation = LogOperations.UploadFile(),
                            FileName = fileInfo.FileName,
                            Explain = "上传成功！"
                        };
                        logInfoService.Add(log);

                        return "上传成功！";
                    }
                    else
                    {
                        tb_LogInfo log = new tb_LogInfo()
                        {
                            UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                            Time = DateTime.Now.ToString(),
                            Operation = LogOperations.UploadFile(),
                            FileName = fileInfo.FileName,
                            Explain = "上传失败，写入数据库出错！"
                        };
                        logInfoService.Add(log);
                        //删除文件
                        if (Directory.Exists(fileSaveFolder))
                        {
                            Directory.Delete(fileSaveFolder, true);
                        }
                        return "上传失败，写入数据库出错！";
                    }
                }
                else
                {
                    tb_LogInfo log = new tb_LogInfo()
                    {
                        UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                        Time = DateTime.Now.ToString(),
                        Operation = LogOperations.UploadFile(),
                        FileName = fileInfo.FileName,
                        Explain = "上传失败，写入图形出错！"
                    };
                    logInfoService.Add(log);
                    //删除文件
                    if (Directory.Exists(fileSaveFolder))
                    {
                        Directory.Delete(fileSaveFolder, true);
                    }
                    return "上传失败，写入图形出错！";
                }

            }
            return "上传失败！";
        }
        /// <summary>
        /// 王军军 发布地图
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private string PublishMap(tb_FileInfo fileInfo)
        {
            var path1 = fileInfo.Directory + "范围文件\\";
            string upObjectId = null;
            DirectoryInfo dir = new DirectoryInfo(path1);
            var filename = "";
            if (Directory.Exists(path1))
            {
                FileInfo[] inf = dir.GetFiles();
                foreach (FileInfo finf in inf)
                {
                    if (finf.Extension.Equals(".shp"))
                        //如果扩展名为“.xml”
                        filename = finf.FullName;
                    //读取文件的完整目录和文件名
                }
            }
            if (filename != "")
            {

                //  path1 = path1 + filename;
                FeatureItem1 fi2 = new FeatureItem1();
                fi2.Attributes = new Dictionary<string, object>();
                fi2.Attributes.Add("FileName", fileInfo.FileName);//文件名
                fi2.Attributes.Add("Directory", fileInfo.Directory);//文件路径
                fi2.Attributes.Add("CoodSystem", fileInfo.CoodinateSystem);//坐标框架信息
                if (fileInfo.Finishtime.Trim() != "")
                {
                    fi2.Attributes.Add("FinishTime", fileInfo.Finishtime);
                }//完成时间信息
                fi2.Attributes.Add("FshPerson", fileInfo.FinishPerson);//完成人信息
                fi2.Attributes.Add("ObjectNum", fileInfo.ObjectNum);//文件中对象数量
                fi2.Attributes.Add("MinCood", fileInfo.MinCoodinate);//最小坐标
                fi2.Attributes.Add("MaxCood", fileInfo.MaxCoodinate);//最大坐标
                fi2.Attributes.Add("Mark", fileInfo.Mark);// 备注信息
                fi2.Attributes.Add("ProName", fileInfo.ProjectName);// 所属项目名称
                fi2.Attributes.Add("FileType", fileInfo.FileType);// 文件类型，宗地图、供地红线图、报批红线图、地籍图、勘测定界报告、竣工验收测绘报告
                fi2.Attributes.Add("ProType", fileInfo.ProjectType);// 所属项目类型，供地、报批、竣工验收
                fi2.Attributes.Add("CenterMed", fileInfo.CenterMeridian);// 中央子午线
                fi2.Attributes.Add("Yoffset", fileInfo.Yoffset);// 纵坐标偏移值
                fi2.Attributes.Add("Xoffset", fileInfo.Xoffset);// 水平坐标偏移值
                fi2.Attributes.Add("SUnitName", fileInfo.SurveyingUnitName);// 测绘单位名称，北湖区测绘队、苏仙区测绘队、市局测绘队
                fi2.Attributes.Add("Memo", fileInfo.Explain);// 成果说明
                if (fileInfo.UploadTime.Trim() != "")
                {
                    fi2.Attributes.Add("UploadTime", fileInfo.UploadTime);
                }// 上传时间
                fi2.Attributes.Add("FileSize", fileInfo.FileSize);// 文件大小，单位M
                fi2.Attributes.Add("UserID", fileInfo.UserID);// 用户ID
                fi2.Attributes.Add("PublicOB", fileInfo.PublicObjs); // 公开单位
                fi2.url = ConfigurationManager.AppSettings["serverurl"];
                upObjectId = openauto.readshpfile(filename, fi2);
                fileInfo.ObjectID = upObjectId;
            }
            return upObjectId;
        }
        private string GetMD5HashFromFile(Stream stream)
        {
            try
            {
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(stream);
                stream.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Log.AddRecord(ex);
                return null;
            }
        }
        /// <summary>
        /// 系统管理员恢复数据分页
        /// </summary>
        /// <returns></returns>
        public PartialViewResult FileRecover(int? pageIndex)
        {

            return PartialView(getPartialModel(pageIndex));
        }
        /// <summary>
        /// 获取分页model
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        /// <returns></returns>
        private PageInfo<tb_FileInfo> getPartialModel(int? pageIndex)
        {
            //分类分页数据
            PageInfo<tb_FileInfo> pageInfo = new PageInfo<tb_FileInfo>()
            {
                pageIndex = pageIndex ?? 1
            };
            int totalRecord;
            pageInfo.pageList = fileInfoService.FindPageList(pageInfo.pageIndex, pageInfo.pageSize, out totalRecord,
                f => f.WasDeleted == true, ORDER_NAME, DESC);
            pageInfo.totalRecord = totalRecord;
            double res = (totalRecord / 1.0) / pageInfo.pageSize;
            pageInfo.totalPage = (int)Math.Ceiling(res);
            return pageInfo;
        }
        [RecoverAuthentication]
        public ActionResult FileRecoverView()
        {
            return View();
        }
        [Authentication]
        public void Recover(int id)
        {
            //根据id查找数据库记录
            tb_FileInfo file = fileInfoService.Find(u => u.ID == id);
            using (TransactionScope tran = new TransactionScope())
            {
                //找到原来的图形，重新上载到地图
                string objectId = PublishMap(file);
                //数据库标识字段WasDelete恢复false
                file.WasDeleted = false;
                bool success = fileInfoService.Update(file);
                if(objectId != null && success)
                {
                    var response = new { result = "恢复成功！", fileId = id };
                    Response.Write(new JavaScriptSerializer().Serialize(response));
                    tran.Complete();
                }
                else
                {
                    var response = new { result = "恢复失败！", fileId = id };
                    Response.Write(new JavaScriptSerializer().Serialize(response));
                }
            }
        }
        [Authentication]
        public void Delete(int id)
        {
            //根据id查找数据库记录
            tb_FileInfo file = fileInfoService.Find(u => u.ID == id);
            //删除数据库
            bool success = fileInfoService.Delete(file);
            if (success)
            {
                var response = new { result = "删除成功！", fileId = id };
                //文件彻底删除
                try
                {
                    DirectoryInfo dreInfo = new DirectoryInfo(file.Directory);
                    dreInfo.Delete(true);
                    Response.Write(new JavaScriptSerializer().Serialize(response));
                }
                catch (Exception e)
                {
                    Log.AddRecord(e);
                    Response.Write(new JavaScriptSerializer().Serialize(response));
                }
            }
            else
            {

                var response = new { result = "删除失败！", fileId = id };
                Response.Write(new JavaScriptSerializer().Serialize(response));
            }
        }
       
    }
}