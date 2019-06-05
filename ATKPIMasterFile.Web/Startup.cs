using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ATKPIMasterFile.Web.Startup))]
namespace ATKPIMasterFile.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
