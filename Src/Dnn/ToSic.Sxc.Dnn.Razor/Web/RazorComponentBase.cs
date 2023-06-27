using System;
using System.IO;
using System.Web.Hosting;
using System.Web.WebPages;
using Custom.Hybrid;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Engines.Razor;
using File = System.IO.File;
using IHasLog = ToSic.Lib.Logging.IHasLog;
using ILog = ToSic.Lib.Logging.ILog;

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// The base page type for razor pages
    /// It's the foundation for RazorPage and the old SexyContent page
    /// It only contains internal wiring stuff, so not to be published
    /// </summary>
    [PrivateApi("internal class only!")]
    public abstract partial class RazorComponentBase: WebPageBase, ICreateInstance, IHasCodeLog, IHasLog, IRazor, IDnnRazor
    {
        public IHtmlHelper Html => _html ?? (_html = new HtmlHelper(this, _DynCodeRoot.Block?.Context.User.IsSystemAdmin ?? false));
        private IHtmlHelper _html;

        [PrivateApi]
        public IDynamicCodeRoot _DynCodeRoot { get; private set; }

        /// <inheritdoc />
        public string Path => VirtualPath;


        /// <summary>
        /// Override the base class ConfigurePage, and additionally update internal objects so sub-pages work just like the master
        /// </summary>
        /// <param name="parentPage"></param>
        [PrivateApi]
        protected override void ConfigurePage(WebPageBase parentPage)
        {
            base.ConfigurePage(parentPage);

            // Child pages need to get their context from the Parent
            Context = parentPage.Context;

            // Return if parent page is not a SexyContentWebPage
            if (!(parentPage is RazorComponentBase typedParent)) return;

            _parentComponent = typedParent;

            // Forward the context
            ConnectToRoot(typedParent._DynCodeRoot);
            try
            {
                Log15.A("@RenderPage:" + VirtualPath);
            } catch { /* ignore */ }
        }

        private RazorComponentBase _parentComponent;

        #region Compile Helpers

        public string CreateInstancePath { get; set; }

        /// <summary>
        /// Creates instances of the shared pages with the given relative path
        /// </summary>
        /// <returns></returns>
        public dynamic CreateInstance(string virtualPath,
            string noParamOrder = Eav.Parameters.Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var wrapLog = Log15.Fn<object>($"{virtualPath}, ..., {name}");
            var path = NormalizePath(virtualPath);
            if (!File.Exists(HostingEnvironment.MapPath(path)))
                throw new FileNotFoundException("The shared file does not exist.", path);
            var result = path.EndsWith(CodeCompiler.CsFileExtension)
                ? _DynCodeRoot.CreateInstance(path, noParamOrder, name, null, throwOnError)
                : CreateInstanceCshtml(path);
            return wrapLog.Return((object)result, "ok");
        }

        [PrivateApi]
        // ReSharper disable once InconsistentNaming
        protected string _ErrorWhenUsingCreateInstanceCshtml = null;
        
        private dynamic CreateInstanceCshtml(string path)
        {
            if (_ErrorWhenUsingCreateInstanceCshtml != null)
                throw new NotSupportedException(_ErrorWhenUsingCreateInstanceCshtml);
            var webPage = (RazorComponentBase)CreateInstanceFromVirtualPath(path);
            webPage.ConfigurePage(this);
            return webPage;
        }

        #endregion

        #region IHasLog

        /// <inheritdoc />
        public ICodeLog Log => _codeLog.Get(() => new CodeLog(Log15));
        private readonly GetOnce<ICodeLog> _codeLog = new GetOnce<ICodeLog>();

        [PrivateApi] public ILog Log15 { get; } = new Log("Rzr.Comp");

        /// <summary>
        /// EXPLICIT NEW Log implementation (to ensure that new IHasLog.Log interface is implemented)
        /// </summary>
        [PrivateApi] ILog IHasLog.Log => Log15;

        #endregion

        public void ConnectToRoot(IDynamicCodeRoot codeRoot) => Log15.Do(message: "connected", action: () =>
        {
            _DynCodeRoot = codeRoot;
            this.LinkLog(codeRoot?.Log);
            _codeLog.Reset(); // Reset inner log, so it will reconnect
        });

    }
}