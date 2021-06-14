using System.Collections.Generic;
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
        public dynamic Settings => _settings ?? (_settings = new DynamicStack(
                new DynamicEntityDependencies(_DynCodeRoot.Block,
                    _DynCodeRoot.DataSourceFactory.ServiceProvider,
                    CmsContext.SafeLanguagePriorityCodes()),
                new KeyValuePair<string, IPropertyLookup>(SourceNameView, _DynCodeRoot.Block?.View?.Settings),
                new KeyValuePair<string, IPropertyLookup>(SourceNameApp, _DynCodeRoot.App?.Settings?.Entity),
                new KeyValuePair<string, IPropertyLookup>(SourceNameAppSystem, (_DynCodeRoot.App as Eav.Apps.App)?.AppState.SystemSettings),
                new KeyValuePair<string, IPropertyLookup>(SourceNamePreset, Eav.Configuration.Global.Settings))
            );
        private dynamic _settings;
    }
}
