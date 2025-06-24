using ToSic.Eav.Context.Sys.ZoneCulture;

namespace ToSic.Sxc.Context.Internal;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CmsCulture(CmsContext parent) : ICmsCulture
{
    public string DefaultCode => parent.CtxSite.Site.DefaultCultureCode;

    public string CurrentCode => parent.CtxSite.Site.SafeCurrentCultureCode();
}