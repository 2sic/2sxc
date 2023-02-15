using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Tests.Mocks;
using ToSic.Eav.Configuration;
using ToSic.Eav.Data;
using ToSic.Eav.Persistence.File;
using ToSic.Eav.Run;
using ToSic.Testing.Shared;
using ToSic.Testing.Shared.Mocks;

namespace ToSic.Eav.ImportExport.Tests.FeatureTests
{
    public class FeatureTestsBase: TestBaseEav
    {
        public FeatureTestsBase()
        {
            // Make sure that features are ready to use
            var sysLoader = Build<EavSystemLoader>();
            sysLoader.ReloadFeatures();
        }

        protected override void SetupServices(IServiceCollection services)
        {
            base.SetupServices(services);
            services.AddTransient<IRuntime, Runtime>();
            services.TryAddTransient<IValueConverter, MockValueConverter>();
            services.TryAddTransient<IZoneMapper, MockZoneMapper>();
        }

    }
}
