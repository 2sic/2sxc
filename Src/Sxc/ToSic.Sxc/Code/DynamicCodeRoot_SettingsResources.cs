using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using static ToSic.Eav.Configuration.ConfigurationStack;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        /// <inheritdoc />
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic Resources => _resources ?? (_resources = new DynamicStack(
                new DynamicEntityDependencies(Block,
                    DataSourceFactory.ServiceProvider,
                    CmsContext.SafeLanguagePriorityCodes()),
                new KeyValuePair<string, IPropertyLookup>(PartView, Block?.View?.Resources),
                new KeyValuePair<string, IPropertyLookup>(PartApp, App?.Resources?.Entity))
            );
        private dynamic _resources;

        /// <inheritdoc />
        [PublicApi("Careful - still Experimental in 12.02")]
        public dynamic Settings
        {
            get
            {
                if (_settings != null) return _settings;
                var currentAppState = ((App)_DynCodeRoot.App).AppState;

                var sources = new List<KeyValuePair<string, IPropertyLookup>>();
                
                // View level
                if (_DynCodeRoot.Block?.View?.Settings != null)
                    sources.Add(new KeyValuePair<string, IPropertyLookup>(PartView, _DynCodeRoot.Block?.View?.Settings));
                
                // All in the App and below
                sources.AddRange(currentAppState.SettingsInApp.SettingsStackForThisApp());
                
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
