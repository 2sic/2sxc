using ToSic.Eav.Plumbing;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    public abstract class DynamicCode<TModel, TKit>: DynamicCode, IDynamicCode<TModel, TKit>
        where TModel : class
        where TKit : ServiceKit
    {
        public TModel Model => !(_DynCodeRoot is IDynamicCode<TModel, TKit> root) ? default : root.Model;

        public TKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TKit>());
        private readonly ValueGetOnce<TKit> _kit = new ValueGetOnce<TKit>();
    }
}
