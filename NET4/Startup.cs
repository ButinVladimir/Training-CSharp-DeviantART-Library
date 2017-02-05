using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NET4.Startup))]
namespace NET4
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
