using SurveyingResultManageSystem.App_Start;
using System.Web;
using System.Web.Mvc;

namespace SurveyingResultManageSystem
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
