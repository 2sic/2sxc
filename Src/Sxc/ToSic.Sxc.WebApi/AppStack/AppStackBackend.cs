using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using static ToSic.Eav.Apps.AppConstants;

namespace ToSic.Sxc.WebApi.AppStack
{
    public class AppStackBackend: WebApiBackendBase<AppStackBackend>
    {

        #region Constructor / DI

        public AppStackBackend(IServiceProvider serviceProvider, IContextResolver ctxResolver, AppSettingsStack settingsStack) : base(serviceProvider, "Sxc.ApiApQ")
        {
            _ctxResolver = ctxResolver;
            _settingsStack = settingsStack;
        }

        private readonly IContextResolver _ctxResolver;
        private readonly AppSettingsStack _settingsStack;

        #endregion

        public List<StackInfoDto> GetAll(int appId, string part, string key, Guid? viewGuid, string[] languages)
        {
            var results = GetStackDump(appId, part, viewGuid, languages);

            if (!string.IsNullOrEmpty(key))
            {
                var relevant = results.FirstOrDefault(r => r.Path.Equals(key, StringComparison.InvariantCultureIgnoreCase));
                if (relevant == null) return new List<StackInfoDto>();
                results = relevant.AllOptions;
            }

            return results
                .GroupBy(original => new {original.Path, original.SourceName}) // remove "duplicate" settings from results
                .Select(g => g.OrderByDescending(i => i.AllOptions?.Count ?? 0).First())
                .Select(r => new StackInfoDto(r))
                .ToList();
        }

        private List<PropertyDumpItem> GetStackDump(int appId, string part, Guid? viewGuid, string[] languages)
        {
            // Ensure name is known
            string realName = null;
            if (RootNameSettings.Equals(part, StringComparison.InvariantCultureIgnoreCase))
                realName = RootNameSettings;
            if (RootNameResources.Equals(part, StringComparison.InvariantCultureIgnoreCase))
                realName = RootNameResources;
            if (realName == null)
                throw new Exception(
                    $"Parameter '{nameof(part)}' must be {RootNameSettings} or {RootNameResources}");

            // Get app 
            var appState = _ctxResolver.App(appId).AppState;
            var siteContext = _ctxResolver.Site();

            // Correct languages
            if (languages == null || !languages.Any())
                languages = siteContext.Site.SafeLanguagePriorityCodes();

            IEntity viewStackPart = null;
            if (viewGuid != null)
            {
                var viewEnt = appState.List.One(viewGuid.Value);
                if (viewEnt == null) throw new Exception($"Tried to get view but not found. Guid was {viewGuid}");
                var view = new View(viewEnt, languages, Log);

                viewStackPart = realName == RootNameSettings ? view.Settings : view.Resources;
            }

            // Build Sources List
            var partId = part == RootNameSettings ? ConfigurationConstants.Settings : ConfigurationConstants.Resources;
            var sources = _settingsStack.Init(Log).Init(appState).GetStack(partId, viewStackPart);
            var settings = new PropertyStack().Init(part, sources);

            // Dump results
            var results = settings._Dump(new PropReqSpecs(null, languages, Log), null);
            return results;
        }
    }
}
