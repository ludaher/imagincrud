using ImaginCrud.Controllers;
using ImaginCrud.DataAccess;
using ImaginCrud.Entities;
using ImaginCrud.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace ImaginCrud
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Database.SetInitializer<ImaginCrudDataContext>(new DropCreateDatabaseIfModelChanges<ImaginCrudDataContext>());
            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Section, SectionModel>();
                cfg.CreateMap<Field, FieldModel>();
                cfg.CreateMap<Option, OptionModel>();
                cfg.CreateMap<SectionModel, Section>();
                cfg.CreateMap<FieldModel, Field>();
                cfg.CreateMap<OptionModel, Option>();
                cfg.CreateMap<FormData, FormDataHistory>();
                cfg.CreateMap<FormDataDetail, FormDataHistoryDetail>();
                cfg.CreateMap<FormDataHistory, FormData>();
                cfg.CreateMap<FormDataHistoryDetail, FormDataDetail>();
            });
        }
    }
}
