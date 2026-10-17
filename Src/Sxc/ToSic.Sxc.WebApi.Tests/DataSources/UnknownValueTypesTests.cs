using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Backend.Admin;
using ToSic.Sxc.Backend.Views;
using ToSic.Sxc.WebApi.Tests.CodeGeneration;
using Xunit.DependencyInjection;

namespace ToSic.Sxc.WebApi.Tests.DataSources;

[Startup(typeof(StartupCodeGenerationTests))]
public class UnknownValueTypesTests(
    Generator<IDataFactory, DataFactoryOptions> dataFactory,
    ContentTypesFromCodeManager contentTypes)
{
    [Fact]
    public void ObjectFieldsEnableUnknownValueConversion()
    {
        var raw = new InputTypeInfoRaw
        {
            Type = "test",
            Label = null,
            Description = null,
            DisableI18n = false,
            UiAssets = new Dictionary<string, string> { ["main"] = "main.js" },
            UseAdam = false,
            IsObsolete = false,
            ObsoleteMessage = null,
            IsRecommended = false,
            IsDefault = false,
            Source = null,
            ConfigTypes = ["TestConfig"],
        };

        var entity = dataFactory.New(new()).Create(raw);

        Assert.Equal(ValueTypes.Object, entity.Attributes[nameof(InputTypeInfoRaw.UiAssets)].Type);
        Assert.Equal(ValueTypes.Object, entity.Attributes[nameof(InputTypeInfoRaw.ConfigTypes)].Type);
    }

    [Theory]
    [MemberData(nameof(ObjectFields))]
    public void CodeContentTypesDeclareObjectFields(Type type, string field)
        => Assert.Equal(ValueTypes.Object, contentTypes.Get(type)[field]!.Type);

    public static TheoryData<Type, string> ObjectFields => new()
    {
        { typeof(InputTypeInfoRaw), nameof(InputTypeInfoRaw.UiAssets) },
        { typeof(AppWebApiEndpointRaw), nameof(AppWebApiEndpointRaw.parameters) },
        { typeof(ContentTypeDto), nameof(ContentTypeDto.Properties) },
        { typeof(ContentTypeFieldDto), nameof(ContentTypeFieldDto.InputTypeConfig) },
        { typeof(DataSourceDto), nameof(DataSourceDto.Out) },
        { typeof(ExtensionDto), nameof(ExtensionDto.Configuration) },
        { typeof(ViewDetailsDto), nameof(ViewDetailsDto.Lightspeed) },
    };
}
