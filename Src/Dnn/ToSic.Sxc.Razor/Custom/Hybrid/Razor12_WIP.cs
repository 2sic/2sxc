using ToSic.Sxc.Context;
using ToSic.Sxc.Data;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public partial class Razor12
    {
        protected dynamic Resources => _resources ?? (_resources = new DynamicStack(
            new DynamicEntityDependencies(_DynCodeRoot.Block,
                _DynCodeRoot.DataSourceFactory.ServiceProvider, 
            CmsContext.SafeLanguagePriorityCodes()), 
            _DynCodeRoot.Block?.View?.Resources, 
            _DynCodeRoot.App?.Resources?.Entity));
        private dynamic _resources;
    }
}
