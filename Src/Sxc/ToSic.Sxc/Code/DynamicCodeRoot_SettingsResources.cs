using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Data;
using static ToSic.Eav.Configuration.ConfigurationStack;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        /// <inheritdoc />
        [PublicApi]
        public dynamic Resources => _resources ?? (_resources = new DynamicStack(
                DynamicEntityDependencies,
                new KeyValuePair<string, IPropertyLookup>(PartView, Block?.View?.Resources),
                new KeyValuePair<string, IPropertyLookup>(PartApp, App?.Resources?.Entity))
            );
        private dynamic _resources;

        /// <inheritdoc />
        [PublicApi]
        public dynamic Settings
        {
            get
            {
                if (_settings != null) return _settings;
                var currentAppState = ((App)_DynCodeRoot.App).AppState;

                var sources = new List<KeyValuePair<string, IPropertyLookup>>
                {
                    // View level - always add, no matter if null
                    new KeyValuePair<string, IPropertyLookup>(PartView, _DynCodeRoot.Block?.View?.Settings)
                };

                // All in the App and below
                sources.AddRange(currentAppState.SettingsInApp.SettingsStackForThisApp());
                
                return _settings = new DynamicStack(DynamicEntityDependencies, sources.ToArray());
            }
        }

        private dynamic _settings;
    }
}
