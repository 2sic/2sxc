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
            return AsC.AsStack(RootNameResources, Services.SettingsStack
                .Init(((App)_DynCodeRoot.App).AppState)
                .GetStack(ConfigurationConstants.Resources, _DynCodeRoot.Block?.View?.Resources));
            //var appState = ((App)_DynCodeRoot.App).AppState;
            //return new DynamicStack(RootNameResources, AsC.DynamicEntityServices,
            //    Services.SettingsStack.Init(appState)
            //        .GetStack(ConfigurationConstants.Resources, _DynCodeRoot.Block?.View?.Resources));
        });
        private readonly GetOnce<DynamicStack> _resources = new GetOnce<DynamicStack>();


        /// <inheritdoc />
        [PublicApi]
        public DynamicStack Settings => _settings.Get(() =>
        {
            return AsC.AsStack(RootNameSettings, Services.SettingsStack
                .Init(((App)_DynCodeRoot.App).AppState)
                .GetStack(ConfigurationConstants.Settings, _DynCodeRoot.Block?.View?.Settings));
            //var appState = ((App)_DynCodeRoot.App).AppState;
            //return new DynamicStack(RootNameSettings, AsC.DynamicEntityServices,
            //    Services.SettingsStack.Init(appState)
            //        .GetStack(ConfigurationConstants.Settings, _DynCodeRoot.Block?.View?.Settings));

        });
        private readonly GetOnce<DynamicStack> _settings = new GetOnce<DynamicStack>();

        dynamic IDynamicCode12.Resources => Resources;
        dynamic IDynamicCode12.Settings => Settings;
    }
}
