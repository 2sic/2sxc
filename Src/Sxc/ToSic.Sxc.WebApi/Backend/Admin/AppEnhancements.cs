using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.Raw.Sys;
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
    private readonly GenWorkPlus<WorkEntities> _workEntities;
    private readonly GenWorkBasic<WorkAttributes> _workAttributes;
    private readonly Generator<ConvertAttributeToDto> _convertAttribute;
    private readonly IAppsCatalog _appsCatalog;
    private readonly LazySvc<MetadataControllerReal> _metadataController;

    public AppEnhancements(Dependencies services, GenWorkPlus<WorkEntities> workEntities,
        GenWorkBasic<WorkAttributes> workAttributes, Generator<ConvertAttributeToDto> convertAttribute,
        IAppsCatalog appsCatalog, LazySvc<MetadataControllerReal> metadataController)
        : base(services, logName: "Sxc.AppEnh", connect: [workEntities, workAttributes, convertAttribute, appsCatalog, metadataController])
    {
        _workEntities = workEntities;
        _workAttributes = workAttributes;
        _convertAttribute = convertAttribute;
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

    private IEnumerable<IEntity> GetAppSettings() => GetEntities(GetTypes().Settings);
    private IEnumerable<IEntity> GetAppResources() => GetEntities(GetTypes().Resources);

    private IEnumerable<IEntity> GetEntities(string? typeName)
    {
        if (typeName == null) return [];
        var appReader = _workEntities.CtxSvc.ContextPlus(AppId).AppReader;
        return appReader.List.Where(entity => entity.AppId == AppId && entity.Type.Name == typeName);
    }

    private IEnumerable<IRawEntity> GetFields(bool settings)
    {
        var typeName = settings ? GetTypes().Settings : GetTypes().Resources;
        if (typeName == null) return [];
        return _convertAttribute.New().Init(AppId, false)
            .Convert(_workAttributes.New(AppId).GetFields(typeName))
            .Select(field => field.ToRawEntity());
    }

    private IEnumerable<IRawEntity> GetMetadata()
    {
        var items = _metadataController.Value.Get(AppId, (int)TargetTypes.App, "number", AppId.ToString()).Items ?? [];
        return items.Select(item => new RawEntity(item.ToDictionary(pair => pair.Key, pair => pair.Value))
        {
            Id = item.TryGetValue("Id", out var id) ? Convert.ToInt32(id) : 0,
        });
    }

    private (string? Settings, string? Resources) GetTypes()
    {
        var appReader = _workEntities.CtxSvc.ContextPlus(AppId).AppReader;
        var hasSettingsCustom = appReader.ContentTypes.Any(type => type.Scope == ScopeConstants.SystemConfiguration && type.Name == "SettingsCustom");
        var hasResourcesCustom = appReader.ContentTypes.Any(type => type.Scope == ScopeConstants.SystemConfiguration && type.Name == "ResourcesCustom");
        var app = _appsCatalog.AppIdentity(AppId);
        var isGlobalOrPrimary = app.IsGlobalSettingsApp() || app.AppId == _appsCatalog.PrimaryAppIdentity(app.ZoneId).AppId;
        return isGlobalOrPrimary
            ? (hasSettingsCustom ? "SettingsCustom" : null, hasResourcesCustom ? "ResourcesCustom" : null)
            : (AppLoadConstants.TypeAppSettings, AppLoadConstants.TypeAppResources);
    }

}
