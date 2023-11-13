using ToSic.Eav.Context;

namespace ToSic.Sxc.Context
{
    // ReSharper disable once InconsistentNaming
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class ICmsContextExtensions
    {
        public static string[] SafeLanguagePriorityCodes(this ICmsContext context)
        {
            var site = (context as CmsContext)?.CtxSite.Site;
            return site.SafeLanguagePriorityCodes();
        }
    }
}
