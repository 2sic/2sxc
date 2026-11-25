using ToSic.Eav.LookUp.Sys.Engines;

namespace ToSic.Sxc.Apps.Sys;

/// <summary>
/// The configuration of an app-data - usually relevant so the source will auto-filter out unpublished data for normal viewers.
/// </summary>
/// <param name="LookUpEngine">
/// The lookup engine used to resolve data sources.
/// Will deliver url-parameters and other important configuration values.
/// </param>
/// <param name="ShowDrafts">
/// Indicates whether to show draft items.
/// This is usually dependent on the current users permissions
/// </param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public record AppDataConfiguration(
    ILookUpEngine LookUpEngine,
    bool? ShowDrafts = null
) : IAppDataConfiguration;