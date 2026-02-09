using System.Text.Json.Serialization;
using ToSic.Eav.Models;

namespace ToSic.Sxc.Backend.App;

[ModelSpecs(ContentType = ContentTypeId)]
internal record AppExtensionRelease: ModelOfEntityCore
{
    public const string ContentTypeId = "1d91a2c0-4d5d-4197-8cb8-d4bfebf72f15";
    public const string ContentTypeName = "AppExtensionRelease";
    private const string DefaultVersion = "00.00.01";

    [JsonPropertyName("releaseNotes")]
    public string ReleaseNotes => GetThis("");

    [JsonPropertyName("version")]
    public string Version => GetThis(DefaultVersion);

    [JsonPropertyName("isBreaking")]
    public bool IsBreaking => GetThis(false);
}
