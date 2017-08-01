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

namespace BLL
{
    public class LogInfoService:BaseService<tb_LogInfo>,InterfaceLogInfoService
    {
        public LogInfoService() : base(RepositoryFactory.LogInfoRepository) 
        {

        }
        public tb_LogInfo Add(tb_LogInfo entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(tb_LogInfo entity)
        {
            throw new NotImplementedException();
        }

        public List<tb_LogInfo> FindLogListWithTime(string timeNow)
        {
            var info = CurrentRepository.FindList(u => u.Time.Contains(timeNow),"Time",false);
            List<tb_LogInfo> list = new List<tb_LogInfo>();
            foreach (tb_LogInfo item in info)
            {
                list.Add(item);
            }
            if (info.Count() > 0)
                list.Add(info.First());
            return list;
        }
        public List<tb_LogInfo> FinLogListWithOperation(string operation)
        {
            var info = CurrentRepository.FindList(u => operation.Contains(u.Operation), "Time", false);
            List<tb_LogInfo> list = new List<tb_LogInfo>();
            foreach (tb_LogInfo item in info)
            {
                list.Add(item);
            }
            return list;
        }

        public bool Update(tb_LogInfo entity)
        {
            throw new NotImplementedException();
        }
    }
}
