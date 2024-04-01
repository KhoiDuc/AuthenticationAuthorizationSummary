using System.Web;
using System.Web.Mvc;

namespace OktaNet4SSOLogin
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
