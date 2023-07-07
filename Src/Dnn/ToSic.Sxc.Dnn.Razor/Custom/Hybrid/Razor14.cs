using System.Collections.Generic;
using System.Web.WebPages;
using ToSic.Eav.Code.Help;
using ToSic.Eav.DataSource;
using ToSic.Eav.LookUp;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.DevTools;
using ToSic.Sxc.Code.Help;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Dnn.Web;
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
    public abstract partial class Razor14: RazorComponentBase, IRazor14<object, ServiceKit14>, IHasCodeHelp, ICreateInstance
    {
        /// <inheritdoc cref="DnnRazorHelper.RenderPageNotSupported"/>
        [PrivateApi]
        public override HelperResult RenderPage(string path, params object[] data)
            => SysHlp.RenderPageNotSupported();


        [PrivateApi] public override int CompatibilityLevel => Constants.CompatibilityLevel12;


        /// <inheritdoc cref="IDynamicCode.GetService{TService}" />
        public TService GetService<TService>() => _DynCodeRoot.GetService<TService>();


        public ServiceKit14 Kit => _kit.Get(() => _DynCodeRoot.GetKit<ServiceKit14>());
        private readonly GetOnce<ServiceKit14> _kit = new GetOnce<ServiceKit14>();


        #region Core Properties which should appear in docs

        /// <inheritdoc />
        public override ICodeLog Log => SysHlp.CodeLog;

        /// <inheritdoc />
        public override IHtmlHelper Html => SysHlp.Html;

        #endregion


        #region Link, Edit

        /// <inheritdoc cref="IDynamicCode.Link" />
        public ILinkService Link => _DynCodeRoot.Link;

        /// <inheritdoc cref="IDynamicCode.Edit" />
        public IEditService Edit => _DynCodeRoot.Edit;

        #endregion


        #region CmsContext

        /// <inheritdoc cref="IDynamicCode.CmsContext" />
        public ICmsContext CmsContext => _DynCodeRoot.CmsContext;

        #endregion


        #region Content, Header, etc. and List

        /// <inheritdoc cref="IDynamicCode.Content" />
        public dynamic Content => _DynCodeRoot.Content;

        /// <inheritdoc cref="IDynamicCode.Header" />
        public dynamic Header => _DynCodeRoot.Header;

        /// <inheritdoc />
        public IContextData Data => _DynCodeRoot.Data;

        #endregion

        #region CreateSource Stuff

        /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataSource, ILookUpEngine)" />
        public T CreateSource<T>(IDataSource inSource = null, ILookUpEngine configurationProvider = default) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(inSource, configurationProvider);

        /// <inheritdoc cref="IDynamicCode.CreateSource{T}(IDataStream)" />
        public T CreateSource<T>(IDataStream source) where T : IDataSource
            => _DynCodeRoot.CreateSource<T>(source);

        #endregion



        #region Dev Tools & Dev Helpers

        [PrivateApi("Not yet ready")]
        public IDevTools DevTools => _DynCodeRoot.DevTools;

        [PrivateApi] List<CodeHelp> IHasCodeHelp.ErrorHelpers => CodeHelpDbV14.Compile14;

        #endregion

        #region CreateInstance

        [PrivateApi] string IGetCodePath.CreateInstancePath { get; set; }

        /// <inheritdoc />
        public virtual dynamic CreateInstance(string virtualPath, string noParamOrder = ToSic.Eav.Parameters.Protector, string name = null, string relativePath = null, bool throwOnError = true)
            => SysHlp.CreateInstance(virtualPath, noParamOrder, name, throwOnError);

        #endregion

    }


}
