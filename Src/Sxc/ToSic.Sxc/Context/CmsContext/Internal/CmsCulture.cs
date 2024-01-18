using ToSic.Eav.Context;

namespace ToSic.Sxc.Context.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsCulture: ICmsCulture
{
    private readonly CmsContext _parent;

    internal CmsCulture(CmsContext parent)
    {
        _parent = parent;
    }

    public string DefaultCode => _parent.CtxSite.Site.DefaultCultureCode;

    public string CurrentCode => _parent.CtxSite.Site.SafeCurrentCultureCode();
}