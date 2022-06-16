using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract class Api14<TModel, TServiceKit>: Api12, IDynamicCode<TModel, TServiceKit>
        where TModel : class
        where TServiceKit : ServiceKit
    {
        public TModel Model => !(_DynCodeRoot is IDynamicCode<TModel, TServiceKit> root) ? default : root.Model;

        public TServiceKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TServiceKit>());
        private readonly ValueGetOnce<TServiceKit> _kit = new ValueGetOnce<TServiceKit>();

    }
}
