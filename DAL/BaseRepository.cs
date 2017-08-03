/**************************************************
 *项目名称：DAL
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/7/31 16:41:40
 *更新时间：2017/7/31 16:41:40
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using IDAL;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DAL
{
    public class BaseRepository<T>: InterfaceBaseRepository<T> where T:class
    {
        protected CZSRMS_DBEntities dbContext = DBContextFactory.GetCurrentContext();

        public T Add(T entity)
        {
            try
            {
                dbContext.Set<T>().Add(entity);
                dbContext.SaveChanges();
                return entity;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return dbContext.Set<T>().Count(predicate);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool Delete(T entity)
        {
            try
            {
                dbContext.Set<T>().Attach(entity);
                dbContext.Entry<T>(entity).State = System.Data.Entity.EntityState.Deleted;
                return dbContext.SaveChanges() > 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Exist(Expression<Func<T, bool>> anyLambda)
        {
            try
            {
                return dbContext.Set<T>().Any(anyLambda);
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public T Find(Expression<Func<T, bool>> whereLambda)
        {
            try
            {
                T _entity = dbContext.Set<T>().FirstOrDefault<T>(whereLambda);
                return _entity;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public IQueryable<T> FindList(Expression<Func<T, bool>> whereLamdba, string orderName, bool isAsc)
        {
            try
            {
                var _list = dbContext.Set<T>().Where(whereLamdba);
                _list = OrderBy(_list, orderName, isAsc);
                return _list;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public IQueryable<T> FindPageList(int pageIndex, int pageSize, out int totalRecord, Expression<Func<T, bool>> whereLamdba, string orderName, bool isAsc)
        {
            try
            {
                var _list = dbContext.Set<T>().Where<T>(whereLamdba);
                totalRecord = _list.Count();
                _list = OrderBy(_list, orderName, isAsc).Skip<T>((pageIndex - 1) * pageSize).Take<T>(pageSize);
                return _list;
                throw new NotImplementedException();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public bool Update(T entity)
        {
            try
            {
                dbContext.Set<T>().Attach(entity);
                dbContext.Entry<T>(entity).State = System.Data.Entity.EntityState.Modified;
                return dbContext.SaveChanges() > 0;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="source">原IQueryable</param>
        /// <param name="propertyName">排序属性名</param>
        /// <param name="isAsc">是否正序</param>
        /// <returns>排序后的IQueryable<T></returns>
        private IQueryable<T> OrderBy(IQueryable<T> source, string propertyName, bool isAsc)
        {
            if (source == null) throw new ArgumentNullException("source", "不能为空");
            if (string.IsNullOrEmpty(propertyName)) return source;
            var _parameter = Expression.Parameter(source.ElementType);
            var _property = Expression.Property(_parameter, propertyName);
            if (_property == null) throw new ArgumentNullException("propertyName", "属性不存在");
            var _lambda = Expression.Lambda(_property, _parameter);
            var _methodName = isAsc ? "OrderBy" : "OrderByDescending";
            var _resultExpression = Expression.Call(typeof(Queryable), _methodName, new Type[] { source.ElementType, _property.Type }, source.Expression, Expression.Quote(_lambda));
            return source.Provider.CreateQuery<T>(_resultExpression);
        }
    }
}
