using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SurveyingResultManageSystem.Models
{
    public class LogOperations
    {
        /// <summary>
        /// 操作类型枚举，上传 = 1，下载 = 2,删除 = 3,创建用户 = 4,修改密码 = 5,重制密码 = 6,系统日志 = 7
        /// </summary>
        public enum Operations
        {
            上传 = 1,
            下载 = 2,
            删除 = 3,
            创建用户 = 4,
            修改密码 = 5,
            重制密码 = 6,
            系统日志 = 7
        }
    }
}