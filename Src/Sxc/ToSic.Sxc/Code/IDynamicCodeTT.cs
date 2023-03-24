using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    [PrivateApi("WIP v14.02")]
    public interface IDynamicCode<out TModel, out TServiceKit>: IDynamicCode, IDynamicCodeKit<TServiceKit>
        where TModel : class
        where TServiceKit: ServiceKit
    {
        [PrivateApi]
        TModel Model { get; }

        #region DynamicCode New in v15 - probably available in v14 as well

        [PrivateApi]
        IDataSource CreateSourceWip(
            string name,
            string noParamOrder = Eav.Parameters.Protector,
            IDataSource source = default,
            object options = default);

        #endregion

    }
}
