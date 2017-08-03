/**************************************************
 *项目名称：BLL
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/7/31 17:31:50
 *更新时间：2017/7/31 17:31:50
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using IBLL;
using Model;
using System.Collections.Generic;
using System.Linq;
using System;
using DAL;
using System.Linq.Expressions;
using BLL.Tools;

namespace BLL
{
    public class LogInfoService:BaseService<tb_LogInfo>,InterfaceLogInfoService
    {
        public LogInfoService() : base(RepositoryFactory.LogInfoRepository) 
        {

        }
        public List<tb_LogInfo> FindAll()
        {
            List<tb_LogInfo> list = new List<tb_LogInfo>();
            try
            {
                var info = CurrentRepository.FindList(u => u.Time.Contains(""), "Time", false);
                foreach (tb_LogInfo item in info)
                {
                    list.Add(item);
                }
                return list;

            }
            catch (Exception e)
            {
                Log.AddRecord(e.Message);
                return list;
            }
        }
        /// <summary>
        /// 循环加了第一条记录
        /// </summary>
        /// <param name="timeNow"></param>
        /// <returns></returns>
        public List<tb_LogInfo> FindLogListWithTime(string timeNow)
        {
            List<tb_LogInfo> list = new List<tb_LogInfo>();
            try
            {
                var info = CurrentRepository.FindList(u => u.Time.Contains(timeNow), "Time", false);
                foreach (tb_LogInfo item in info)
                {
                    list.Add(item);
                }
                if (info.Count() > 0)
                    list.Add(info.First());
                return list;

            }
            catch(Exception e)
            {
                Log.AddRecord(e.Message);
                return list;
            }
        }
        public List<tb_LogInfo> FinLogListWithOperation(string operation)
        {
            List<tb_LogInfo> list = new List<tb_LogInfo>();
            try
            {
                var info = CurrentRepository.FindList(u => operation.Contains(u.Operation), "Time", false);
                foreach (tb_LogInfo item in info)
                {
                    list.Add(item);
                }
                return list;
            }
            catch(Exception e)
            {
                Log.AddRecord(e.Message);
                return list;
            }
        }
        public List<tb_LogInfo> FindPageList(int pageIndex,int pageSize,out int totalRecord, Expression<Func<tb_LogInfo, bool>> whereLamdba, string orderName, bool isAsc)
        {
            List<tb_LogInfo> list = new List<tb_LogInfo>();
            try
            {
                var info = CurrentRepository.FindPageList(pageIndex, pageSize, out int total, whereLamdba, orderName, isAsc);
                foreach (tb_LogInfo item in info)
                {
                    list.Add(item);
                }
                totalRecord = total;
                return list;
            }
            catch(Exception e)
            {
                Log.AddRecord(e.Message);
                totalRecord = 0;
                return list;
            }
        }
    }
}
