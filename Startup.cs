using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(D40.Startup))]
namespace D40
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
