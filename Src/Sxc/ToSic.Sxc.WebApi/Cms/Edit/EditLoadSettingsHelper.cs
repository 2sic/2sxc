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
        private readonly ILazySvc<JsonSerializer> _jsonSerializerGenerator;

        public EditLoadSettingsHelper(
            ILazySvc<JsonSerializer> jsonSerializerGenerator,
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
            var values = SettingsValues(contextOfApp);
            var parameters = FormulaSettings(contextOfApp, contentTypes);

            foreach (var p in parameters) 
                values[p.Key] = p.Value;

            var settings = new EditSettingsDto
            {
                Values = values,
                Entities = SettingsEntities(jsonTypes, appRuntime),
            };
            return settings;
        });

        private Dictionary<string, object> SettingsValues(IContextOfApp contextOfApp) => Log.Func(l =>
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
                    { "gps-default-coordinates", coordinates },
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

        public IDictionary<string, object> FormulaSettings(IContextOfApp contextOfApp, List<IContentType> contentTypes) => Log.Func(l =>
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
                    .ToList();

                // try to extract keys from formulas
                // NOTE: this can't work, because formulas are edited and can try to look 
                // up new keys as they go - must reconsider...
                //const string UiFormulaTypeName = "UiFormula";
                //const string UiFormulaNameId = "772dfff1-b236-4aa9-8359-5f53c08ff7bf";
                //const string FormulaField = "Formula";
                //var attributeMetadata = contentTypes
                //    .SelectMany(ct => ct.Attributes
                //        .SelectMany(a => a.Metadata
                //            .OfType(UiFormulaTypeName)
                //            .Select(e => e.GetBestValue<string>(FormulaField, Array.Empty<string>()))
                //            .Where(str => !IsNullOrWhiteSpace(str)
                //            )
                //        )
                //    );



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
