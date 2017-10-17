using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ImaginCrudWebSite.Startup))]
namespace ImaginCrudWebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
