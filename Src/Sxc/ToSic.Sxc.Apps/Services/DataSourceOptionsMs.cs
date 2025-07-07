using ToSic.Eav.LookUp.Sys.Engines;

namespace ToSic.Sxc.Services.DataServices;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class DataSourceOptionsMs(IAppIdentity? identity, Func<ILookUpEngine?>? getLookup)
    : ServiceBase(SxcLogName + "DtOptH")
{
    private ILookUpEngine? LookUpEngine => _lookupEngine.Get(() => getLookup?.Invoke());
    private readonly GetOnce<ILookUpEngine?> _lookupEngine = new();

    public IDataSourceOptions SafeOptions(object? parameters, object? options, bool identityRequired = false)
    {
        var l = Log.Fn<IDataSourceOptions>($"{nameof(options)}: {options}, {nameof(identityRequired)}: {identityRequired}");
        // Ensure we have a valid AppIdentity
        var appIdentity = identity ?? (options as IDataSourceOptions)?.AppIdentityOrReader
            ?? (identityRequired
                ? throw new(
                    "Creating a DataSource requires an AppIdentity which must either be supplied by the context, " +
                    $"(the Module / WebApi call) or provided manually by spawning a new {nameof(IDataService)} with the AppIdentity using 'New(...).")
                : new AppIdentity(0, 0)
            );
        // Convert to a pure identity, in case the original object was much more
        appIdentity = new AppIdentity(appIdentity);
        var opts = new DataSourceOptionConverter().Create(new DataSourceOptions
        {
            AppIdentityOrReader = appIdentity,
            LookUp = LookUpEngine,
            Immutable = true,
        }, options);

        // Check if parameters were supplied, if yes, they override any values in the existing options (16.01)
        var values = new DataSourceOptionConverter().Values(original: parameters, throwIfNull: false, throwIfNoMatch: true);
        return values != null
            ? l.Return(opts with { Values = values }, "with values")
            : l.Return(opts, "without values");
    }
}