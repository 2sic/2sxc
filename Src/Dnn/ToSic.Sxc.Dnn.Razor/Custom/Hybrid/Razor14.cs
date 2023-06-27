using System.Web.WebPages;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc;
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
    /// Important: The property `Convert` which exited on Razor12 was removed. use `Kit.Convert` instead.
    /// </remarks>
    [PublicApi]
    public abstract partial class Razor14: RazorComponentBase, IRazor14<object, ServiceKit14>
    {
        /// <inheritdoc cref="RazorHelper.RenderPageNotSupported"/>
        [PrivateApi]
        public override HelperResult RenderPage(string path, params object[] data)
            => RazorHelper.RenderPageNotSupported();


        [PrivateApi] public int CompatibilityLevel => Constants.CompatibilityLevel12;


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


        #region CmsContext

        /// <inheritdoc />
        public ICmsContext CmsContext => _DynCodeRoot.CmsContext;

        #endregion


        #region Content, Header, etc. and List

        /// <inheritdoc/>
        public dynamic Content => _DynCodeRoot.Content;

        /// <inheritdoc />
        public dynamic Header => _DynCodeRoot.Header;

        /// <inheritdoc />
        public IContextData Data => _DynCodeRoot.Data;

        #endregion

        #region CreateSource Stuff

        /// <inheritdoc/>
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc/>
        public T CreateSource<T>(IDataStream source) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(source);

        #endregion



        #region Dev Tools & Dev Helpers

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        #endregion
    }


}
