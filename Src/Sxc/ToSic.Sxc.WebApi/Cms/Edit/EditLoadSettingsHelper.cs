using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.ImportExport.Serialization;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using static System.StringComparer;

namespace ToSic.Sxc.WebApi.Cms
{
    public class EditLoadSettingsHelper: ServiceBase
    {
        #region Constructor / DI

        private readonly IEnumerable<ILoadSettingsProvider> _loadSettingsProviders;
        private readonly LazySvc<JsonSerializer> _jsonSerializerGenerator;

        public EditLoadSettingsHelper(
            LazySvc<JsonSerializer> jsonSerializerGenerator,
            IEnumerable<ILoadSettingsProvider> loadSettingsProviders
            ) : base(Constants.SxcLogName + ".LodSet")
        {
            ConnectServices(
                _jsonSerializerGenerator = jsonSerializerGenerator,
                _loadSettingsProviders = loadSettingsProviders
            );
        }

        #endregion

        /// <summary>
        /// WIP v15.
        /// Later it should be built using a list of services that provide settings to the UI.
        /// - put gps coordinates in static
        /// - later get from settings
        /// </summary>
        /// <returns></returns>
        public EditSettingsDto GetSettings(IContextOfApp contextOfApp, List<IContentType> contentTypes,
            List<JsonContentType> jsonTypes, AppRuntime appRuntime) => Log.Func(l =>
        {
            var allInputTypes = jsonTypes
                .SelectMany(ct => ct.Attributes.Select(at => at.InputType))
                .Distinct()
                .ToList();

            var lspParameters = new LoadSettingsProviderParameters
            {
                ContextOfApp = contextOfApp,
                ContentTypes = contentTypes,
                InputTypes = allInputTypes
            };

            var settingsFromProviders = _loadSettingsProviders.Select(lsp =>
                {
                    try
                    {
                        return lsp.LinkLog(Log).GetSettings(lspParameters);
                    }
                    catch (Exception e)
                    {
                        l.E($"Error on {lsp.GetType().Name}");
                        l.Ex(e);
                        return new Dictionary<string, object>();
                    }
                })
                .ToList();

            var finalSettings = new Dictionary<string, object>(InvariantCultureIgnoreCase);
            foreach (var pair in settingsFromProviders.SelectMany(sfp => sfp))
                finalSettings[pair.Key] = pair.Value;

            var settings = new EditSettingsDto
            {
                Values = finalSettings,
                Entities = SettingsEntities(appRuntime, allInputTypes),
            };
            return settings;
        });
        

        public List<JsonEntity> SettingsEntities(AppRuntime appRuntime, List<string> allInputTypes) => Log.Func(l =>
        {
            try
            {
                var hasWysiwyg = allInputTypes.Any(it => it.ContainsInsensitive("wysiwyg"));
                if (!hasWysiwyg)
                    return (new List<JsonEntity>(), "no wysiwyg field");

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
        
    }
}
