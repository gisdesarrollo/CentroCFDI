using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(APBox.Startup))]
namespace APBox
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
