using ImaginCrud.Controllers;
using System.Web;
using System.Web.Mvc;

namespace ImaginCrud
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MyErrorHandler());
        }
    }
}
