using ImaginCrud.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImaginCrud.Controllers
{
    public class MyErrorHandler : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
                return;
            AppLogger.Logger.Error("Error no controlado: " + filterContext.Exception.ToString());

        }

    }
}