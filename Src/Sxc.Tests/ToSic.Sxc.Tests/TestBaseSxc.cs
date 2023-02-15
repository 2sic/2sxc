using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.StartUp;
using ToSic.Lib;
using ToSic.Sxc.Startup;
using ToSic.Testing.Shared;

namespace ToSic.Sxc.Tests
{
    public class TestBaseSxc: TestBaseEav
    {
        public TestBaseSxc() : base(new TestConfiguration() {AddAllEavServices = false})
        { }

        protected override void SetupServices(IServiceCollection services)
        {
            base.SetupServices(services);
            services
                .AddSxcCore()
                .AddEavCore()
                .AddEavCoreFallbackServices()
                .AddLibCore();
        }

    }
}
