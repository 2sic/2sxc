using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;

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

                return _resources = new DynamicStack(
                    AppConstants.RootNameResources,
                    DynamicEntityDependencies,
                    Deps.SettingsStack.Init(Log).Init(appState)
                        .GetStack(ConfigurationConstants.Resources, _DynCodeRoot.Block?.View?.Resources).ToArray());
            }
        }

        private DynamicStack _resources;

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
                    Deps.SettingsStack.Init(Log).Init(appState)
                        .GetStack(ConfigurationConstants.Settings, _DynCodeRoot.Block?.View?.Settings).ToArray());
            }
        }

        private DynamicStack _settings;
    }
}
