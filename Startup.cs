using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TravelNotification.Startup))]
namespace TravelNotification
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
