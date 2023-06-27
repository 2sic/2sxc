using System.Web.WebPages;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services;
using ToSic.Sxc.Web;

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    /// <summary>
    /// Base class for v14 Dynamic Razor files.
    /// Will provide the <see cref="ServiceKit14"/> on property `Kit`.
    /// This contains all the popular services used in v14, so that your code can be lighter. 
    /// </summary>
    /// <remarks>
    /// Important: This is very different from Razor12 or Razor14, as it doesn't rely on `dynamic` code any more.
    /// Be aware of this since the APIs are very different.
    /// </remarks>
    [WorkInProgressApi("WIP 16.02 - not final")]
    public abstract partial class Razor16: RazorComponentBase, IRazor, IDynamicCode16
    {

        /// <inheritdoc cref="RazorHelper.RenderPageNotSupported"/>
        [PrivateApi]
        public override HelperResult RenderPage(string path, params object[] data)
            => RazorHelper.RenderPageNotSupported();


        [PrivateApi] public int CompatibilityLevel => Constants.CompatibilityLevel16;

        /// <inheritdoc />
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();


        public ServiceKit14 Kit => _kit.Get(() => _DynCodeRoot.GetKit<ServiceKit14>());
        private readonly GetOnce<ServiceKit14> _kit = new GetOnce<ServiceKit14>();

        ///// <inheritdoc />
        //public string Path => VirtualPath;

        #region Link, Edit

        /// <inheritdoc />
        public ILinkService Link => _DynCodeRoot.Link;

        /// <inheritdoc />
        public IEditService Edit => _DynCodeRoot.Edit;

        #endregion


        #region New App, Settings, Resources

        /// <inheritdoc />
        public new IAppTyped App => (IAppTyped)_DynCodeRoot.App;

        /// <inheritdoc />
        public ITypedStack SettingsStack => _DynCodeRoot.Settings;

        /// <inheritdoc />
        public ITypedStack ResourcesStack => _DynCodeRoot.Resources;

        #endregion


        #region CmsContext

        /// <inheritdoc />
        public ICmsContext CmsContext => _DynCodeRoot.CmsContext;

        #endregion


        #region Dev Tools & Dev Helpers

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        #endregion
    }


}
