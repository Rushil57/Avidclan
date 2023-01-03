using System.Web;
using System.Web.Mvc;

namespace Avidclan_BlogsVacancy
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
