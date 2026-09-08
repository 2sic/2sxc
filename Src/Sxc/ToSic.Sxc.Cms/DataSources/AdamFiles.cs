using System.Collections.Immutable;
using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Sys;
using ToSic.Eav.DataSource.VisualQuery;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources;

/// <summary>
/// Deliver a list of App files and folders from the current platform (Dnn or Oqtane).
///
/// As of now there are no parameters to set.
///
/// To figure out the properties returned and what they match up to, see <see cref="AdamItemDataRaw"/> TODO
/// </summary>
[VisualQuery(
    NiceName = "Adam",
    UiHint = "Files and folders in the Adam",
    NameId = "ee1d0cb6-5086-4d59-b16a-d0dc7b594bf2",
    HelpLink = "https://go.2sxc.org/ds-adam",
    Icon = DataSourceIcons.Tree,
    Type = DataSourceType.Lookup,
    Audience = Audience.Advanced,
    In = [DataSourceConstants.InStreamDefaultRequired],
    ConfigurationType = "" // TODO: ...
)]
[PrivateApi("Was till v17 InternalApi_DoNotUse_MayChangeWithoutNotice(still wip / finishing specs etc.)")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamFiles : CustomDataSource
{
    private readonly AdamDataSourceProvider<int, int> _provider;

    #region Configuration properties

    /// <summary>
    /// Uses the [immutable convention](xref:NetCode.Conventions.Immutable).
    /// </summary>
    [Configuration]
    public string? EntityIds => Configuration.GetThis();

    /// <summary>
    /// Uses the [immutable convention](xref:NetCode.Conventions.Immutable).
    /// </summary>
    [Configuration]
    public string? EntityGuids => Configuration.GetThis();

    /// <summary>
    /// Uses the [immutable convention](xref:NetCode.Conventions.Immutable).
    /// </summary>
    [Configuration]
    public string? Fields => Configuration.GetThis();

    /// <summary>
    /// Uses the [immutable convention](xref:NetCode.Conventions.Immutable).
    /// </summary>
    [Configuration(Fallback = "*.*")]
    public string? Filter => Configuration.GetThis();

    #endregion

    #region Constructor

    [PrivateApi]
    public AdamFiles(Dependencies services, AdamDataSourceProvider<int, int> provider) : base(services, "CDS.Adam", connect: [provider])
    {
        _provider = provider;

        ProvideOut(GetInternal, options: Options);
        ProvideOut(GetFolders, name: "Folders", options: Options);
        ProvideOut(GetFiles, name: "Files", options: Options);
    }
    #endregion

    private DataFactoryOptions Options() => new() { AppId = AppId };

    private object GetFolders()
        => GetInternal() is IImmutableList<AdamItemDataRaw> items
            ? items.Where(e => e.IsFolder).ToImmutableOpt()
            : GetInternal();

    private object GetFiles()
        => GetInternal() is IImmutableList<AdamItemDataRaw> items
            ? items.Where(e => !e.IsFolder).ToImmutableOpt()
            : GetInternal();

    private object GetInternal() => _getInternal.Get(() =>
    {
        var l = Log.Fn<object>(timer: true);
        Configuration.Parse();

        // Make sure we have an In - otherwise error
        var source = TryGetIn();
        if (source is null)
            return l.Return(Error.TryGetInFailed(), "error");

        _provider.Configure(appId: AppId, entityIds: EntityIds, entityGuids: EntityGuids, fields: Fields,
            filter: Filter);
        var find = _provider.GetInternal();

        var items = source.SelectMany(o => find(o)).ToImmutableOpt();
        return l.Return(items, "ok");
    })!;
    
    private readonly LazyGet<object> _getInternal = new();

}