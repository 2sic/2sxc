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
        public dynamic Resources
        {
            get
            {
                if (_resources != null) return _resources;
                var appState = ((App)_DynCodeRoot.App).AppState;

                return _resources ?? (_resources = new DynamicStack(
                        AppConstants.RootNameResources,
                        DynamicEntityDependencies,
                        appState.SettingsInApp.GetStack(false, _DynCodeRoot.Block?.View?.Resources).ToArray())
                    );
            }
        }

        private dynamic _resources;

        /// <inheritdoc />
        [PublicApi]
        public dynamic Settings
        {
            get
            {
                if (_settings != null) return _settings;
                var appState = ((App)_DynCodeRoot.App).AppState;
                
                return _settings = new DynamicStack(
                    AppConstants.RootNameSettings,
                    DynamicEntityDependencies,
                    appState.SettingsInApp.GetStack(true, _DynCodeRoot.Block?.View?.Settings).ToArray());
            }
        }

        private dynamic _settings;
    }
}
