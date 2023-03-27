using Custom.Hybrid.Advanced;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Linking;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Services;
using static ToSic.Eav.Parameters;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract class Razor14: Razor14<dynamic, ServiceKit14>
    {
        [PrivateApi]
        public new T CreateDataSource<T>(string noParamOrder = Protector, IDataSourceLinkable attach = null, object options = default) where T : IDataSource
            => _DynCodeRoot.CreateDataSource<T>(noParamOrder: noParamOrder, attach: attach, options: options);

        [PrivateApi]
        public new IDataSource CreateDataSource(string noParamOrder = Protector, string name = default, IDataSourceLinkable attach = default, object options = default)
            => _DynCodeRoot.CreateDataSource(noParamOrder: noParamOrder, name: name, attach: attach, options: options);

    }
}
