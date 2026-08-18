using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;

namespace ToSic.Sxc.Backend.Cms;

[PrivateApi]
[VisualQuery(
    NiceName = "Item History",
    NameId = "43f0261c-7400-4edb-b622-2606db9eebda",
    NameIds = ["System.ItemHistory"],
    Type = DataSourceType.System,
    Audience = Audience.System,
    DataConfidentiality = DataConfidentiality.Confidential,
    UiHint = "Version history of an item")]
public class ItemHistory : CustomDataSource
{
    [Configuration]
    public int EntityId => Configuration.GetThis(0);

    public ItemHistory(Dependencies services, AppWorkQuick<WorkEntityVersioning> versioning)
        : base(services, "Sxc.ItemHist", connect: [versioning])
        => ProvideOutRaw(() => Get(versioning), options: Options);

    private IEnumerable<ItemHistoryRaw> Get(AppWorkQuick<WorkEntityVersioning> versioning)
        => versioning.New(appId: AppId).VersionHistory(EntityId).Select(history => new ItemHistoryRaw(history));

    private static DataFactoryOptions Options() => new() { TypeName = "ItemHistory" };

    private sealed class ItemHistoryRaw(ToSic.Eav.Persistence.Versions.ItemHistory history) : IRawEntityAutoConvert
    {
        public DateTime TimeStamp => history.TimeStamp;
        public string? User => history.User;
        public int ChangeSetId => history.ChangeSetId;
        public int HistoryId => history.HistoryId;

        [ContentTypeTitle]
        public int VersionNumber => history.VersionNumber;

        public string? Json => history.Json;
    }
}
