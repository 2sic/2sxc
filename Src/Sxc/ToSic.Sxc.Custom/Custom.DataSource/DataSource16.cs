using System.Collections.Immutable;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.DataSource;

using ToSic.Eav.DataSource.Sys;
using ToSic.Eav.DataSources;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;


namespace Custom.DataSource;

/// <summary>
/// The Base Class for custom Dynamic DataSources in your App.
/// </summary>
[PublicApi]
public abstract partial class DataSource16: ServiceBase<DataSource16.Dependencies>, IDataSource, IAppIdentitySync
{
    /// <summary>
    /// Dependencies of DataSource16.
    /// </summary>
    /// <remarks>
    /// This ensures that all users must have this in the constructor, so we can be sure we can add more dependencies as we need them.
    ///
    /// Note that this used to be called `MyServices` and that term will still work, but it's deprecated as of v20.
    ///
    /// See [](xref:NetCode.Conventions.DependenciesClass).
    /// </remarks>
    [PublicApi]
    [method: PrivateApi]
    public class Dependencies(CustomDataSource.Dependencies parentServices, ServiceKitLight16 kit)
        : DependenciesBase(connect: [kit])
    {
        [PrivateApi]
        public CustomDataSource.Dependencies ParentServices { get; } = parentServices;
        [PrivateApi]
        public ServiceKitLight16 Kit { get; } = kit;
    }

    /// <summary>
    /// This is just for compatibility for any custom data sources which may have used the term `MyServices` since v16.
    /// </summary>
    /// <param name="parentServices"></param>
    /// <param name="kit"></param>
    [PrivateApi]
    public class MyServices(CustomDataSource.Dependencies parentServices, ServiceKitLight16 kit)
        : Dependencies(parentServices, kit);

    /// <summary>
    /// Constructor with the option to provide a log name.
    /// </summary>
    /// <param name="services">All the needed services - see [](xref:NetCode.Conventions.MyServices)</param>
    /// <param name="logName">Optional name for logging such as `My.JsonDS`</param>
    protected DataSource16(Dependencies services, string? logName = default): base(services, logName ?? "Cus.HybDs")
    {
        _inner = BreachExtensions.CustomDataSourceLight(services.ParentServices, this, logName: logName ?? "Cus.HybDs");
        _inner.BreachProvideOut(GetDefault);
        Kit = services.Kit.Setup(this, () => Configuration.LookUpEngine);
    }
    private readonly CustomDataSource _inner;

    /// <summary>
    /// A simplified (light) Kit containing a bunch of helpers.
    /// </summary>
    /// <remarks>
    /// This Kit has fewer APIs than in the typical Razor Kits,
    /// because many of the Razor APIs require a Razor context.
    /// </remarks>
    public ServiceKitLight16 Kit { get; }

    /// <summary>
    /// Optional method to provide default data.
    /// You can override this, or use one or more <see cref="ProvideOut"/> in your constructor.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerable<IRawEntity> GetDefault() => new List<IRawEntity>();

    /// <summary>
    /// Provide out-data on this data source.
    /// Typically called in the constructor.
    ///
    /// You can call this multiple times, providing different names.
    /// </summary>
    /// <param name="getList"></param>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="name"></param>
    /// <param name="options"></param>
    protected void ProvideOut(
        Func<object> getList,
        NoParamOrder npo = default,
        string name = DataSourceConstants.StreamDefaultName,
        Func<DataFactoryOptions>? options = default
    )
        => _inner.BreachProvideOut(getList, name: name, options: options);


    #region CodeLog

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => _codeLog.Get(() => new CodeLog(_inner.Log))!;
    private readonly GetOnce<ICodeLog> _codeLog = new();

    #endregion

    #region Public IDataSource Implementation

    public IDataSourceConfiguration Configuration => _inner.Configuration;

    public DataSourceErrorHelper Error => _inner.Error;

    public int ZoneId => _inner.ZoneId;

    public int AppId => _inner.AppId;

    #endregion


    #region IDataTarget - allmost all hidden

    /// <inheritdoc/>
    public IImmutableList<IEntity>? TryGetIn(string name = DataSourceConstants.StreamDefaultName)
        => _inner.TryGetIn(name);

    /// <inheritdoc/>
    public IImmutableList<IEntity>? TryGetOut(string name = DataSourceConstants.StreamDefaultName)
        => _inner.TryGetOut(name);

    // The rest is all explicit implementation only

    IReadOnlyDictionary<string, IDataStream> IDataSource.In => _inner.In;

    // todo: attach must error - but only once the query has been optimized
    // note also that temporarily the old interface IDataTarget will already error
    // but soon the new one must too
    private static readonly string AttachNotSupported = $"Attach(...) is not supported on new data sources. Provide 'attach:' in CreateDataSource(...) instead";
    void IDataTarget.Attach(IDataSource dataSource)
        => _inner.Attach(dataSource);

    void IDataTarget.Attach(string streamName, IDataSource dataSource, string sourceName)
        => _inner.Attach(streamName, dataSource, sourceName ?? DataSourceConstants.StreamDefaultName);

    void IDataTarget.Attach(string streamName, IDataStream dataStream)
        => _inner.Attach(streamName, dataStream);

    #endregion

    void IAppIdentitySync.UpdateAppIdentity(IAppIdentity appIdentity)
        => ((IAppIdentitySync)_inner).UpdateAppIdentity(appIdentity);

}