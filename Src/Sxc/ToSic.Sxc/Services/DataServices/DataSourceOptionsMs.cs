using System;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Services.DataServices
{
    internal class DataSourceOptionsMs: ServiceBase
    {
        internal DataSourceOptionsMs(IAppIdentity appIdentity, Func<ILookUpEngine> getLookup): base(Constants.SxcLogName + "DtOptH")
        {
            _appIdentity = appIdentity;
            _getLookup = getLookup;
        }
        private readonly IAppIdentity _appIdentity;
        private readonly Func<ILookUpEngine> _getLookup;

        private ILookUpEngine LookUpEngine => _lookupEngine.Get(() => _getLookup?.Invoke());
        private readonly GetOnce<ILookUpEngine> _lookupEngine = new GetOnce<ILookUpEngine>();

        public IDataSourceOptions SafeOptions(object parameters, object options, bool identityRequired = false)
        {
            var l = Log.Fn<IDataSourceOptions>($"{nameof(options)}: {options}, {nameof(identityRequired)}: {identityRequired}");
            // Ensure we have a valid AppIdentity
            var appIdentity = _appIdentity ?? (options as IDataSourceOptions)?.AppIdentity
                ?? (identityRequired
                    ? throw new Exception(
                        "Creating a DataSource requires an AppIdentity which must either be supplied by the context, " +
                        $"(the Module / WebApi call) or provided manually by spawning a new {nameof(IDataService)} with the AppIdentity using 'New(...).")
                    : new AppIdentity(0, 0)
                );
            // Convert to a pure identity, in case the original object was much more
            appIdentity = new AppIdentity(appIdentity);
            var opts = new DataSourceOptions.Converter().Create(new DataSourceOptions(appIdentity: appIdentity, lookUp: LookUpEngine, immutable: true), options);

            // Check if parameters were supplied, if yes, they override any values in the existing options (16.01)
            var values = new DataSourceOptions.Converter().Values(parameters, false, true);
            if (values != null) opts = new DataSourceOptions(opts, values: values);

            return l.Return(opts);
        }
    }
}
