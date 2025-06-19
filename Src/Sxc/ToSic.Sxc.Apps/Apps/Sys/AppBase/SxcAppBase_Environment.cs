using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Context;
using ToSic.Sys.Caching.PiggyBack;

namespace ToSic.Sxc.Apps.Sys;

partial class SxcAppBase
{
    /// <inheritdoc />
    [PrivateApi]
    public ISite Site { get; protected set; } = services.Site;


    #region Paths

    [PrivateApi]
    public string PhysicalPath
        => AppReaderInt.GetCache().GetPiggyBack(nameof(PhysicalPath), () => Path.Combine(Site.AppsRootPhysicalFull, Folder));

    #endregion

}