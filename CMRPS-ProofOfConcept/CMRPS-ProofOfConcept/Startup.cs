using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CMRPS_ProofOfConcept.Startup))]
namespace CMRPS_ProofOfConcept
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
