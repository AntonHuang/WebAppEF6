using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebAppEF6.Startup))]
namespace WebAppEF6
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
