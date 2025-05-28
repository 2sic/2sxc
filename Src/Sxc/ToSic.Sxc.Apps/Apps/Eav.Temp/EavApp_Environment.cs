using ToSic.Eav.Context;
using ToSic.Lib.Caching.PiggyBack;

namespace ToSic.Eav.Apps.Internal;

partial class EavApp
{
    /// <inheritdoc />
    [PrivateApi]
    public ISite Site { get; protected set; } = services.Site;


    #region Paths

    [PrivateApi]
    public string PhysicalPath => AppReaderInt.GetCache().GetPiggyBack(nameof(PhysicalPath), () => Path.Combine(Site.AppsRootPhysicalFull, Folder));

    #endregion

}