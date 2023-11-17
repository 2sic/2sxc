using ToSic.Eav.Apps;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using static ToSic.Eav.Apps.AppStackConstants;
using SettingsSources = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, ToSic.Eav.Data.PropertyLookup.IPropertyLookup>>;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        /// <inheritdoc />
        [PublicApi]
        public DynamicStack Resources => _resources.Get(() => Cdf.AsDynStack(RootNameResources, ResSrc));
        private readonly GetOnce<DynamicStack> _resources = new GetOnce<DynamicStack>();

        [PrivateApi]
        public ITypedStack AllResources => _allRes.Get(() => Cdf.AsTypedStack(RootNameResources, ResSrc));
        private readonly GetOnce<ITypedStack> _allRes= new GetOnce<ITypedStack>();

        private SettingsSources ResSrc => _resSrc.Get(() =>
            Services.SettingsStack.Init(App.AppState)
                .GetStack(AppStackConstants.Resources, Block?.View?.Resources));
        private readonly GetOnce<SettingsSources> _resSrc = new GetOnce<SettingsSources>();


        private SettingsSources SetSrc => _setSrc.Get(() =>
            Services.SettingsStack.Init(App.AppState)
                .GetStack(AppStackConstants.Settings, Block?.View?.Settings));
        private readonly GetOnce<SettingsSources> _setSrc = new GetOnce<SettingsSources>();

        /// <inheritdoc />
        [PublicApi]
        public DynamicStack Settings => _settings.Get(() => Cdf.AsDynStack(RootNameSettings, SetSrc));
        private readonly GetOnce<DynamicStack> _settings = new GetOnce<DynamicStack>();

        public ITypedStack AllSettings => _allSettings.Get(() => Cdf.AsTypedStack(RootNameSettings, SetSrc));
        private readonly GetOnce<ITypedStack> _allSettings = new GetOnce<ITypedStack>();

        dynamic IDynamicCode12.Resources => Resources;
        dynamic IDynamicCode12.Settings => Settings;
    }
}
