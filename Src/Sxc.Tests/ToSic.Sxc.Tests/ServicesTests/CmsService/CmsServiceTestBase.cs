using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Core.Tests;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
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
            var eavSystemLoader = GetService<EavSystemLoader>();
            eavSystemLoader.StartUp();
            eavSystemLoader.LoadLicenseAndFeatures();

            var appStates = GetService<IAppStates>();
            var app = appStates.GetPresetOrNull();
            TstDataContentType = app.GetContentType("TstData");
            if (TstDataContentType == null) throw new Exception("TstData content type not found. Probably JSON is missing.");
            DynamicEntityServices = GetService<DynamicEntity.MyServices>();
        }
        public readonly DynamicEntity.MyServices DynamicEntityServices;
        public readonly IContentType TstDataContentType;

        protected override void SetupServices(IServiceCollection services)
        {
            base.SetupServices(services);
            services.AddTransient<IRuntime, Runtime>();
            services.TryAddTransient<IValueConverter, MockValueConverter>();
        }


        public IEntity TstDataEntity(string text = "", string html = "", IContentType contentType = null)
        {
            var values = new Dictionary<string, object>()
            {
                {SomeTextField, text},
                {SomeHtmlField, html}
            };
            return GetService<EntityBuilder>().TestCreate(appId: AppId, entityId: 1, contentType: contentType, values: values, titleField: SomeTextField);
        }

        public DynamicEntity DynEntity(IEntity entity = null) => new DynamicEntity(entity, DynamicEntityServices, strict: true);

        public IHtmlTag CmsServiceShow(string someHtmlValue)
        {
            const string someTextValue = "Just Basic Text";
            var entity = TstDataEntity(someTextValue, someHtmlValue, TstDataContentType);
            var dynamicEntity = DynEntity(entity);
            var dynamicField = dynamicEntity.Field(SomeHtmlField);
            //var imgService = Build<LazySvc<IImageService>>();
            //var valueConverter = Build<LazySvc<IValueConverter>>();
            var cmsService = GetService<ICmsService>(); // new Services.CmsService.CmsService(imgService, valueConverter);
            return cmsService.Html(dynamicField);
        }
    }
}