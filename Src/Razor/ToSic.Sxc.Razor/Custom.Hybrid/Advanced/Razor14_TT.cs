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
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract class Razor14<TModel, TServiceKit>: Razor12<TModel>, IRazor14<TModel, TServiceKit>
        where TModel : class
        where TServiceKit : ServiceKit
    {
        public TServiceKit Kit => _kit.Get(() => _DynCodeRoot.GetKit<TServiceKit>());
        private readonly GetOnce<TServiceKit> _kit = new();

        [PrivateApi]
        public new T CreateDataSource<T>(string noParamOrder = Protector, IDataSourceLinkable attach = null, object options = default) where T : IDataSource
            => _DynCodeRoot.CreateDataSource<T>(noParamOrder: noParamOrder, attach: attach, options: options);

        [PrivateApi]
        public new IDataSource CreateDataSource(string noParamOrder = Protector, string name = default, IDataSourceLinkable attach = null, object options = null)
            => _DynCodeRoot.CreateDataSource(noParamOrder: noParamOrder, name: name, attach: attach, options: options);


    }
}
