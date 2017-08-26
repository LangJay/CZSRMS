
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
               // Stream inputStream = file.InputStream;
                string fileSaveFolder = HttpRuntime.AppDomainAppPath.ToString() + "/Data/File/";
                //如果目标不存在，则创建
                if (!Directory.Exists(fileSaveFolder))
                {
                    Directory.CreateDirectory(fileSaveFolder);
                }
                //int lenth = 1024;
                //for(int index = 0;index < inputStream.Length/lenth + 1;index ++)
                //{
                //    byte[] buffer = new byte[lenth];
                //    inputStream.Read(buffer,)
                //}
                //inputStream.Read(buffer, 0, buffer.Length);
                //string strFileMd5 = MD5Helper.GetMD5FromFile(buffer);
                string fileSavePath = Path.Combine(fileSaveFolder, fileName);
                file.SaveAs(fileSavePath);
                //inputStream.Close();
                //tb_FileInfo fileInfo = FormClass.FormToClass<tb_FileInfo>(Request.Form);
                tb_FileInfo fileInfo = JsonConvert.DeserializeObject<tb_FileInfo>(json) as tb_FileInfo;
                fileInfo.Directory = fileSavePath;
                fileInfo.FileName = fileName;
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
                    var response = new {fileId = fileInfo.ID };
                    Response.Write(new JavaScriptSerializer().Serialize(response));
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
                        Explain = "上传失败！"
                    };
                    logInfoService.Add(log);
                    return "上传失败！";
                }
            }
            return "上传失败！";
        }
    }
}