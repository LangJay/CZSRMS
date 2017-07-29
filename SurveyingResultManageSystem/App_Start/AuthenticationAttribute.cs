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
                filterContext.Result = new RedirectResult("/Home/Login");
            base.OnActionExecuting(filterContext);
        }
    }
}