using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.AppStack;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.Metadata;
using ToSic.Eav.WebApi.Sys.Admin.Metadata;
using ToSic.Eav.WebApi.Sys.Dto;

namespace ToSic.Eav.WebApi.Sys.Admin;

/// <summary>
/// All data needed by the App administration settings screen in one request.
/// </summary>
[PrivateApi]
[VisualQuery(
    NiceName = "App System Data",
    NameId = "bbcd8a91-1ddb-4d27-b7b6-ca6f5cebdbae",
    NameIds = ["System.AppSystemData"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Internal,
    UiHint = "Settings, resources, fields, app configuration and metadata of the current app"
)]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppSystemData(
    CustomDataSource.Dependencies services,
    AppWorkContextService appWorkCtxSvc,
    AppWorkChain<WorkAttributes> workAttributes,
    Generator<ConvertAttributeToDto> convertAttribute,
    IAppsCatalog appsCatalog,
    LazySvc<EntitiesAdminData> entitiesAdminData,
    LazySvc<MetadataControllerReal> metadataController)
    : AppEnhancements(
        services,
        appWorkCtxSvc,
        workAttributes,
        convertAttribute,
        appsCatalog,
        metadataController)
{
    protected override IEnumerable<IEntity> GetEntities(string? typeName)
        => entitiesAdminData.Value.GetEntities(AppId, typeName);
}
