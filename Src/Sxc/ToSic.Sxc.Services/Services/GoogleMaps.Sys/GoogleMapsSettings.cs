using System.Text.Json;
using ToSic.Eav.Models;
using ToSic.Eav.Serialization.Sys.Json;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.GoogleMaps.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
[ModelSpecs(ContentType = ContentTypeNameId)]
public record GoogleMapsSettings() : ModelOfEntity
{
    public const string ContentTypeNameId = "f5764f60-2621-4a5d-9391-100fbe664640";

    public const string SettingsPath = "Settings.GoogleMaps";

    public int Zoom => GetThis(14); // 14 is a kind of neutral default

    public string ApiKey => GetThis("");

    public string Icon => GetThis("");

    public MapsCoordinates DefaultCoordinates => field ??= GetMapsCoordinates();

    private MapsCoordinates GetMapsCoordinates()
    {
        var json = Get(nameof(DefaultCoordinates), "");
        if (!json.HasValue())
            return MapsCoordinates.Defaults;
        try
        {
            return JsonSerializer.Deserialize<MapsCoordinates>(json, JsonOptions.SafeJsonForHtmlAttributes)
                ?? MapsCoordinates.Defaults;
        }
        catch
        {
            return MapsCoordinates.Defaults;
        }
    }

}