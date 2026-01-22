using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.Model;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.GoogleMaps.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record GoogleMapsSettings : ModelOfEntityWithLog
{
    public GoogleMapsSettings(IJsonService jsonService)
        : base(null! /* Entity must be added later */, null, $"{SxcLogName}.GMapSt")
    {
        _jsonService = jsonService;
    }

    public static string TypeIdentifier = "f5764f60-2621-4a5d-9391-100fbe664640";

    public string SettingsIdentifier => "Settings.GoogleMaps";

    public int Zoom => GetThis(14); // 14 is a kind of neutral default

    public string ApiKey => GetThis("");

    public string Icon => GetThis("");

    public MapsCoordinates DefaultCoordinates => _defCoords.Get(GetMapsCoordinates)!;
    private readonly GetOnce<MapsCoordinates> _defCoords = new();
    private readonly IJsonService _jsonService;

    private MapsCoordinates GetMapsCoordinates()
    {
        var l = Log.Fn<MapsCoordinates>();
        var json = Get(nameof(DefaultCoordinates), "");
        if (!json.HasValue())
            return l.Return(MapsCoordinates.Defaults, "no json");
        try
        {
            var result = _jsonService.To<MapsCoordinates>(json)
                ?? MapsCoordinates.Defaults;
            return l.Return(result, "from json");
        }
        catch
        {
            return l.Return(MapsCoordinates.Defaults, "error");
        }
    }

}