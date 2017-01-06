using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NewEnglandBeerMapApplication.Startup))]
namespace NewEnglandBeerMapApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
