using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        internal const string SourceNameView = "View";
        internal const string SourceNameApp = "App";
        internal const string SourceNameAppSystem = "AppSystem";
        internal const string SourceNameSite = "Site";
        internal const string SourceNameSiteSystem = "SiteSystem";
        internal const string SourceNameGlobal = "Global";
        internal const string SourceNameGlobalSystem = "GlobalSystem";
        internal const string SourceNamePreset = "Preset";
        
        /// <inheritdoc />
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic Resources => _resources ?? (_resources = new DynamicStack(
                new DynamicEntityDependencies(Block,
                    DataSourceFactory.ServiceProvider,
                    CmsContext.SafeLanguagePriorityCodes()),
                new KeyValuePair<string, IPropertyLookup>(SourceNameView, Block?.View?.Resources),
                new KeyValuePair<string, IPropertyLookup>(SourceNameApp, App?.Resources?.Entity))
            );
        private dynamic _resources;

        /// <inheritdoc />
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic Settings
        {
            get
            {
                if (_settings != null) return _settings;
                var currentAppState = (_DynCodeRoot.App as App)?.AppState;
                var primaryAppState = State.Get(State.Identity(App.ZoneId, null));
                var globalAppState = State.Get(State.Identity(null, null));

                var sources = new List<KeyValuePair<string, IPropertyLookup>>();

                void AddIfNotNull(string name, IPropertyLookup lookup)
                {
                    if(lookup!=null) sources.Add(new KeyValuePair<string, IPropertyLookup>(name, lookup));
                }
                
                // View level
                AddIfNotNull(SourceNameView, _DynCodeRoot.Block?.View?.Settings);
                
                // App level
                AddIfNotNull(SourceNameApp, _DynCodeRoot.App?.Settings?.Entity);
                AddIfNotNull(SourceNameAppSystem, currentAppState?.SystemSettingsApp);
                
                // Site level
                AddIfNotNull(SourceNameSite, primaryAppState?.SiteSettingsCustom);
                AddIfNotNull(SourceNameSiteSystem, primaryAppState?.SystemSettingsSite);
                
                // Global
                AddIfNotNull(SourceNameGlobal, globalAppState?.GlobalSettingsCustom);
                AddIfNotNull(SourceNameGlobalSystem, globalAppState?.SystemSettingsApp);
                
                // System Presets
                AddIfNotNull(SourceNamePreset, Eav.Configuration.Global.Settings);
                
                return _settings = new DynamicStack(
                        new DynamicEntityDependencies(_DynCodeRoot.Block,
                            _DynCodeRoot.DataSourceFactory.ServiceProvider,
                            CmsContext.SafeLanguagePriorityCodes()),
                        sources.ToArray());
            }
        }

        private dynamic _settings;
    }
}
