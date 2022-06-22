using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(REA_Tracker.Startup))]
namespace REA_Tracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
