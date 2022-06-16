using ToSic.Eav.Plumbing;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    public abstract class DynamicCode<TModel, TServiceKit>: DynamicCode, IDynamicCode<TModel, TServiceKit>
        where TModel : class
        where TServiceKit : ServiceKit
    {
        public TModel Model => !(_DynCodeRoot is IDynamicCode<TModel, TServiceKit> root) ? default : root.Model;

        public TServiceKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TServiceKit>());
        private readonly ValueGetOnce<TServiceKit> _kit = new ValueGetOnce<TServiceKit>();
    }
}
