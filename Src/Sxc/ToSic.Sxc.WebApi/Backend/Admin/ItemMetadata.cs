using ToSic.Eav.Apps;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.Metadata.Recommendations.Sys;
using ToSic.Eav.Metadata.Sys;
using ToSic.Eav.WebApi.Sys.Admin.Metadata;

namespace ToSic.Sxc.Backend.Admin;

[PrivateApi]
[VisualQuery(NiceName = "Item Metadata", NameId = "0c40297c-249b-44d8-b473-7ee5b182b5e2", NameIds = ["System.ItemMetadata"], Type = DataSourceType.System, Audience = Audience.System, DataConfidentiality = DataConfidentiality.Confidential, UiHint = "Metadata items and recommendations for a target")]
public class ItemMetadata : CustomDataSource
{
    private MetadataListDto? _result;
    [Configuration] public int TargetType => Configuration.GetThis(0);
    [Configuration(Fallback = "string")] public string KeyType => Configuration.GetThis<string>("string");
    [Configuration(Fallback = "")] public string Key => Configuration.GetThis<string>("");
    [Configuration(Fallback = "")] public string ContentType => Configuration.GetThis<string>("");
    [Configuration(Fallback = "")] public string Refresh => Configuration.GetThis<string>("");

    public ItemMetadata(Dependencies services, LazySvc<MetadataControllerReal> metadata, IAppReaderFactory appReaders, Generator<IConvertToEavLight> converters)
        : base(services, "Sxc.ItemMd", connect: [metadata, appReaders, converters])
    {
        ProvideOutRaw(() => Recommendations(metadata), name: "Recommendations", options: Options);
        ProvideOutRaw(() => Items(appReaders, converters), name: "Items", options: Options);
        ProvideOutRaw(() => Target(metadata), name: "For", options: Options);
    }

    private MetadataListDto Result(LazySvc<MetadataControllerReal> metadata)
    {
        if (_result != null)
            return _result;

        _ = Refresh; // Ensure refresh participates in configuration/cache identity.
        var result = metadata.Value.Get(AppId, TargetType, KeyType, Key, ContentType);
        return _result = new MetadataListDto
        {
            For = result.For,
            Items = result.Items?.ToList(),
            Recommendations = result.Recommendations?.ToList(),
        };
    }
    private IEnumerable<MetadataRecommendationRaw> Recommendations(LazySvc<MetadataControllerReal> metadata)
        => Result(metadata).Recommendations?.Select(recommendation => new MetadataRecommendationRaw(recommendation)) ?? [];
    private IEnumerable<IRawEntity> Items(IAppReaderFactory appReaders, Generator<IConvertToEavLight> converters)
    {
        var metadata = appReaders.Get(AppId).Metadata;
        var contentType = string.IsNullOrWhiteSpace(ContentType) ? null : ContentType;
        var items = (KeyType switch
        {
            "guid" when Guid.TryParse(Key, out var guid) => metadata.GetMetadata(TargetType, guid, contentType),
            "number" when int.TryParse(Key, out var number) => metadata.GetMetadata(TargetType, number, contentType),
            "string" => metadata.GetMetadata(TargetType, Key, contentType),
            _ => [],
        }).Where(item => contentType != null || !Permission.IsPermission(item)).ToList();

        var converter = converters.New();
        converter.Type.Serialize = true;
        converter.Type.WithDescription = true;
        converter.WithGuid = true;
        var converted = converter.Convert(items).ToList();

        return items.Zip(converted, (item, values) =>
        {
            var type = new JsonType(item, withDescription: true);
            var rawValues = new Dictionary<string, object?>(values)
            {
                ["MetadataTypeId"] = type.Id,
                ["MetadataTypeName"] = type.Name,
                ["MetadataTypeTitle"] = type.Title,
                ["MetadataTypeDescription"] = type.Description,
            };
            return (IRawEntity)new RawEntity
            {
                Id = item.EntityId,
                Guid = item.EntityGuid,
                Values = rawValues,
            };
        }).ToList();
    }
    private IEnumerable<MetadataForRaw> Target(LazySvc<MetadataControllerReal> metadata)
        => [new(Result(metadata).For)];
    private static DataFactoryOptions Options() => new() { AllowUnknownValueTypes = true };

    private sealed class MetadataRecommendationRaw(MetadataRecommendation recommendation) : IRawEntityAutoConvert
    {
        public string ContentTypeId => recommendation.Id;

        [ContentTypeTitle]
        public string Title => recommendation.Title;

        public string Name => recommendation.Name;
        public int Count => recommendation.Count;
        public string? DeleteWarning => recommendation.DeleteWarning;
        public string? Icon => recommendation.Icon;
        public bool? CreateEmpty => recommendation.CreateEmpty;
        public string Debug => recommendation.Debug;
        public bool Enabled => recommendation.Enabled;
        public string? MissingFeature => recommendation.MissingFeature;
    }

    private sealed class MetadataForRaw(JsonMetadataFor target) : IRawEntityAutoConvert
    {
        public string? Target => target.Target;
        public int TargetType => target.TargetType;
        public string? String => target.String;
        public Guid? Guid => target.Guid;
        public int? Number => target.Number;
        public bool? Singleton => target.Singleton;

        [ContentTypeTitle]
        public string? Title => target.Title;
    }
}
