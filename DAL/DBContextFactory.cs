/**************************************************
 *项目名称：DAL
 *作者：周继良
 *单位：湖南省第一测绘院
 *CLR版本：4.0.30319.42000
 *创建时间：2017/7/31 16:39:19
 *更新时间：2017/7/31 16:39:19
 **************************************************
 * Copyright @ 周继良 2017. All rights reserved.
 **************************************************/

using Model;
using System.Runtime.Remoting.Messaging;

namespace DAL
{
    public class DBContextFactory
    {
        public static CZSRMS_DBEntities GetCurrentContext()
        {
            CZSRMS_DBEntities _nContext = CallContext.GetData("CZSRMS_DBEntities") as CZSRMS_DBEntities;
            if (_nContext == null)
            {
                _nContext = new CZSRMS_DBEntities();
                CallContext.SetData("CZSRMS_DBEntities", _nContext);
            }
            return _nContext;
        }
    }
}
