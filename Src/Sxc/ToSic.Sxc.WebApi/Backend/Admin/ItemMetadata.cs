using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.WebApi.Sys.Admin.Metadata;
using ToSic.Sxc.Backend.SysData;

namespace ToSic.Sxc.Backend.Admin;

[PrivateApi]
[VisualQuery(NiceName = "Item Metadata", NameId = "0c40297c-249b-44d8-b473-7ee5b182b5e2", NameIds = ["System.ItemMetadata"], Type = DataSourceType.System, Audience = Audience.System, DataConfidentiality = DataConfidentiality.Confidential, UiHint = "Metadata items and recommendations for a target")]
public class ItemMetadata : CustomDataSource
{
    [Configuration] public int TargetType => Configuration.GetThis(0);
    [Configuration(Fallback = "string")] public string KeyType => Configuration.GetThis<string>("string");
    [Configuration(Fallback = "")] public string Key => Configuration.GetThis<string>("");
    [Configuration(Fallback = "")] public string ContentType => Configuration.GetThis<string>("");

    public ItemMetadata(Dependencies services, LazySvc<MetadataControllerReal> metadata)
        : base(services, "Sxc.ItemMd", connect: [metadata])
    {
        ProvideOutRaw(() => Recommendations(metadata), name: "Recommendations", options: Options);
        ProvideOutRaw(() => Items(metadata), name: "Items", options: Options);
        ProvideOutRaw(() => Target(metadata), name: "For", options: Options);
    }

    private MetadataListDto Result(LazySvc<MetadataControllerReal> metadata) => metadata.Value.Get(AppId, TargetType, KeyType, Key, ContentType);
    private IEnumerable<IRawEntity> Recommendations(LazySvc<MetadataControllerReal> metadata) => SysDataRaw.Many(Result(metadata).Recommendations);
    private IEnumerable<IRawEntity> Items(LazySvc<MetadataControllerReal> metadata) => Result(metadata).Items?.Select(x => (IRawEntity)new RawEntity { Values = x }) ?? [];
    private IEnumerable<IRawEntity> Target(LazySvc<MetadataControllerReal> metadata) => [SysDataRaw.One(Result(metadata).For)];
    private static DataFactoryOptions Options() => new() { AutoId = true, AllowUnknownValueTypes = true };
}
