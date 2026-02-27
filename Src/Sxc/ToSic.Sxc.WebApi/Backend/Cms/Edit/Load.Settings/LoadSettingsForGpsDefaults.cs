using ToSic.Sxc.Services.GoogleMaps.Sys;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Backend.Cms.Load.Settings;

internal class LoadSettingsForGpsDefaults(LazySvc<Services.IFeaturesService> features)
    : LoadSettingsForBase($"{SxcLogName}.LdGpsD", connect: [features])
{
    public override Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters) =>
        GetSettings<MapsCoordinates, GoogleMapsSettings>(
            parameters,
            MapsCoordinates.Defaults,
            !features.Value.IsEnabled(BuiltInFeatures.EditUiGpsCustomDefaults.NameId),
            GoogleMapsSettings.SettingsPath,
            $"{GoogleMapsSettings.SettingsPath}.{nameof(GoogleMapsSettings.DefaultCoordinates)}",
            model => model.DefaultCoordinates);
}