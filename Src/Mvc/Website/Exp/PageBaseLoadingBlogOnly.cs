using System;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;

namespace ToSic.Sxc.Mvc.RazorPages.Exp
{
    public abstract partial class PageBaseLoadingBlogOnly: Microsoft.AspNetCore.Mvc.RazorPages.Page, IHasLog
    {
        public IServiceProvider ServiceProvider { get; }

        #region Constructor / DI
        protected PageBaseLoadingBlogOnly()
        {
            ServiceProvider = this.HttpContext.RequestServices;
            Log = new Log("Mvc.Page");
        }
        public ILog Log { get; }
        #endregion


        public string Hi() => "hi";

    }
}
