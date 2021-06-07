using ToSic.Eav.Documentation;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {
        /// <inheritdoc />
        [PrivateApi("WIP")]
        public dynamic Resources => _resources ?? (_resources = new DynamicStack(
            new DynamicEntityDependencies(Block,
                DataSourceFactory.ServiceProvider,
                CmsContext.SafeLanguagePriorityCodes()),
            Block?.View?.Resources,
            App?.Resources?.Entity));
        private dynamic _resources;

        /// <inheritdoc />
        [PrivateApi("WIP")]
        public dynamic Settings => _settings ?? (_settings = new DynamicStack(
            new DynamicEntityDependencies(_DynCodeRoot.Block,
                _DynCodeRoot.DataSourceFactory.ServiceProvider,
                CmsContext.SafeLanguagePriorityCodes()),
            _DynCodeRoot.Block?.View?.Settings,
            _DynCodeRoot.App?.Settings?.Entity));
        private dynamic _settings;
    }
}
