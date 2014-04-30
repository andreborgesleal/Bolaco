using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bolaco.Startup))]
namespace Bolaco
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
