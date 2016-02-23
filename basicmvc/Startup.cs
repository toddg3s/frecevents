using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(basicmvc.Startup))]
namespace basicmvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
