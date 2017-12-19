using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BCS.FW.Startup))]
namespace BCS.FW
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
