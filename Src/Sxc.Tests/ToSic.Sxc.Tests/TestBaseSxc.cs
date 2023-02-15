using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Startup;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests
{
    public class TestBaseSxc: TestBaseEavCore
    {
        protected override void SetupServices(IServiceCollection services)
        {
            base.SetupServices(services);
            services
                .AddSxcCore();
        }

    }
}
