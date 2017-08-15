/**************************************************
 *项目名称：Model
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/8/14 10:01:40
 *更新时间：2017/8/14 10:01:40
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Category
    {
        public string kind { set; get; }
        public string select { set; get; }
        public List<string> nodes { set; get; } = new List<string>();
    }
}
