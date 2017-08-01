/**************************************************
 *项目名称：DAL
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/8/1 7:54:45
 *更新时间：2017/8/1 7:54:45
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public static class RepositoryFactory
    {
        public static InterfaceUserInfoRepository UserInfoRepository
        {
            get
            {
                return new UserInfoRepository();
            }
        }
        public static InterfaceLogInfoRepository LogInfoRepository
        {
            get
            {
                return new LogInfoRepository();
            }
        }
        public static InterfaceFileInfoRepository FileInfoRepository
        {
            get
            {
                return new FileInfoRepository();
            }
        }
    }
}
