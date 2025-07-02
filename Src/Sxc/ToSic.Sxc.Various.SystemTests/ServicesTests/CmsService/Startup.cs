using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Mocks;
using ToSic.Eav.Apps.Sys.Catalog;
using ToSic.Eav.Context;
using ToSic.Sxc.Mocks;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.PageService;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class Startup: StartupSxcWithDb
{
    public override void ConfigureServices(IServiceCollection services)
    {
        //services.TryAddTransient<IValueConverter, MockValueConverter>();
        base.ConfigureServices(
            services
                // Add the MockPageService to the services - needed as a sub-dependency of the CmsService / StringWysiwyg
                .AddTransient<IPageService, MockPageService>()
                .AddTransient<DataForCmsServiceTests>()

                // This is a dependency of the IApp
                .AddTransient<ISite, MockSite>()
                .AddTransient<ExecutionContextMock>()
                .AddTransient<IAppsCatalog, AppsCatalogMock>()
        );
    }

}