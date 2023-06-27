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
            AsC.AsStack(RootNameResources, Services.SettingsStack
                .Init(App.AppState)
                .GetStack(ConfigurationConstants.Resources, Block?.View?.Resources)));
        private readonly GetOnce<DynamicStack> _resources = new GetOnce<DynamicStack>();


        /// <inheritdoc />
        [PublicApi]
        public DynamicStack Settings => _settings.Get(() =>
            AsC.AsStack(RootNameSettings, Services.SettingsStack
                .Init(App.AppState)
                .GetStack(ConfigurationConstants.Settings, Block?.View?.Settings)));
        private readonly GetOnce<DynamicStack> _settings = new GetOnce<DynamicStack>();

        dynamic IDynamicCode12.Resources => Resources;
        dynamic IDynamicCode12.Settings => Settings;
    }
}
