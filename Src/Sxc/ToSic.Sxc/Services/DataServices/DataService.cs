using ToSic.Eav;
using ToSic.Eav.DataSource;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Services
{
    [PrivateApi("not yet ready / public")]
    public class DataService: ServiceForDynamicCode, IDataService
    {
        public DataService() : base("Sxc.DatSvc")
        {
        }

        // WIP - ATM the code is in _DynCodeRoot, but it should actually be moved here and removed there
        public T GetSource<T>(string noParamOrder = Parameters.Protector, IDataSourceLinkable attach = null, object options = default) where T : IDataSource
        {
            return _DynCodeRoot.DataSources.CreateDataSource<T>(true, noParamOrder: noParamOrder, attach: attach, options: options);
        }

        public IDataSource GetSource(string noParamOrder = Parameters.Protector, string name = default,
            IDataSourceLinkable attach = default, object options = default)
        {
            return _DynCodeRoot.DataSources.CreateDataSource(noParamOrder: noParamOrder, name: name, attach: attach, options: options);
        }
    }
}
