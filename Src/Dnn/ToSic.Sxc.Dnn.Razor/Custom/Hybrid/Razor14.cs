using ToSic.Sxc.Code;
using ToSic.Sxc.Services.Kits;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract class Razor14<TModel, TKit>: Razor12, IRazor14<TModel, TKit>
        where TKit : KitBase
    {
        public TModel Model => !(_DynCodeRoot is IDynamicCode<TModel, TKit> root) ? default : root.Model;

        public TKit Kit => (_DynCodeRoot as IDynamicCode<TModel, TKit>)?.Kit;

    }
}
