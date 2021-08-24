using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.AppStack
{
    public class AppStackBackend: WebApiBackendBase<AppStackBackend>
    {

        #region Constructor / DI

        public AppStackBackend(IServiceProvider serviceProvider, ICmsContext cmsContext, IContextResolver ctxResolver) : base(serviceProvider, "Sxc.ApiApQ")
        {
            _cmsContext = cmsContext;
            _ctxResolver = ctxResolver;
        }

        private readonly ICmsContext _cmsContext;
        private readonly IContextResolver _ctxResolver;
        #endregion

        public List<StackInfoDto> GetStack(int appId, string name, string forView, string[] languages)
        {
            // ensure name is known
            if (!AppConstants.RootNameSettings.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                throw new Exception($"Parameter '{nameof(name)}' must be {AppConstants.RootNameSettings}");

            // Todo: get app 
            var appState = _ctxResolver.App(appId).AppState;

            // todo: build settings
            var sources = appState.SettingsInApp.SettingsStackForThisApp();
            var settings = new PropertyStack();
            settings.Init(AppConstants.RootNameSettings, sources.ToArray());

            if (languages == null || !languages.Any()) 
                languages = _cmsContext.SafeLanguagePriorityCodes();

            // todo: generate...
            var results = settings._Dump(languages, null, Log);

            return results
                .Select(r => new StackInfoDto()
                {
                    Path = r.Path,
                    Priority = r.SourcePriority,
                    Source = r.SourceName,
                    TotalResults = r.AllOptions?.Count ?? 0,
                    Type = r.Property.FieldType,
                    Value = r.Property.Result
                })
                .ToList();
        }
        
    }
}
