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
// ReSharper disable once UnusedMember.Global
public class AppEnhancements : CustomDataSource
{
    private readonly IAppsCatalog _appsCatalog;

    private readonly Lazy<IAppWorkContext> _workContext;
    private readonly Lazy<WorkAttributes> _workAttributes;
    private readonly Lazy<ConvertAttributeToDto> _convertAttributeToDto;

    public AppEnhancements(
        Dependencies services,
        AppWorkContextService appWorkCtxSvc,
        AppWorkChain<WorkAttributes> workAttributes,
        Generator<ConvertAttributeToDto> convertAttribute,
        IAppsCatalog appsCatalog,
        LazySvc<MetadataControllerReal> metadataController)
        : base(services, logName: "Sxc.AppEnh", connect: [appWorkCtxSvc, workAttributes, convertAttribute, appsCatalog, metadataController])
    {
        _workContext = new(() => appWorkCtxSvc.ContextNew(AppId));
        _workAttributes = new(() => workAttributes.New(_workContext.Value));
        _convertAttributeToDto = new(() => convertAttribute.New().Init(AppId, false));
        _appsCatalog = appsCatalog;

        ProvideOut(() => GetEntities(TypeNames.Settings), "AppSettings");
        ProvideOut(() => GetEntities(TypeNames.Resources), "AppResources");
        ProvideOut(() => GetEntities(AppStackConstants.Settings.SystemType), AppStackConstants.Settings.SystemType);
        ProvideOut(() => GetEntities(AppStackConstants.Resources.SystemType), AppStackConstants.Resources.SystemType);

        // TODO: @2rb - this should be "AppConfig" to be consistent with the other names.
        // TODO: pls fix and update the UI which uses this.
        ProvideOut(() => GetEntities(AppLoadConstants.TypeAppConfig), "ToSxcContentApp");

        ProvideOutRaw(() => GetFields(TypeNames.Settings), name: "AppSettingFields", options: () => new()
        {
            TitleField = nameof(ContentTypeFieldDto.StaticName), TypeName = "ContentTypeField", AllowUnknownValueTypes = true,
        });

        // TODO: @2rb - this should be AppResourcesFields (with an "s" at the end) to be consistent with the other names.
        // TODO: pls fix, and update the UI to use the corrected name.
        ProvideOutRaw(() => GetFields(TypeNames.Resources), name: "AppResourceFields", options: () => new()
        {
            TitleField = nameof(ContentTypeFieldDto.StaticName), TypeName = "ContentTypeField", AllowUnknownValueTypes = true,
        });
        ProvideOutRaw(() => GetMetadata(metadataController.Value), name: "Metadata", options: () => new() { AllowUnknownValueTypes = true });
    }

    private IEnumerable<IEntity> GetEntities(string? typeName)
    {
        // For primary/global apps the name might be null, if there is no data for that type, so we return an empty list in that case
        if (typeName == null)
            return [];
        return _workContext.Value.AppReader.List
            .Where(entity => entity.AppId == AppId && entity.Type.Name == typeName);
    }

    private IEnumerable<IRawEntity> GetFields(string? typeName)
    {
        if (typeName == null)
            return [];
        var fields = _workAttributes.Value.GetFields(typeName);
        return _convertAttributeToDto.Value
            .Convert(fields)
            // TODO: @2rb - I'm quite certain to the ToRawEntity is not needed any more
            // TODO: @2rb pls verify and probably skip and delete the method which almost certainly is irrelevant
            .Select(field => field.ToRawEntity()); 
    }

    private IEnumerable<IRawEntity> GetMetadata(MetadataControllerReal metadataController)
    {
        var items = metadataController
                        .Get(AppId, (int)TargetTypes.App, "number", AppId.ToString())
                        .Items
                    ?? [];
        return items
            .Select(item => new RawEntity
            {
                Id = item.TryGetValue("Id", out var id) ? Convert.ToInt32(id) : 0,
                Values = item.ToDictionary(pair => pair.Key, pair => pair.Value),
            });
    }

    /// <summary>
    /// Settings/Resources ContentType Names
    /// They vary based on the app type (normal, primary, global) and if the special content-types exist.
    /// </summary>
    private (string? Settings, string? Resources) TypeNames => _typeNames ??= BuildTypes();
    private (string? Settings, string? Resources)? _typeNames;
    private (string? Settings, string? Resources) BuildTypes()
    {
        // Check if it's the global app or the primary app, which would be a bit special
        var app = _appsCatalog.AppIdentity(AppId);
        var isGlobalOrPrimary = app.IsGlobalSettingsApp() || app.AppId == _appsCatalog.PrimaryAppIdentity(app.ZoneId).AppId;

        // If it's a normal app (Content, Blog, etc.) just return the default type names
        if (!isGlobalOrPrimary)
            return (AppLoadConstants.TypeAppSettings, AppLoadConstants.TypeAppResources);

        // If it's the global or primary app, check if the special content-types exist,
        // and if so, return those instead of the default ones. If not, return null for that type.
        var appReader = _workContext.Value.AppReader;
        var hasSettingsCustom = appReader.ContentTypes
            .Any(type => type.Scope == ScopeConstants.SystemConfiguration && type.Name == AppStackConstants.Settings.CustomType);
        var hasResourcesCustom = appReader.ContentTypes
            .Any(type => type.Scope == ScopeConstants.SystemConfiguration && type.Name == AppStackConstants.Resources.CustomType);
        return (
            hasSettingsCustom ? AppStackConstants.Settings.CustomType : null,
            hasResourcesCustom ? AppStackConstants.Resources.CustomType : null
        );
    }

}
