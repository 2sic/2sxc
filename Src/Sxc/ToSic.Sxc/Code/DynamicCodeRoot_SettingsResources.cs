using ToSic.Eav.Apps;
using ToSic.Eav.Configuration;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using static ToSic.Eav.Configuration.ConfigurationConstants;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        /// <inheritdoc />
        [PublicApi]
        public DynamicStack Resources => _resources.Get(() =>
        {
            var appState = ((App)_DynCodeRoot.App).AppState;
            return new DynamicStack(RootNameResources, AsC.DynamicEntityServices,
                Services.SettingsStack.Init(appState)
                    .GetStack(ConfigurationConstants.Resources, _DynCodeRoot.Block?.View?.Resources));
        });
        private readonly GetOnce<DynamicStack> _resources = new GetOnce<DynamicStack>();


        /// <inheritdoc />
        [PublicApi]
        public DynamicStack Settings => _settings.Get(() =>
        {
            var appState = ((App)_DynCodeRoot.App).AppState;
            return new DynamicStack(RootNameSettings, AsC.DynamicEntityServices,
                Services.SettingsStack.Init(appState)
                    .GetStack(ConfigurationConstants.Settings, _DynCodeRoot.Block?.View?.Settings));

        });
        private readonly GetOnce<DynamicStack> _settings = new GetOnce<DynamicStack>();

        dynamic IDynamicCode12.Resources => Resources;
        dynamic IDynamicCode12.Settings => Settings;
    }
}
