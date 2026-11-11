using ToSic.Eav.Context;
using ToSic.Sxc.Dnn.Context;

namespace ToSic.Sxc.Dnn;

public class DnnSiteShareWip(LazySvc<DnnSiteFactory> siteFactory)
{
    #region Separating factory from shared global state

    /// <summary>
    /// The shared state, not ideal, but working for now.
    /// In the future, we must find a better way to clearly broadcast the shared site.
    /// </summary>
    public ISite TempSite => TempSiteContents;

    internal DnnSite TempSiteContents
    {
        get => field ??= siteFactory.Value.CreateFromPortal(null);
        set;
    }

    #endregion

}
