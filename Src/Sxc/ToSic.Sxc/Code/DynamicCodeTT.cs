using ToSic.Eav.Plumbing;
using ToSic.Sxc.Services.Kits;

namespace ToSic.Sxc.Code
{
    public abstract class DynamicCode<TModel, TKit>: DynamicCode, IDynamicCode<TModel, TKit>
        where TKit : KitBase
    {
        public TModel Model => !(_DynCodeRoot is IDynamicCode<TModel, TKit> root) ? default : root.Model;

        public TKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TKit>());
        private readonly ValueGetOnce<TKit> _kit = new ValueGetOnce<TKit>();
    }
}
