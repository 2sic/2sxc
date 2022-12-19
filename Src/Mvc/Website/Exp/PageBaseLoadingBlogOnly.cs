using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Mvc.RazorPages.Exp
{
    public abstract partial class PageBaseLoadingBlogOnly: Microsoft.AspNetCore.Mvc.RazorPages.Page, IHasLog
    {
        public TService GetService<TService>() => HttpContext.RequestServices.Build<TService>();

        #region Constructor / DI
        protected PageBaseLoadingBlogOnly()
        {
            Log = new Log("Mvc.Page");
        }
        public ILog Log { get; }
        #endregion


        public string Hi() => "hi";

    }
}
