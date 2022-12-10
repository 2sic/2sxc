using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Data;
using static ToSic.Eav.Configuration.ConfigurationConstants;

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
                    RootNameResources,
                    DynamicEntityDependencies,
                    Deps.SettingsStack.Init(appState)
                        .GetStack(ConfigurationConstants.Resources, _DynCodeRoot.Block?.View?.Resources));
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
                    RootNameSettings,
                    DynamicEntityDependencies,
                    Deps.SettingsStack.Init(appState)
                        .GetStack(ConfigurationConstants.Settings, _DynCodeRoot.Block?.View?.Settings));
            }
        }

        private DynamicStack _settings;
    }
}
