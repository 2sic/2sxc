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
        public EditSettingsDto GetSettings(IContextOfApp contextOfApp, EditDto editData, AppRuntime appRuntime) => Log.Func(() =>
        {
            var coordinates = MapsCoordinates.Defaults;

            if (_features.Value.IsEnabled(BuiltInFeatures.EditUiGpsCustomDefaults.NameId))
            {
                var getMaps = contextOfApp.AppSettings.InternalGetPath(_googleMapsSettings.SettingsIdentifier);
                coordinates = getMaps.GetFirstResultEntity() is IEntity mapsEntity
                    ? _googleMapsSettings.Init(mapsEntity).DefaultCoordinates
                    : MapsCoordinates.Defaults;
            }

            var valueSettings = new Dictionary<string, object>
            {
                { "gps-default-coordinates", coordinates }
            };
            var settings = new EditSettingsDto
            {
                Values = valueSettings,
                Entities = SettingsEntities(editData, appRuntime)
            };
            return settings;
        });

        public List<JsonEntity> SettingsEntities(EditDto editData, AppRuntime appRuntime) => Log.Func(l =>
        {
            var hasWysiwyg = editData.ContentTypes.SelectMany(
                ct => ct.Attributes.Where(at => at.InputType.ContainsInsensitive("wysiwyg"))
            ).ToList();

            if (!hasWysiwyg.Any()) return (new List<JsonEntity>(), "no wysiwyg");

            var entities = appRuntime.Entities
                .GetWithParentAppsExperimental("StringWysiwygConfiguration")
                .ToList();

            var jsonSerializer = _jsonSerializerGenerator.Value.SetApp(appRuntime.AppState);
            var result = entities.Select(e => jsonSerializer.ToJson(e)).ToList();

            return (result, $"{result.Count}");

        });

    }
}
