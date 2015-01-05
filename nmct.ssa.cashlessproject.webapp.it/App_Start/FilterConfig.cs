using System.Web;
using System.Web.Mvc;

namespace nmct.ssa.cashlessproject.webapp.it
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
