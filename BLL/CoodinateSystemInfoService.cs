﻿/**************************************************
 *项目名称：BLL
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/8/4 8:40:04
 *更新时间：2017/8/4 8:40:04
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBLL;
using IDAL;
using Model;
using DAL;

namespace BLL
{
    public class CoodinateSystemInfoService : BaseService<tb_CoodinateSystem>, InterfaceCoodinateSystemInfoService
    {
        public CoodinateSystemInfoService() : base(RepositoryFactory.CoodinateSystemRepository)
        {
        }
    }
}
