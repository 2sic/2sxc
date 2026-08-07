using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Backend.SysData;

namespace ToSic.Sxc.Backend.Cms;

[PrivateApi]
[VisualQuery(NiceName = "Item History", NameId = "43f0261c-7400-4edb-b622-2606db9eebda", NameIds = ["System.ItemHistory"], Type = DataSourceType.System, Audience = Audience.System, DataConfidentiality = DataConfidentiality.Confidential, UiHint = "Version history of an item")]
public class ItemHistory : CustomDataSource
{
    [Configuration]
    public int EntityId => Configuration.GetThis(0);

    public ItemHistory(Dependencies services, GenWorkDb<WorkEntityVersioning> versioning)
        : base(services, "Sxc.ItemHist", connect: [versioning])
        => ProvideOutRaw(() => Get(versioning), options: Options);

    private IEnumerable<IRawEntity> Get(GenWorkDb<WorkEntityVersioning> versioning)
        => SysDataRaw.Many(versioning.New(appId: AppId).VersionHistory(EntityId));

    private static DataFactoryOptions Options() => new() { AutoId = true, TypeName = "ItemHistory", AllowUnknownValueTypes = true };
}
