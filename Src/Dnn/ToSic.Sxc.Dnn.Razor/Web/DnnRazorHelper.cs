using System;
using System.IO;
using System.Web.Hosting;
using System.Web.WebPages;
using Custom.Hybrid;
using ToSic.Eav;
using ToSic.Eav.Code.Help;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.CodeHelpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Wrapper;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Engines.Razor;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Web
{
    [PrivateApi]
    public class DnnRazorHelper: RazorHelperBase
    {
        #region Constructor / Init

        public DnnRazorHelper() : base("Sxc.RzrHlp") { }

        public DnnRazorHelper Init(RazorComponentBase page, Func<string, object[], HelperResult> renderPage)
        {
            Page = page;
            _renderPage = renderPage;
            return this;
        }

        public RazorComponentBase Page { get; private set; }
        private Func<string, object[], HelperResult> _renderPage;

        #endregion

        #region Error Forwarding

        internal void ConfigurePage(WebPageBase parentPage, string virtualPath)
        {
            // Child pages need to get their context from the Parent
            // ...but we're not quite sure why :) - maybe this isn't actually needed
            Page.Context = parentPage.Context;

            // Return if parent page is not a SexyContentWebPage
            if (!(parentPage is RazorComponentBase typedParent)) return;

            ParentPage = typedParent;

            // Only call the Page.ConnectToRoot, as it will call-back this objects ConnectToRoot
            // So don't call: ConnectToRoot(typedParent._DynCodeRoot);
            Page.ConnectToRoot(typedParent._DynCodeRoot);

            Log.A("@RenderPage:" + virtualPath);
        }

        internal RazorComponentBase ParentPage { get; set; }

        #endregion

        #region Html Helper

        internal IHtmlHelper Html => _html ?? (_html = _DynCodeRoot.GetService<HtmlHelper>().Init(Page, this, _DynCodeRoot.Block?.Context.User.IsSystemAdmin ?? false, _renderPage));
        private IHtmlHelper _html;

        #endregion

        #region RenderPage

        /// <summary>
        /// RenderPage is disabled in Razor12+ to force designers to use Html.Partial
        /// </summary>
        internal HelperResult RenderPageNotSupported()
            => throw new NotSupportedException("RenderPage(...) is not supported in Hybrid Razor. Use Html.Partial(...) instead.");


        #endregion

        #region Create Instance

        public object GetCode(string path, string noParamOrder = Protector, string className = default)
        {
            Protect(noParamOrder, nameof(className));
            return CreateInstance(path, name: className);
        }

        public object CreateInstance(string virtualPath,
            string noParamOrder = Protector,
            string name = null,
            bool throwOnError = true)
        {
            var l = Log.Fn<object>($"{virtualPath}, ..., {name}");
            Protect(noParamOrder, $"{nameof(name)}, {nameof(throwOnError)}");

            var path = Page.NormalizePath(virtualPath);
            if (!File.Exists(HostingEnvironment.MapPath(path)))
                throw new FileNotFoundException("The shared file does not exist.", path);

            try
            {
                object result = path.EndsWith(CodeCompiler.CsFileExtension)
                    ? _DynCodeRoot.CreateInstance(path, noParamOrder, name, null, throwOnError)
                    : CreateInstanceCshtml(path);
                return l.Return(result, "ok");
            }
            catch (Exception ex)
            {
                l.Done(ex);
                throw;
            }

        }

        private object CreateInstanceCshtml(string path)
        {
            // ReSharper disable once ConvertTypeCheckToNullCheck
            if (!(Page is IHasDnn))
                throw new ExceptionWithHelp(new CodeHelp(name: "create-instance-cshtml-only-in-old-code",
                    detect: null,
                    uiMessage: "CreateInstance(*.cshtml) is not supported in Hybrid Razor. Use .cs files instead."));
            var pageAsCode = WebPageBase.CreateInstanceFromVirtualPath(path);
            var pageAsRcb = pageAsCode as RazorComponentBase;
            pageAsRcb?.SysHlp.ConfigurePage(Page, pageAsRcb.VirtualPath);
            return pageAsCode;
        }


        #endregion

        #region DynamicModel and Factory

        private DynamicWrapperFactory DynamicWrapperFactory => _dynJacketFactory.Get(() => _DynCodeRoot.GetService<DynamicWrapperFactory>());
        private readonly GetOnce<DynamicWrapperFactory> _dynJacketFactory = new GetOnce<DynamicWrapperFactory>();

        /// <inheritdoc cref="IRazor14{TModel,TServiceKit}.DynamicModel"/>
        public dynamic DynamicModel => _dynamicModel ?? (_dynamicModel = DynamicWrapperFactory.FromDictionary(Page.PageData));
        // new DynamicReadDictionary<object, dynamic>(Page.PageData, _DynCodeRoot.GetService<DynamicJacketFactory>()));
        private dynamic _dynamicModel;

        internal void SetDynamicModel(object data) =>
            _dynamicModel = DynamicWrapperFactory.FromObject(data, WrapperSettings.Dyn(children: false, realObjectsToo: false));
        // new DynamicReadObject(data, false, false, _DynCodeRoot.GetService<DynamicJacketFactory>());

        #endregion
    }
}
