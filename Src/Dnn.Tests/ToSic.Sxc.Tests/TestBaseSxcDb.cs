using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Run;
using ToSic.Sxc.Tests.ServicesTests;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests
{
    public class TestBaseSxcDb: TestBaseDiEavFullAndDb
    {

        protected override IServiceCollection SetupServices(IServiceCollection services = null)
        {
            return base.SetupServices(services)
                .AddSxcCore()
                .AddTransient<IPlatformInfo, TestPlatformInfo>()
                ;
        }

    }
}
