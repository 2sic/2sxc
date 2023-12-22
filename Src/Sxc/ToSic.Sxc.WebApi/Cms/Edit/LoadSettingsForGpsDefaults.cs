using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Internal.Features;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Services.GoogleMaps;
using static System.StringComparer;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.WebApi.Cms;

internal class LoadSettingsForGpsDefaults: ServiceBase, ILoadSettingsProvider
{
    private readonly GoogleMapsSettings _googleMapsSettings;
    private readonly LazySvc<IFeaturesService> _features;

    public LoadSettingsForGpsDefaults(GoogleMapsSettings googleMapsSettings,
        LazySvc<IFeaturesService> features) : base($"{Constants.SxcLogName}.LdGpsD")
    {
        ConnectServices(
            _googleMapsSettings = googleMapsSettings,
            _features = features
        );
    }

    public Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters) => Log.Func(l =>
    {
        var coordinates = MapsCoordinates.Defaults;

        if (_features.Value.IsEnabled(BuiltInFeatures.EditUiGpsCustomDefaults.NameId))
        {
            var getMaps = parameters.ContextOfApp.AppSettings.InternalGetPath(_googleMapsSettings.SettingsIdentifier);
            coordinates = getMaps.GetFirstResultEntity() is IEntity mapsEntity
                ? _googleMapsSettings.Init(mapsEntity).DefaultCoordinates
                : MapsCoordinates.Defaults;
        }

        var result = new Dictionary<string, object>(InvariantCultureIgnoreCase)
        {
            {
                _googleMapsSettings.SettingsIdentifier + "." + nameof(_googleMapsSettings.DefaultCoordinates),
                coordinates
            }
        };
        return (result, $"{result.Count}");
    });
}