using IDAL;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DAL
{
    internal class FileInfoRepository :BaseRepository<tb_FileInfo>, InterfaceFileInfoRepository
    {
        new public bool Delete(tb_FileInfo entity)
        {
            try
            {
                entity.WasDeleted = true;//tb_FileInfo的删除就是把这个字段改成True；
                dbContext.Set<tb_FileInfo>().Attach(entity);
                dbContext.Entry<tb_FileInfo>(entity).State = System.Data.Entity.EntityState.Modified;
                return dbContext.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}