using System;
using System.IO;
using System.Web.Hosting;
using System.Web.WebPages;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Errors;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Engines.Razor;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Web
{
    [PrivateApi]
    public class RazorHelper: ServiceForDynamicCode
    {
        #region Constructor / Init

        public RazorHelper(RazorComponentBase page, Func<string, object[], HelperResult> renderPage): base("Sxc.RzrHlp")
        {
            Page = page;
            _renderPage = renderPage;
        }

        public RazorComponentBase Page { get; }
        private readonly Func<string, object[], HelperResult> _renderPage;

        public override void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            // Do base work
            base.ConnectToRoot(codeRoot);
            var l = Log.Fn(message: "connected");
            // Make sure the Code-Log is reset, in case it was used before this call
            _codeLog.Reset();
            l.Done();
        }

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

        internal Exception RenderException
        {
            get => _renderException;
            set
            {
                _renderException = value;
                if (ParentPage != null) ParentPage.RazorHelper.RenderException = value;
            }
        }

        private Exception _renderException;

        #endregion

        #region CodeLog / Html Helper

        public ICodeLog CodeLog => _codeLog.Get(() => new CodeLog(Log));
        private readonly GetOnce<ICodeLog> _codeLog = new GetOnce<ICodeLog>();


        public IHtmlHelper Html => _html ?? (_html = new HtmlHelper(Page, _DynCodeRoot.Block?.Context.User.IsSystemAdmin ?? false, _renderPage));
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

        public object CreateInstance(string virtualPath,
            string noParamOrder = Eav.Parameters.Protector,
            string name = null,
            bool throwOnError = true)
        {
            var wrapLog = Log.Fn<object>($"{virtualPath}, ..., {name}");
            var path = Page.NormalizePath(virtualPath);
            if (!File.Exists(HostingEnvironment.MapPath(path)))
                throw new FileNotFoundException("The shared file does not exist.", path);
            object result = path.EndsWith(CodeCompiler.CsFileExtension)
                ? _DynCodeRoot.CreateInstance(path, noParamOrder, name, null, throwOnError)
                : CreateInstanceCshtml(path);
            return wrapLog.Return(result, "ok");
        }

        internal object CreateInstanceCshtml(string path)
        {
            // ReSharper disable once ConvertTypeCheckToNullCheck
            if (!(Page is IHasDnn))
                throw new ExceptionWithHelp(new CodeHelp(name: "create-instance-cshtml-only-in-old-code",
                    detect: null,
                    uiMessage: "CreateInstance(*.cshtml) is not supported in Hybrid Razor. Use .cs files instead."));
            var pageAsCode = WebPageBase.CreateInstanceFromVirtualPath(path);
            var pageAsRcb = pageAsCode as RazorComponentBase;
            pageAsRcb?.RazorHelper.ConfigurePage(Page, pageAsRcb.VirtualPath);
            return pageAsCode;
        }



        #endregion
    }
}
