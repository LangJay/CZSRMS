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
        /// 操作类型枚举，上传 = 1，下载 = 2,删除 = 3,创建用户 = 4,修改密码 = 5,重制密码 = 6,系统日志 = 7
        /// </summary

        private static string[] operations = new string[] { "上传", "下载", "删除", "创建用户", "修改密码", "重制密码", "系统日志" };
        public static string Upload()
        {
            return operations[0];
        }
        public static string Download()
        {
            return operations[1];
        }
        public static string Delete()
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
        public static string SystemLog()
        {
            return operations[6];
        }
    }
}