using ToSic.Eav.Data.Sys;
using ToSic.Eav.Models;
using ToSic.Sxc.Services.GoogleMaps.Sys;
using ToSic.Sys.Capabilities.Features;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.Backend.Cms;

internal class LoadSettingsForGpsDefaults(
    LazySvc<IFeaturesService> features)
    : ServiceBase($"{SxcLogName}.LdGpsD", connect: [features]), ILoadSettingsProvider
{
    public Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters)
    {
        var l = Log.Fn<Dictionary<string, object>>();
        var coordinates = MapsCoordinates.Defaults;

        if (features.Value.IsEnabled(BuiltInFeatures.EditUiGpsCustomDefaults.NameId))
        {
            var getMaps = parameters.ContextOfApp.AppSettings.InternalGetPath(GoogleMapsSettings.SettingsPath);
            coordinates = getMaps?.GetFirstResultEntity() is { } mapsEntity
                ? mapsEntity.As<GoogleMapsSettings>()!.DefaultCoordinates
                : MapsCoordinates.Defaults;
        }

        var result = new Dictionary<string, object>
        {
            [$"{GoogleMapsSettings.SettingsPath}.{nameof(GoogleMapsSettings.DefaultCoordinates)}"] = coordinates,
        };
        return l.Return(result, $"{result.Count}");
    }
}