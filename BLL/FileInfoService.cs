/**************************************************
 *项目名称：BLL
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/8/1 8:06:00
 *更新时间：2017/8/1 8:06:00
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using IBLL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using DAL;
using BLL.Tools;
using System.Linq.Expressions;

namespace BLL
{
    public class FileInfoService : BaseService<tb_FileInfo>, InterfaceFileInfoService
    {
        public FileInfoService() : base(RepositoryFactory.FileInfoRepository)
        {

        }
        public new List<tb_FileInfo> FindPageList(int pageIndex, int pageSize, out int totalRecord, Expression<Func<tb_FileInfo, bool>> whereLamdba, string orderName, bool isAsc)
        {
            List<tb_FileInfo> list = new List<tb_FileInfo>();
            try
            {
                var info = CurrentRepository.FindPageList(pageIndex, pageSize, out totalRecord, whereLamdba, orderName, isAsc);
                foreach (tb_FileInfo item in info)
                {
                    list.Add(item);
                }
                return list;
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                totalRecord = 0;
                return list;
            }
        }
        public new List<tb_FileInfo> FindAll(Expression<Func<tb_FileInfo, bool>> where, string orderName, bool isArsc)
        {
            List<tb_FileInfo> list = new List<tb_FileInfo>();
            try
            {
                var info = CurrentRepository.FindList(where, orderName, isArsc);
                foreach (tb_FileInfo item in info)
                {
                    list.Add(item);
                }
                return list;
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
                return list;
            }
        }
    }
}
