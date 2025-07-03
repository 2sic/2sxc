using System.Collections.Immutable;
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
    DynamicOut = false,
    ConfigurationType = "" // TODO: ...
)]
[PrivateApi("Was till v17 InternalApi_DoNotUse_MayChangeWithoutNotice(still wip / finishing specs etc.)")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamFiles : CustomDataSourceAdvanced
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

        ProvideOut(GetInternal);
        ProvideOut(GetFolders, "Folders");
        ProvideOut(GetFiles, "Files");
    }
    #endregion

    private IImmutableList<IEntity> GetFolders() => GetInternal()
        .Where(e => e.Get<bool>("IsFolder"))
        .ToImmutableOpt();

    private IImmutableList<IEntity> GetFiles() => GetInternal()
        .Where(e => !e.Get<bool>("IsFolder"))
        .ToImmutableOpt();

    private IImmutableList<IEntity> GetInternal() => _getInternal.Get(() =>
    {
        var l = Log.Fn<IImmutableList<IEntity>>(timer: true);
        Configuration.Parse();

        // Make sure we have an In - otherwise error
        var source = TryGetIn();
        if (source is null)
            return l.Return(Error.TryGetInFailed(), "error");

        _provider.Configure(appId: AppId, entityIds: EntityIds, entityGuids: EntityGuids, fields: Fields,
            filter: Filter);
        var find = _provider.GetInternal();

        var adamFactory = DataFactory.SpawnNew(options: AdamItemDataRaw.Options with { AppId = AppId });

        var entities = adamFactory.Create(source.SelectMany(o => find(o)));

        return l.Return(entities, "ok");
    })!;
    private readonly GetOnce<IImmutableList<IEntity>> _getInternal = new();

}