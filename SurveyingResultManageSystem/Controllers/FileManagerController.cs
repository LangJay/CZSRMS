
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
        public FileManagerController()
        {
            fileInfoService = new FileInfoService();
        }

        [Authentication]
        [HttpPost]
        public ActionResult UpLoadFile()
        {
            var sr = new StreamReader(Request.InputStream);
            var stream = sr.ReadToEnd();
            tb_FileInfo obj = JsonConvert.DeserializeObject<tb_FileInfo>(stream) as tb_FileInfo;
            obj.Directory = HttpRuntime.AppDomainAppPath.ToString() + "/Data/" + obj.FileName;
            if (fileInfoService.Add(obj) != null)
            {
                LogInfoService logInfoService = new LogInfoService();
                tb_LogInfo log = new tb_LogInfo()
                {
                    UserName = System.Web.HttpContext.Current.Request.Cookies["username"].Value,
                    Time = DateTime.Now.ToString(),
                    Operation = LogOperations.UploadFile(),
                    FileName = obj.FileName,
                    Explain = obj.Explain
                };
                logInfoService.Add(log);
                return Content("上传成功！");
            }
            else
            {
                return Content("上传失败！");
            }
        }
        [Authentication]
        [HttpPost]
        public ActionResult Upload()
        {
            var files = Request.Files;
            if (files.Count > 0)
            {
                var file = files[0];
                string fileName = file.FileName;
                Stream inputStream = file.InputStream;
                byte[] buffer = new byte[inputStream.Length];
                inputStream.Read(buffer, 0, buffer.Length);
                //string strFileMd5 = MD5Helper.GetMD5FromFile(buffer);
                string basePath = HttpRuntime.AppDomainAppPath.ToString() + "/Data/";
                string fileSavePath = Path.Combine(basePath, fileName);
                file.SaveAs(fileSavePath);
            }
            return Content("上传成功！");
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
        [HttpGet]
        public string GetFileInfoByCategory(string category,int ? pageIndex,string keywords)
        {
            string result = "";
            PageInfo<tb_FileInfo> pageInfo = new PageInfo<tb_FileInfo>()
            {
                pageIndex = pageIndex ?? 1
            };
            int totalRecord;
            string username = System.Web.HttpContext.Current.Request.Cookies["username"].Value;
            pageInfo.pageList = fileInfoService.FindPageList(pageInfo.pageIndex, pageInfo.pageSize, out totalRecord,
                f => f.PublicObjs.Contains(username), "ID", false);
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
            pageInfo.totalRecord = totalRecord;
            double res = (totalRecord / 1.0) / pageInfo.pageSize;
            pageInfo.totalPage = (int)Math.Ceiling(res);
            result = JsonConvert.SerializeObject(pageInfo);
            return result;
        }
    }
}