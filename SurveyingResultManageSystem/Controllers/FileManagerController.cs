
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
using BLL;
using System.Web;
using SurveyingResultManageSystem.Models;
using System.Net.Mail;
using System.Text;
using FilePackageLib;

namespace SurveyingResultManageSystem.Controllers
{
    public class FileManagerController : Controller
    {
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
                string fileSaveFolder = HttpRuntime.AppDomainAppPath.ToString() + "Data\\File\\";
                //如果目标不存在，则创建
                //if (!Directory.Exists(fileSaveFolder))
                //{
                //    Directory.CreateDirectory(fileSaveFolder);
                //}
                //string md5 = GetMD5HashFromFile(file.InputStream);
                //if(md5 == null)//不允许上传
                //{
                //    tb_LogInfo log = new tb_LogInfo()
                //    {
                //        UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                //        Time = DateTime.Now.ToString(),
                //        Operation = LogOperations.UploadFile(),
                //        FileName = fileName,
                //        Explain = "文件格式不正确！"
                //    };
                //    logInfoService.Add(log);
                //    return "文件格式不正确！";
                //}
                ////检查数据库是否存在该文件，若是，则不允许上传
                //tb_FileInfo oldFile = fileInfoService.Find(u => u.MD5 == md5);
                //if (oldFile != null )//
                //{
                //    tb_LogInfo log = new tb_LogInfo()
                //    {
                //        UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                //        Time = DateTime.Now.ToString(),
                //        Operation = LogOperations.UploadFile(),
                //        FileName = fileName,
                //        Explain = "已经存在该文件！"
                //    };
                //    logInfoService.Add(log);
                //    return "已经存在该文件！";
                //}
                string fileSavePath = Path.Combine(fileSaveFolder, fileName);
                file.SaveAs(fileSavePath);
                //解压该文件
                string zipFileSavePath = Path.Combine(fileSaveFolder, DateTime.Now.ToFileTime().ToString());
                //创建该目录i
                Directory.CreateDirectory(zipFileSavePath);
                FileCompressExtend fce = new FileCompressExtend();
                fce.DecompressDirectory(fileSavePath, zipFileSavePath);
                //赋值模型
                tb_FileInfo fileInfo = JsonConvert.DeserializeObject<tb_FileInfo>(json) as tb_FileInfo;
                 fileInfo.Directory = fileSavePath;//重新赋值路径
                fileInfo.FileName = fileName;
                //fileInfo.MD5 = md5;
                var username = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
                fileInfo.UserID = userInfoService.Find(u => u.UserName == username).ID;
                fileInfo.UploadTime = DateTime.Now.ToString();
                //fileInfo.FileSize =  / 1024.00 / 1024.00;
                if (fileInfo.PublicObjs != null)
                {
                    fileInfo.PublicObjs = fileInfo.PublicObjs.Replace(",", "|");
                }
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
                    return "上传失败，写入数据库出错！";
                }
            }
            return "上传失败！";
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
    }
}