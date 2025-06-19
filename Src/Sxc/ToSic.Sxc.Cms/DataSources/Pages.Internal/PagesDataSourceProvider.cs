using ToSic.Lib.Services;
using ToSic.Sxc.Cms.Pages.Internal;

namespace ToSic.Sxc.DataSources.Internal;

/// <summary>
/// Base class to provide data to the Pages DataSource.
///
/// Must be overriden in each platform.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class PagesDataSourceProvider(string logName, object[]? connect = default) : ServiceBase(logName, connect: connect)
{
    public const int NoParent = 0;

    /// <summary>
    /// FYI: The filters are not actually implemented yet.
    /// So the core data source doesn't have settings to configure this
    /// </summary>
    /// <returns></returns>
    public abstract List<PageModelRaw> GetPagesInternal(
        NoParamOrder noParamOrder = default,
        bool includeHidden = default,
        bool includeDeleted = default,
        bool includeAdmin = default,
        bool includeSystem = default,
        bool includeLinks = default,
        bool requireViewPermissions = true,
        bool requireEditPermissions = true
    );
}