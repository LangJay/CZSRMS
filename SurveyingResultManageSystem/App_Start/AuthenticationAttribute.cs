using BLL;
using BLL.Tools;
using Model;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SurveyingResultManageSystem.App_Start
{
    public class AuthenticationAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = HttpContext.Current.Request.Cookies["username"];
            if (user == null)
                filterContext.Result = new RedirectResult("../Home/Login");
            base.OnActionExecuting(filterContext);
        }
    }
    public class AuthorityAuthenticationAttribute : ActionFilterAttribute
    {
        private UserInfoService userInfoService;
        public AuthorityAuthenticationAttribute()
        {
            userInfoService = new UserInfoService();
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                var cook = HttpContext.Current.Request.Cookies["username"];
                if(cook == null)
                {
                    filterContext.Result = new RedirectResult("../Home/Login");
                }
                string username = cook.Value;
                tb_UserInfo user = userInfoService.Find(u => u.UserName == username);
                if(user == null)
                {
                    filterContext.Result = new RedirectResult("../Home/Login");
                }
                else if(user.Levels == "1")
                {
                    filterContext.Result = new RedirectResult("../Home/Error");
                }
                base.OnActionExecuted(filterContext);
            }
            catch(Exception e)
            {
                Log.AddRecord(e);
            }
        }
    }
    /// <summary>
    /// 系统管理员不能进的地方
    /// </summary>
    public class MapAuthenticationAttribute : ActionFilterAttribute
    {
        private UserInfoService userInfoService;
        public MapAuthenticationAttribute()
        {
            userInfoService = new UserInfoService();
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                var cook = HttpContext.Current.Request.Cookies["username"];
                if (cook == null)
                {
                    filterContext.Result = new RedirectResult("../Home/Login");
                }
                string username = cook.Value;
                tb_UserInfo user = userInfoService.Find(u => u.UserName == username);
                if (user == null)
                {
                    filterContext.Result = new RedirectResult("../Home/Login");
                }
                else if (user.Levels == "-1")
                {
                    filterContext.Result = new RedirectResult("../Home/Error");
                }
                base.OnActionExecuted(filterContext);
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
            }
        }
    }
    /// <summary>
    /// 其他用户不能进的地方
    /// </summary>
    public class RecoverAuthenticationAttribute : ActionFilterAttribute
    {
        private UserInfoService userInfoService;
        public RecoverAuthenticationAttribute()
        {
            userInfoService = new UserInfoService();
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                var cook = HttpContext.Current.Request.Cookies["username"];
                if (cook == null)
                {
                    filterContext.Result = new RedirectResult("../Home/Login");
                }
                string username = cook.Value;
                tb_UserInfo user = userInfoService.Find(u => u.UserName == username);
                if (user == null)
                {
                    filterContext.Result = new RedirectResult("../Home/Login");
                }
                else if (user.Levels != "-1")
                {
                    filterContext.Result = new RedirectResult("../Home/Error");
                }
                base.OnActionExecuted(filterContext);
            }
            catch (Exception e)
            {
                Log.AddRecord(e);
            }
        }
    }

}