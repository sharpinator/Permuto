using System.Web;
using System.Web.Mvc;

namespace Permuto.CxC.Exchange.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
