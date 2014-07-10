using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StravaChallenge.Startup))]
namespace StravaChallenge
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
