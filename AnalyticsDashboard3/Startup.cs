using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AnalyticsDashboard3.Startup))]
namespace AnalyticsDashboard3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
