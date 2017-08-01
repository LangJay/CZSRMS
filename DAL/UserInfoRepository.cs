/**************************************************
 *项目名称：DAL
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/7/31 17:02:52
 *更新时间：2017/7/31 17:02:52
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL;
using Model;

namespace DAL
{
    class UserInfoRepository:BaseRepository<tb_UserInfo>, InterfaceUserInfoRepository
    {
    }
}
