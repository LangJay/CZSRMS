/**************************************************
 *项目名称：Model
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/8/1 11:32:58
 *更新时间：2017/8/1 11:32:58
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class PageInfo<T>where T :class
    {
        public int pageIndex { set; get; }
        public int pageSize {
            get
            {
                try
                {
                    return int.Parse(ConfigurationManager.AppSettings["pageSize"]);
                }
                catch
                {
                    return 10;
                }
            }
         }
        /// <summary>
        /// 总页数，记录数/pageSize
        /// </summary>
        public int totalPage { set; get; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int totalRecord { set; get; }
        public List<T> pageList { set; get; }
        /// <summary>
        /// 保存搜索的关键字
        /// </summary>
        public string keywords { set; get; }
    }
}
