using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DMDProject.Startup))]
namespace DMDProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
