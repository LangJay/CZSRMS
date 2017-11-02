/**************************************************
 *项目名称：IBLL
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/7/31 16:15:08
 *更新时间：2017/7/31 16:15:08
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/


using System.Collections.Generic;

namespace IBLL
{
    public interface InterfaceBaseService<T> where T:class
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns>添加后的数据实体</returns>
        T Add(T entity);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns>是否成功</returns>
        bool Update(T entity);

        bool UpdateMulti(List<T> list);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns>是否成功</returns>
        bool Delete(T entity);
    }
}
