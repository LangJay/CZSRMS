/**************************************************
 *项目名称：BLL
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/7/31 17:14:52
 *更新时间：2017/7/31 17:14:52
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using IBLL;
using IDAL;
using System;
using System.Linq.Expressions;

namespace BLL
{
    public abstract class BaseService<T> : InterfaceBaseService<T> where T : class
    {
        protected InterfaceBaseRepository<T> CurrentRepository { get; set; }
        public BaseService(InterfaceBaseRepository<T> currentRepository)
        {
            CurrentRepository = currentRepository;
        }
        public T Add(T entity)
        {
            return CurrentRepository.Add(entity);
        }

        public bool Update(T entity)
        {
            return CurrentRepository.Update(entity);
        }

        public bool Delete(T entity)
        {
            return CurrentRepository.Delete(entity);
        }
        public T Find(Expression<Func<T, bool>> whereLamdba)
        {
            return CurrentRepository.Find(whereLamdba);
        }
        
    }
}
