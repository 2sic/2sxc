using ToSic.Eav.Documentation;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public partial class Razor12
    {

        //protected dynamic Resources => _resources ?? (_resources = new DynamicStack(
        //    new DynamicEntityDependencies(_DynCodeRoot.Block,
        //        _DynCodeRoot.DataSourceFactory.ServiceProvider, 
        //    CmsContext.SafeLanguagePriorityCodes()), 
        //    _DynCodeRoot.Block?.View?.Resources, 
        //    _DynCodeRoot.App?.Resources?.Entity));
        //private dynamic _resources;
        [PrivateApi("WIP 12.02")]
        public dynamic Resources => _DynCodeRoot.Resources;

        [PrivateApi("WIP 12.02")]
        public dynamic Settings => _DynCodeRoot.Settings;
    }
}
