//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.DependencyInjection.Extensions;
//using ToSic.Eav.Apps.Mocks;
//using ToSic.Eav.Data;
//using ToSic.Eav.Data.Mocks;
//using ToSic.Eav.Integration;
//using ToSic.Eav.Internal.Loaders;

//namespace ToSic.Sxc.ToSic.Eav.Configuration.Features_Compatibility;

//public class FeatureTestsBase: TestBaseEav
//{
//    public FeatureTestsBase()
//    {
//        // Make sure that features are ready to use
//        var sysLoader = GetService<EavSystemLoader>();
//        sysLoader.ReloadFeatures();
//    }

//    protected override IServiceCollection SetupServices(IServiceCollection services)
//    {
//        base.SetupServices(services)
//            .AddAppLoader();
//        services.TryAddTransient<IValueConverter, MockValueConverter>();
//        services.TryAddTransient<IZoneMapper, MockZoneMapper>();
//        return services;
//    }

//}