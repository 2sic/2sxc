using Custom.Dnn;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Custom.Hybrid
{
    public abstract class Api14<TModel, TKit>: Api12, IDynamicCode<TModel, TKit>
        where TModel : class
        where TKit : KitBase
    {
        public TModel Model => !(_DynCodeRoot is IDynamicCode<TModel, TKit> root) ? default : root.Model;

        public TKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TKit>());
        private readonly ValueGetOnce<TKit> _kit = new ValueGetOnce<TKit>();

    }
}
