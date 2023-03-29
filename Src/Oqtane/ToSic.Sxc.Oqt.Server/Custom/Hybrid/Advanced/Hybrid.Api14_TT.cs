using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid.Advanced
{
    /// <summary>
    /// Oqtane specific Api base class.
    ///
    /// As of 2sxc v12 it's identical to [](xref:Custom.Hybrid.Api12) but this may be enhanced in future. 
    /// </summary>
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract class Api14<TModel, TServiceKit> : Api12, IDynamicCode<TModel, TServiceKit>
        where TModel : class
        where TServiceKit : ServiceKit
    {
        public TModel Model => _DynCodeRoot is not IDynamicCode<TModel, TServiceKit> root ? default : root.Model;

        public TServiceKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TServiceKit>());
        private readonly GetOnce<TServiceKit> _kit = new();

    }
}