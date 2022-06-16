using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.Oqtane
{
    /// <summary>
    /// Oqtane specific Api base class.
    ///
    /// As of 2sxc v12 it's identical to [](xref:Custom.Hybrid.Api12) but this may be enhanced in future. 
    /// </summary>
    [PublicApi]
    public abstract class Api14<TModel, TServiceKit> : Hybrid.Api12, IDynamicCode<TModel, TServiceKit>
        where TModel : class
        where TServiceKit : ServiceKit
    {
        public TModel Model => _DynCodeRoot is not IDynamicCode<TModel, TServiceKit> root ? default : root.Model;

        public TServiceKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TServiceKit>());
        private readonly ValueGetOnce<TServiceKit> _kit = new();

    }
}