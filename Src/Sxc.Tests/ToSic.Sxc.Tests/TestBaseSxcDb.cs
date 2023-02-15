using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Startup;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests
{
    public class TestBaseSxcDb: TestBaseDiEavFullAndDb
    {
        //protected override void Configure()
        //{
        //    var sxcStartup = Build<SxcSystemLoader>();
        //    sxcStartup.PreStartUp();
        //    base.Configure();
        //}

        protected override void SetupServices(IServiceCollection services)
        {
            base.SetupServices(services);
            services
                .AddSxcCore();
        }

    }
}
