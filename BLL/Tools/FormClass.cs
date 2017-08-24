/**************************************************
 *项目名称：BLL.Tools
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/8/21 17:54:41
 *更新时间：2017/8/21 17:54:41
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Tools
{
    public static class FormClass
    {
        /// <summary>
        /// 把Form Post过来的表单集合转换成对象 ，仿 MVC post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T FormToClass<T>(NameValueCollection collection)
        {
            T t = Activator.CreateInstance<T>();
            PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo _target in properties)
            {
                if (_target.CanWrite)
                {
                    var obj = collection[_target.Name];
                    if (obj != null && obj != "")
                    {
                        Type type = _target.PropertyType;
                        _target.SetValue(t, Convert.ChangeType(obj, (Nullable.GetUnderlyingType(type) ?? type)));
                    }
                   
                }
            }
            return t;
        }
    }
}
