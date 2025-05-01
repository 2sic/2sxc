using ToSic.Eav.Context;

namespace ToSic.Sxc.Context.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsCulture(CmsContext parent) : ICmsCulture
{
    public string DefaultCode => parent.CtxSite.Site.DefaultCultureCode;

    public string CurrentCode => parent.CtxSite.Site.SafeCurrentCultureCode();
}