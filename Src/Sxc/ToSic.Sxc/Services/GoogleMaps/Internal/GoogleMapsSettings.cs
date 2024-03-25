using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;

namespace ToSic.Sxc.Services.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class GoogleMapsSettings(IJsonService jsonService)
    : EntityBasedService<GoogleMapsSettings>($"{SxcLogName}.GMapSt")
{
    public static string TypeIdentifier = "f5764f60-2621-4a5d-9391-100fbe664640";

    public string SettingsIdentifier => "Settings.GoogleMaps";

    public int Zoom => GetThis(14); // 14 is a kind of neutral default

    public string ApiKey => GetThis("");

    public string Icon => GetThis("");

    public MapsCoordinates DefaultCoordinates => _defCoords.Get(GetMapsCoordinates);
    private readonly GetOnce<MapsCoordinates> _defCoords = new();

    private MapsCoordinates GetMapsCoordinates() => Log.Func(() =>
    {
        var json = Get(nameof(DefaultCoordinates), "");
        if (!json.HasValue()) return (MapsCoordinates.Defaults, "no json");
        try
        {
            return (jsonService.To<MapsCoordinates>(json), "from json");
        }
        catch
        {
            return (MapsCoordinates.Defaults, "error");
        }
    });

}