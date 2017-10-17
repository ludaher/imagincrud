using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ImaginCrud.Startup))]
namespace ImaginCrud
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
