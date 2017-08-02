/**************************************************
 *项目名称：BLL.Tools
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/8/2 22:02:30
 *更新时间：2017/8/2 22:02:30
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace BLL.Tools
{
    public static class Log
    {
        /// <summary>  
        /// 日志文件记录   
        /// </summary>  
        /// <param name="msg">内容</param>  
        public static void AddRecord(string msg)
        {
            string program = "郴州市测绘成果管理系统";
            string path = Path.Combine("./log");
            if (!Directory.Exists(path))//判断是否有该文件    
                Directory.CreateDirectory(path);//不存在则创建log文件夹  
            string info = path + "\\" + program;
            if (!Directory.Exists(info))
                Directory.CreateDirectory(info);
            string logFileName = path + "\\" + program + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";//生成日志文件  
            StreamWriter writer;
            try
            {
                writer = File.AppendText(logFileName);//文件中添加文件流  
                writer.WriteLine(DateTime.Now.ToString("HH:mm:ss") + " " + msg);
                writer.Flush();
                writer.Close();
            }
            catch (Exception e)
            {
                writer = File.AppendText(logFileName);
                writer.WriteLine(DateTime.Now.ToString("日志记录错误HH:mm:ss") + " " + e.Message + " " + msg);
                writer.Flush();
                writer.Close();
            }
        }
    }
}