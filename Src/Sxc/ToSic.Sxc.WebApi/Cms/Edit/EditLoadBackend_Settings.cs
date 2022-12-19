using System.Collections.Generic;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Sxc.Services.GoogleMaps;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EditLoadBackend
    {
        /// <summary>
        /// WIP v15.
        /// Later it should be built using a list of services that provide settings to the UI.
        /// - put gps coordinates in static
        /// - later get from settings
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> GetSettings(IContextOfApp contextOfApp)
        {
            var coordinates = MapsCoordinates.Defaults;

            if (_features.Value.IsEnabled(BuiltInFeatures.EditUiGpsCustomDefaults.NameId))
            {
                var getMaps = contextOfApp.AppSettings.InternalGetPath(_googleMapsSettings.SettingsIdentifier);
                coordinates = getMaps.GetFirstResultEntity() is IEntity mapsEntity
                    ? _googleMapsSettings.Init(mapsEntity).DefaultCoordinates
                    : MapsCoordinates.Defaults;
            }

            var settings = new Dictionary<string, object>
            {
                { "gps-default-coordinates", coordinates }
            };
            return settings;
        }
    }
}
