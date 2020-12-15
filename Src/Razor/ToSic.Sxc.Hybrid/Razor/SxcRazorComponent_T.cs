using System;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Hybrid.Razor
{
    // test, doesn't do anything yet
    public abstract partial class RazorComponent<TModel>: Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        #region Constructor / DI

        // Source: https://dotnetstories.com/blog/How-to-implement-a-custom-base-class-for-razor-views-in-ASPNET-Core-en-7106773524?o=rss

        /// <summary>
        /// Experimental. Note that this object isn't ready in the constructor, but is later on
        /// </summary>
        [PrivateApi]
        [RazorInject]
        public IServiceProvider ServiceProvider { get; set; }

        [PrivateApi]
        public TService GetService<TService>() => ServiceProvider.Build<TService>();


        protected RazorComponent()
        {
            Log = new Log("Mvc.SxcRzr");
        }

        public ILog Log { get; }
        #endregion

        public Purpose Purpose { get; set; }

    }
}
