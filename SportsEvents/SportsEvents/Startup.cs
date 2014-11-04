using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SportsEvents.Startup))]
namespace SportsEvents
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
