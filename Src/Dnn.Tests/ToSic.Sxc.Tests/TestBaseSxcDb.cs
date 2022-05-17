using Microsoft.Extensions.DependencyInjection;
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

        protected override IServiceCollection SetupServices(IServiceCollection services)
        {
            return base.SetupServices(services)
                .AddSxcCore();
        }

    }
}
