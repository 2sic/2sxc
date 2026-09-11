using System.Collections.Immutable;
using ToSic.Eav.Data.Build;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Sys;
using ToSic.Eav.DataSource.Sys.Errors;
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

        ProvideOutRaw(() => Streams.All, options: Options);
        ProvideOutRaw(() => Streams.Folders, name: "Folders", options: Options);
        ProvideOutRaw(() => Streams.Files, name: "Files", options: Options);
    }
    #endregion

    private DataFactoryOptions Options() => new() { AppId = AppId };

    private StreamResults Streams => _streams ??= BuildStreams();
    private StreamResults? _streams;

    private StreamResults BuildStreams()
    {
        var l = Log.Fn<StreamResults>(timer: true);
        Configuration.Parse();

        // Make sure we have an In - otherwise error
        var source = TryGetIn();
        if (source is null)
        {
            var failed = new ResultOrError<IEnumerable<AdamItemDataRaw>>(false, null, Error.TryGetInFailed());
            return l.Return(new(failed, failed, failed), "error");
        }

        _provider.Configure(appId: AppId, entityIds: EntityIds, entityGuids: EntityGuids, fields: Fields,
            filter: Filter);
        var find = _provider.GetInternal();

        var items = source.SelectMany(o => find(o)).ToImmutableOpt();
        var folders = items.Where(item => item.IsFolder).ToImmutableOpt();
        var files = items.Where(item => !item.IsFolder).ToImmutableOpt();

        return l.Return(new(
            new(true, items),
            new(true, folders),
            new(true, files)
        ), $"all: {items.Count}, folders: {folders.Count}, files: {files.Count}");
    }

    private sealed record StreamResults(
        ResultOrError<IEnumerable<AdamItemDataRaw>> All,
        ResultOrError<IEnumerable<AdamItemDataRaw>> Folders,
        ResultOrError<IEnumerable<AdamItemDataRaw>> Files
    );

}
