using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Metadata;
using ToSic.Eav.WebApi.Sys.Admin.Metadata;

namespace ToSic.Eav.WebApi.Sys.Admin;

[PrivateApi]
[VisualQuery(
    NiceName = "App Enhancements",
    NameId = "32d31f86-b9d6-44e0-a322-f32b2aa43f62",
    NameIds = ["System.AppEnhancements"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Internal,
    UiHint = "Settings, resources and metadata of the current app"
)]
public class AppEnhancements : CustomDataSource
{
    private readonly LazySvc<IAppReaderFactory> _appReaders;
    private readonly GenWorkBasic<WorkAttributes> _workAttributes;
    private readonly Generator<ConvertAttributeToDto> _convertAttribute;
    private readonly IAppsCatalog _appsCatalog;
    private readonly LazySvc<MetadataControllerReal> _metadataController;

    public AppEnhancements(
        Dependencies services,
        LazySvc<IAppReaderFactory> appReaders,
        GenWorkBasic<WorkAttributes> workAttributes,
        Generator<ConvertAttributeToDto> convertAttribute,
        IAppsCatalog appsCatalog,
        LazySvc<MetadataControllerReal> metadataController)
        : base(services, logName: "Sxc.AppEnh", connect: [appReaders, workAttributes, convertAttribute, appsCatalog, metadataController])
    {
        _workAttributes = workAttributes;
        _convertAttribute = convertAttribute;
        _appReaders = appReaders;
        _appsCatalog = appsCatalog;
        _metadataController = metadataController;

        ProvideOut(GetAppSettings, "AppSettings");
        ProvideOut(GetAppResources, "AppResources");
        ProvideOut(() => GetEntities(AppStackConstants.Settings.SystemType), "SettingsSystem");
        ProvideOut(() => GetEntities(AppStackConstants.Resources.SystemType), "ResourcesSystem");
        ProvideOut(() => GetEntities(AppLoadConstants.TypeAppConfig), "ToSxcContentApp");
        ProvideOutRaw(() => GetFields(true), name: "AppSettingFields", options: () => new()
        {
            TitleField = nameof(ContentTypeFieldDto.StaticName), TypeName = "ContentTypeField", AllowUnknownValueTypes = true,
        });
        ProvideOutRaw(() => GetFields(false), name: "AppResourceFields", options: () => new()
        {
            TitleField = nameof(ContentTypeFieldDto.StaticName), TypeName = "ContentTypeField", AllowUnknownValueTypes = true,
        });
        ProvideOutRaw(GetMetadata, name: "Metadata", options: () => new() { AllowUnknownValueTypes = true });
    }

    private IEnumerable<IEntity> GetAppSettings() => GetEntities(Types.Settings);
    private IEnumerable<IEntity> GetAppResources() => GetEntities(Types.Resources);

    private IEnumerable<IEntity> GetEntities(string? typeName)
    {
        if (typeName == null) return [];
        var appReader = _appReaders.Value.Get(AppId);
        return appReader.List.Where(entity => entity.AppId == AppId && entity.Type.Name == typeName);
    }

    private IEnumerable<IRawEntity> GetFields(bool settings)
    {
        var typeName = settings
            ? Types.Settings
            : Types.Resources;
        if (typeName == null)
            return [];
        var fields = _workAttributes.New(AppId).GetFields(typeName);
        return _convertAttribute.New()
            .Init(AppId, false)
            .Convert(fields)
            .Select(field => field.ToRawEntity());
    }

    private IEnumerable<IRawEntity> GetMetadata()
    {
        var items = _metadataController.Value
            .Get(AppId, (int)TargetTypes.App, "number", AppId.ToString()).Items
                    ?? [];
        return items.Select(item => new RawEntity
        {
            Id = item.TryGetValue("Id", out var id) ? Convert.ToInt32(id) : 0,
            Values = item.ToDictionary(pair => pair.Key, pair => pair.Value),
        });
    }

    private (string? Settings, string? Resources) Types => _types ??= GetTypes();
    private (string? Settings, string? Resources)? _types;
    private (string? Settings, string? Resources) GetTypes()
    {
        if (_types != null)
            return _types.Value;
        
        var appReader = _appReaders.Value.Get(AppId);
        var hasSettingsCustom = appReader.ContentTypes
            .Any(type => type.Scope == ScopeConstants.SystemConfiguration && type.Name == AppStackConstants.Settings.CustomType);
        var hasResourcesCustom = appReader.ContentTypes
            .Any(type => type.Scope == ScopeConstants.SystemConfiguration && type.Name == AppStackConstants.Resources.CustomType);
        var app = _appsCatalog.AppIdentity(AppId);
        var isGlobalOrPrimary = app.IsGlobalSettingsApp() || app.AppId == _appsCatalog.PrimaryAppIdentity(app.ZoneId).AppId;
        return isGlobalOrPrimary
            ? (
                hasSettingsCustom ? AppStackConstants.Settings.CustomType : null,
                hasResourcesCustom ? AppStackConstants.Resources.CustomType : null
            )
            : (
                AppLoadConstants.TypeAppSettings,
                AppLoadConstants.TypeAppResources
            );
    }

}
