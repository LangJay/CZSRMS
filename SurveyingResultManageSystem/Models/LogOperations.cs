/**************************************************
 *项目名称：SurveyingResultManageSystem.Models
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/8/2 22:12:29
 *更新时间：2017/8/2 22:12:29
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SurveyingResultManageSystem.Models
{
    public static class LogOperations
    {
        /// <summary>
        /// 操作类型枚举，上传文件 = 1，下载文件 = 2,删除文件 = 3,创建用户 = 4,修改密码 = 5,重制密码 = 6,删除用户 = 7,系统日志 = 8
        /// </summary

        private static string[] operations = new string[] { "上传文件", "下载文件", "删除文件", "创建用户", "修改密码", "重制密码","删除用户","系统日志" };
        public static string UploadFile()
        {
            return operations[0];
        }
        public static string DownloadFile()
        {
            return operations[1];
        }
        public static string DeleteFile()
        {
            return operations[2];
        }
        public static string CreateUser()
        {
            return operations[3];
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public static string SetPasswords()
        {
            return operations[4];
        }
        /// <summary>
        /// 重制密码
        /// </summary>
        /// <returns></returns>
        public static string ResetPasswords()
        {
            return operations[5];
        }
        public static string DeleteUser()
        {
            return operations[6];
        }
        public static string SystemLog()
        {
            return operations[7];
        }
    }
}