/**************************************************
 *项目名称：BLL
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/7/31 19:35:22
 *更新时间：2017/7/31 19:35:22
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using IBLL;
using Model;
using System.Web;
using System.Web.ModelBinding;
using IDAL;
using DAL;
using System;
using BLL.Tools;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BLL
{
    public class UserInfoService:BaseService<tb_UserInfo>,InterfaceUserInfoService
    {
        public UserInfoService() : base(RepositoryFactory.UserInfoRepository)
        {
        }

        public tb_UserInfo FindUserInfoWithUserName(string username)
        {
            try
            {
                return Find(u => u.UserName == username);
            }
            catch(Exception e)
            {
                Log.AddRecord(e.Message);
                return null;
            }
        }
       
    }
}
