using ToSic.Sxc.Cms.Settings;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Backend.Cms.Load.Settings;

internal class LoadSettingsForGpsDefaults(LazySvc<Services.IFeaturesService> features)
    : LoadSettingsForBase($"{SxcLogName}.LdGpsD", connect: [features])
{
    public override Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters) =>
        GetSettings<MapsCoordinates, GoogleMaps>(
            parameters,
            MapsCoordinates.Defaults,
            !features.Value.IsEnabled(BuiltInFeatures.EditUiGpsCustomDefaults.NameId),
            GoogleMaps.SettingsPath,
            $"{GoogleMaps.SettingsPath}.{nameof(GoogleMaps.DefaultCoordinates)}",
            model => model.DefaultCoordinates);
}