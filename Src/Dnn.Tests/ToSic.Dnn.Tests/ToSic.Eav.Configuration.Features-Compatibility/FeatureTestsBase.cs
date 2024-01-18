using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps.Tests.Mocks;
using ToSic.Eav.Data;
using ToSic.Eav.Internal.Loaders;
using ToSic.Testing.Shared;
using ToSic.Testing.Shared.Mocks;
using ToSic.Eav.ImportExport;
using ToSic.Eav.Integration;

namespace ToSic.Dnn.Tests.ToSic.Eav.Configuration.Features_Compatibility
{
    public class FeatureTestsBase: TestBaseEav
    {
        public FeatureTestsBase()
        {
            // Make sure that features are ready to use
            var sysLoader = GetService<EavSystemLoader>();
            sysLoader.ReloadFeatures();
        }

        protected override void SetupServices(IServiceCollection services)
        {
            base.SetupServices(services);
            services.AddAppLoader();
            services.TryAddTransient<IValueConverter, MockValueConverter>();
            services.TryAddTransient<IZoneMapper, MockZoneMapper>();
        }

    }
}
