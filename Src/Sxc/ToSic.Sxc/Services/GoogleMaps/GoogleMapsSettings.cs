using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helper;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Services.GoogleMaps
{
    [PrivateApi]
    public class GoogleMapsSettings: EntityBasedService<GoogleMapsSettings>
    {
        public static string TypeIdentifier = "f5764f60-2621-4a5d-9391-100fbe664640";

        public GoogleMapsSettings(IJsonService jsonService) : base($"{Constants.SxcLogName}.GMapSt")
        {
            _jsonService = jsonService;
        }
        private readonly IJsonService _jsonService;

        public string SettingsIdentifier => "GoogleMaps";

        public int Zoom => Get(nameof(Zoom), 14); // 14 is a kind of neutral default

        public string ApiKey => Get(nameof(ApiKey), "");

        public string Icon => Get(nameof(Icon), "");

        public MapsCoordinates DefaultCoordinates => _defCoords.Get(GetMapsCoordinates);

        private MapsCoordinates GetMapsCoordinates() => Log.Func(() =>
        {
            var json = Get(nameof(DefaultCoordinates), "");
            if (!json.HasValue()) return (MapsCoordinates.Defaults, "no json");
            try
            {
                return (_jsonService.To<MapsCoordinates>(json), "from json");
            }
            catch
            {
                return (MapsCoordinates.Defaults, "error");
            }
        });

        private readonly GetOnce<MapsCoordinates> _defCoords = new GetOnce<MapsCoordinates>();
    }
}
