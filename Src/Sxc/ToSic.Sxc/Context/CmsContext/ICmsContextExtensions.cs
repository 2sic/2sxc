using ToSic.Eav.Context;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Context;

// ReSharper disable once InconsistentNaming
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal static class ICmsContextExtensions
{
    internal static string[] SafeLanguagePriorityCodes(this ICmsContext context)
    {
        var site = (context as CmsContext)?.CtxSite.Site;
        return site.SafeLanguagePriorityCodes();
    }
}