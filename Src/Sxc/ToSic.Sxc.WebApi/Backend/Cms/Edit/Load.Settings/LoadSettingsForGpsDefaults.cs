using ToSic.Eav.Ui.Edit.Sys.Load.Settings;
using ToSic.Sxc.Cms.Settings;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Backend.Cms.Load.Settings;

internal class LoadSettingsForGpsDefaults(LazySvc<Services.IFeaturesService> features)
    : ServiceBase($"{SxcLogName}.LdGpsD", connect: [features]), ILoadSettingsProvider
{
    public Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters) =>
        LoadSettingsForBase.GetSettings<MapsCoordinates, GoogleMaps>(
            appSettings: parameters.ContextOfApp.AppSettings,
            defaults: MapsCoordinates.Defaults,
            forceDefaults: !features.Value.IsEnabled(BuiltInFeatures.EditUiGpsCustomDefaults.NameId),
            lookupPath: GoogleMaps.SettingsPath,
            outputPath: $"{GoogleMaps.SettingsPath}.{nameof(GoogleMaps.DefaultCoordinates)}",
            modelConverter: model => model.DefaultCoordinates,
            log: Log);
}