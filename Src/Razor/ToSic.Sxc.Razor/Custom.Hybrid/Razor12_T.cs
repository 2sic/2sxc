using System;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Plumbing;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    [PrivateApi("This will already be documented through the Dnn DLL so shouldn't appear again in the docs")]
    public abstract partial class Razor12<TModel>: Microsoft.AspNetCore.Mvc.Razor.RazorPage<TModel>
    {
        #region Constructor / DI

        // Source: https://dotnetstories.com/blog/How-to-implement-a-custom-base-class-for-razor-views-in-ASPNET-Core-en-7106773524?o=rss

        /// <summary>
        /// Public MVC Razor-style injected ServiceProvider.
        /// Must be public, otherwise DI doesn't add it to the object. 
        /// Note that this object isn't ready in the constructor, but is later on.
        /// </summary>
        [PrivateApi]
        [RazorInject]
        public IServiceProvider ServiceProvider { get; set; }

        [PrivateApi]
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();

        /// <summary>
        /// Constructor - only available for inheritance
        /// </summary>
        protected Razor12()
        {
            Log = new Log("Mvc.SxcRzr");
        }

        public ILog Log { get; }
        #endregion

    }
}
