using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Data;
using ToSic.Eav.Persistence.File;
using ToSic.Eav.Run;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Testing.Shared.Mocks;

namespace ToSic.Sxc.Tests.ServicesTests.CmsService
{
    public class CmsServiceTestBase : TestBaseSxcDb
    {
        public const int AppId = -1;
        public const string SomeTextField = "SomeText";
        public const string SomeHtmlField = "SomeHtml";

        public CmsServiceTestBase()
        {
            var eavSystemLoader = Build<EavSystemLoader>();
            eavSystemLoader.StartUp();
            eavSystemLoader.LoadLicenseAndFeatures();

            var appStates = Build<IAppStates>();
            var app = appStates.GetPresetOrNull();
            TstDataContentType = app.GetContentType("TstData");
            if (TstDataContentType == null) throw new Exception("TstData content type not found. Probably JSON is missing.");
            DynamicEntityDependencies = Build<DynamicEntityDependencies>();
        }
        public readonly DynamicEntityDependencies DynamicEntityDependencies;
        public readonly IContentType TstDataContentType;

        protected override void AddServices(IServiceCollection services)
        {
            services.AddTransient<IRuntime, Runtime>();
            services.TryAddTransient<IValueConverter, MockValueConverter>();
        }

        public static IEntity TstDataEntity(string text = "", string html = "", IContentType contentType = null)
        {
            var values = new Dictionary<string, object>()
            {
                {SomeTextField, text},
                {SomeHtmlField, html}
            };
            return new Entity(AppId, 1, contentType, values, SomeTextField);
        }

        public DynamicEntity DynEntity(IEntity entity = null) => new DynamicEntity(entity, DynamicEntityDependencies);

        public IHtmlTag CmsServiceShow(string someHtmlValue)
        {
            const string someTextValue = "Just Basic Text";
            var entity = TstDataEntity(someTextValue, someHtmlValue, TstDataContentType);
            var dynamicEntity = DynEntity(entity);
            var dynamicField = dynamicEntity.Field(SomeHtmlField);
            //var imgService = Build<LazySvc<IImageService>>();
            //var valueConverter = Build<LazySvc<IValueConverter>>();
            var cmsService = Build<ICmsService>(); // new Services.CmsService.CmsService(imgService, valueConverter);
            return cmsService.Show(dynamicField);
        }
    }
}