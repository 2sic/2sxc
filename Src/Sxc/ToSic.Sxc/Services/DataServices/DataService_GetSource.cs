using ToSic.Eav.DataSource;
using ToSic.Lib.Logging;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Services
{
    public partial class DataService
    {

        // IMPORTANT - this is different! from the _DynCodeRoot - as it shouldn't auto attach at all!
        public T GetSource<T>(
            string noParamOrder = Protector,
            IDataSourceLinkable attach = default,
            object parameters = default,
            object options = default) where T : IDataSource
        {
            var l = Log.Fn<T>($"{nameof(attach)}: {attach}, {nameof(options)}: {options}");
            Protect(noParamOrder, $"{nameof(attach)}, {nameof(parameters)}, {nameof(options)}");

            // If no in-source was provided, make sure that we create one from the current app
            var fullOptions = SafeOptions(parameters, options: options);
            var ds = _dataSources.Value.Create<T>(attach: attach, options: fullOptions);

            return l.Return(ds);
        }

        public IDataSource GetSource(
            string noParamOrder = Protector,
            string name = null,
            IDataSourceLinkable attach = null,
            object parameters = default,
            object options = null)
        {
            var l = Log.Fn<IDataSource>($"{nameof(name)}: {name}, {nameof(attach)}: {attach}, {nameof(options)}: {options}");
            Protect(noParamOrder, $"{nameof(attach)}, {nameof(parameters)}, {nameof(options)}");
            // Do this first, to ensure AppIdentity is really known/set
            var safeOptions = SafeOptions(parameters, options: options);
            var type = _catalog.Value.FindDataSourceInfo(name, safeOptions.AppIdentity.AppId)?.Type;

            var ds = _dataSources.Value.Create(type, attach: attach, options: safeOptions);
            return l.Return(ds);
        }
    }
}
