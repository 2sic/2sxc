using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.ImportExport.Serialization;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Services.GoogleMaps;
using static System.String;
using static System.StringComparer;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

namespace ToSic.Sxc.WebApi.Cms
{
    public class EditLoadSettingsHelper: ServiceBase
    {
        private readonly GoogleMapsSettings _googleMapsSettings;
        private readonly LazySvc<IFeaturesService> _features;
        private readonly LazySvc<JsonSerializer> _jsonSerializerGenerator;

        public EditLoadSettingsHelper(
            LazySvc<JsonSerializer> jsonSerializerGenerator,
            GoogleMapsSettings googleMapsSettings,
            LazySvc<IFeaturesService> features
            ) : base(Constants.SxcLogName + ".LodSet")
        {
            ConnectServices(
                _jsonSerializerGenerator = jsonSerializerGenerator,
                _googleMapsSettings = googleMapsSettings,
                _features = features
            );
        }

        /// <summary>
        /// WIP v15.
        /// Later it should be built using a list of services that provide settings to the UI.
        /// - put gps coordinates in static
        /// - later get from settings
        /// </summary>
        /// <returns></returns>
        public EditSettingsDto GetSettings(IContextOfApp contextOfApp, List<IContentType> contentTypes,
            List<JsonContentType> jsonTypes, AppRuntime appRuntime) => Log.Func(() =>
        {
            var values = SettingsValuesCustom(contextOfApp);
            var parameters = SettingsValuesFromContentType(contextOfApp, contentTypes);

            foreach (var p in parameters) 
                values[p.Key] = p.Value;

            var settings = new EditSettingsDto
            {
                Values = values,
                Entities = SettingsEntities(jsonTypes, appRuntime),
            };
            return settings;
        });

        private Dictionary<string, object> SettingsValuesCustom(IContextOfApp contextOfApp) => Log.Func(l =>
        {
            var coordinates = MapsCoordinates.Defaults;
            try
            {
                if (_features.Value.IsEnabled(BuiltInFeatures.EditUiGpsCustomDefaults.NameId))
                {
                    var getMaps = contextOfApp.AppSettings.InternalGetPath(_googleMapsSettings.SettingsIdentifier);
                    coordinates = getMaps.GetFirstResultEntity() is IEntity mapsEntity
                        ? _googleMapsSettings.Init(mapsEntity).DefaultCoordinates
                        : MapsCoordinates.Defaults;
                }

                return new Dictionary<string, object>(InvariantCultureIgnoreCase)
                {
                    { _googleMapsSettings.SettingsIdentifier + "." + nameof(_googleMapsSettings.DefaultCoordinates), coordinates }
                };
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return new Dictionary<string, object>();
            }
        });

        public List<JsonEntity> SettingsEntities(List<JsonContentType> jsonTypes, AppRuntime appRuntime) => Log.Func(l =>
        {
            try
            {
                var hasWysiwyg = jsonTypes.SelectMany(
                    ct => ct.Attributes.Where(at => at.InputType.ContainsInsensitive("wysiwyg"))
                ).ToList();

                if (!hasWysiwyg.Any()) return (new List<JsonEntity>(), "no wysiwyg");

                var entities = appRuntime.Entities
                    .GetWithParentAppsExperimental("StringWysiwygConfiguration")
                    .ToList();

                var jsonSerializer = _jsonSerializerGenerator.Value.SetApp(appRuntime.AppState);
                var result = entities.Select(e => jsonSerializer.ToJson(e)).ToList();

                return (result, $"{result.Count}");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return (new List<JsonEntity>(), "error");
            }
        });

        private IDictionary<string, object> SettingsValuesFromContentType(IContextOfApp contextOfApp, List<IContentType> contentTypes) => Log.Func(l =>
        {
            try
            {
                // TODO: maybe check for feature?

                // find all keys which may be necessary
                var settingsKeys = contentTypes
                    .SelectMany(ct => (ct.Metadata.DetailsOrNull?.AdditionalSettings ?? "")
                        .Split(',')
                        .Select(s => s.Trim())
                    )
                    .Where(c => !IsNullOrWhiteSpace(c))
                    // Only include settings which have the full path
                    // so in future we can add other roots like resources
                    .Where(s => s.StartsWith($"{ConfigurationConstants.RootNameSettings}."))
                    .ToList();

                // Try to find each setting
                var settings = SettingsByKeys(contextOfApp.AppSettings, settingsKeys);

                return (settings, $"{settings.Count}");
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                return (new Dictionary<string, object>(), "error");
            }
        });

        private IDictionary<string, object> SettingsByKeys(PropertyStack appSettings, List<string> keys) => Log.Func(l => 
        {
            // Try to find each setting
            var settings = keys.ToDictionary(
                key => key,
                key => appSettings.InternalGetPath(key).Result
            );

            return (settings, $"{settings.Count}");
        });
    }
}
