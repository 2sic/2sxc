using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ToSic.Eav.Apps;
using ToSic.Eav.Core.Tests;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Integration;
using ToSic.Eav.Internal.Loaders;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;
using ToSic.Testing.Shared.Mocks;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

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

            var appStates = GetService<IAppReaderFactory>();
            var app = appStates.GetSystemPreset();
            TstDataContentType = app.GetContentType("TstData");
            if (TstDataContentType == null) throw new("TstData content type not found. Probably JSON is missing.");
            Cdf = GetService<CodeDataFactory>();
        }
        public readonly CodeDataFactory Cdf;
        public readonly IContentType TstDataContentType;

        protected override void SetupServices(IServiceCollection services)
        {
            base.SetupServices(services);
            services.AddAppLoader();
            // services.AddTransient<IAppLoader, AppLoader>();
            services.TryAddTransient<IValueConverter, MockValueConverter>();
        }


        public IEntity TstDataEntity(string text = "", string html = "", IContentType contentType = null)
        {
            var values = new Dictionary<string, object>()
            {
                {SomeTextField, text},
                {SomeHtmlField, html}
            };
            return GetService<DataBuilder>().TestCreate(appId: AppId, entityId: 1, contentType: contentType, values: values, titleField: SomeTextField);
        }

        public DynamicEntity DynEntStrict(IEntity entity = null) => Cdf.AsDynamic(entity, true);

        public IHtmlTag CmsServiceShow(string someHtmlValue)
        {
            const string someTextValue = "Just Basic Text";
            var entity = TstDataEntity(someTextValue, someHtmlValue, TstDataContentType);
            var dynamicEntity = DynEntStrict(entity);
            var dynamicField = dynamicEntity.Field(SomeHtmlField);
            var cmsService = GetService<ICmsService>();
            return cmsService.Html(dynamicField);
        }
    }
}