using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(AgentHub.Web.Startup))]

namespace AgentHub.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
