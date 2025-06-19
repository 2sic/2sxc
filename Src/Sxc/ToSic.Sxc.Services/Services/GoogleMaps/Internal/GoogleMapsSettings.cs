
using ToSic.Eav.Data.EntityBased.Sys;
using ToSic.Lib.Helpers;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class GoogleMapsSettings(IJsonService jsonService) : EntityBasedService($"{SxcLogName}.GMapSt")
{
    public static string TypeIdentifier = "f5764f60-2621-4a5d-9391-100fbe664640";

    public string SettingsIdentifier => "Settings.GoogleMaps";

    public int Zoom => GetThis(14); // 14 is a kind of neutral default

    public string ApiKey => GetThis("");

    public string Icon => GetThis("");

    public MapsCoordinates DefaultCoordinates => _defCoords.Get(GetMapsCoordinates)!;
    private readonly GetOnce<MapsCoordinates> _defCoords = new();

    private MapsCoordinates GetMapsCoordinates()
    {
        var l = Log.Fn<MapsCoordinates>();
        var json = Get(nameof(DefaultCoordinates), "");
        if (!json.HasValue())
            return l.Return(MapsCoordinates.Defaults, "no json");
        try
        {
            var result = jsonService.To<MapsCoordinates>(json)
                ?? MapsCoordinates.Defaults;
            return l.Return(result, "from json");
        }
        catch
        {
            return l.Return(MapsCoordinates.Defaults, "error");
        }
    }

}