using System.Collections;
using System.Collections.Immutable;
using ToSic.Eav.Apps;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.DataSources;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services;

// ReSharper disable once CheckNamespace
namespace Custom.DataSource;

/// <summary>
/// The Base Class for custom Dynamic DataSources in your App.
/// </summary>
[PublicApi]
public abstract partial class DataSource16: ServiceBase<DataSource16.MyServices>, IDataSource, IAppIdentitySync
{
    /// <summary>
    /// These are dependencies of DataSource15.
    /// At the moment we could use something else, but this ensures that all users must have this
    /// in the constructor, so we can be sure we can add more dependencies as we need them.
    /// </summary>
    [PrivateApi]
    public class MyServices: MyServicesBase<CustomDataSource.MyServices>
    {
        public ServiceKitLight16 Kit { get; }

        public MyServices(CustomDataSource.MyServices parentServices, ServiceKitLight16 kit) : base(parentServices)
        {
            ConnectLogs([
                Kit = kit
            ]);
        }
    }

    /// <summary>
    /// Constructor with the option to provide a log name.
    /// </summary>
    /// <param name="services">All the needed services - see [](xref:NetCode.Conventions.MyServices)</param>
    /// <param name="logName">Optional name for logging such as `My.JsonDS`</param>
    protected DataSource16(MyServices services, string logName = default): base(services, logName ?? "Cus.HybDs")
    {
        _inner = BreachExtensions.CustomDataSourceLight(services.ParentServices, this, logName: logName ?? "Cus.HybDs");
        _inner.BreachProvideOut(GetDefault);
        Kit = services.Kit.Setup(this, () => Configuration.LookUpEngine);
    }
    private readonly CustomDataSource _inner;

    public ServiceKitLight16 Kit { get; }

    protected virtual IEnumerable<IRawEntity> GetDefault() => new List<IRawEntity>();


    protected void ProvideOut(
        Func<object> getList,
        NoParamOrder noParamOrder = default,
        string name = DataSourceConstants.StreamDefaultName,
        Func<DataFactoryOptions> options = default
    )
        => _inner.BreachProvideOut(getList, name: name, options: options);


    #region CodeLog

    /// <inheritdoc cref="IHasCodeLog.Log" />
    public new ICodeLog Log => _codeLog.Get(() => new CodeLog(_inner.Log));
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
    public IImmutableList<IEntity> TryGetIn(string name = DataSourceConstants.StreamDefaultName) => _inner.TryGetIn(name);

    /// <inheritdoc/>
    public IImmutableList<IEntity> TryGetOut(string name = DataSourceConstants.StreamDefaultName) => _inner.TryGetOut(name);

    // The rest is all explicit implementation only

    IReadOnlyDictionary<string, IDataStream> IDataSource.In => _inner.In;

    // todo: attach must error - but only once the query has been optimized
    // note also that temporarily the old interface IDataTarget will already error
    // but soon the new one must too
    private static readonly string AttachNotSupported = $"Attach(...) is not supported on new data sources. Provide 'attach:' in CreateDataSource(...) instead";
    void IDataTarget.Attach(IDataSource dataSource) => _inner.Attach(dataSource);

    void IDataTarget.Attach(string streamName, IDataSource dataSource, string sourceName) => _inner.Attach(streamName, dataSource, sourceName ?? DataSourceConstants.StreamDefaultName);

    void IDataTarget.Attach(string streamName, IDataStream dataStream) => _inner.Attach(streamName, dataStream);

    #endregion

    void IAppIdentitySync.UpdateAppIdentity(IAppIdentity appIdentity) => ((IAppIdentitySync)_inner).UpdateAppIdentity(appIdentity);

    //IEnumerator<IEntity> IEnumerable<IEntity>.GetEnumerator() => _inner.List.GetEnumerator();

    //IEnumerator IEnumerable.GetEnumerator() => _inner.List.GetEnumerator();
}