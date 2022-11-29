using System;
using System.IO;
using System.Web.Hosting;
using System.Web.WebPages;
using Custom.Hybrid;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Logging;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Engines.Razor;
using ToSic.Sxc.Services;
using File = System.IO.File;
using LogAdapter = ToSic.Eav.Logging.LogAdapter;

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
        public IHtmlHelper Html => _html ?? (_html = new HtmlHelper(this, _DynCodeRoot?.Block?.Context.User.IsSystemAdmin ?? false, _DynCodeRoot?.GetService<IFeaturesService>()));
        private IHtmlHelper _html;

        [PrivateApi]
        public IDynamicCodeRoot _DynCodeRoot { get; private set; }


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


            // Forward the context
            ConnectToRoot(typedParent._DynCodeRoot);
            try
            {
                Log15.A("@RenderPage:" + VirtualPath);
            } catch { /* ignore */ }
        }

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
            var wrapLog = Log15.Fn<dynamic>($"{virtualPath}, ..., {name}");
            var path = NormalizePath(virtualPath);
            VerifyFileExists(path);
            var result = path.EndsWith(CodeCompiler.CsFileExtension)
                ? _DynCodeRoot.CreateInstance(path, noParamOrder, name, null, throwOnError)
                : CreateInstanceCshtml(path);
            return wrapLog.Return(result, "ok");
        }

        [PrivateApi]
        // ReSharper disable once InconsistentNaming
        protected string _ErrorWhenUsingCreateInstanceCshtml = null;
        
        protected dynamic CreateInstanceCshtml(string path)
        {
            if (_ErrorWhenUsingCreateInstanceCshtml != null)
                throw new NotSupportedException(_ErrorWhenUsingCreateInstanceCshtml);
            var webPage = (RazorComponentBase)CreateInstanceFromVirtualPath(path);
            webPage.ConfigurePage(this);
            return webPage;
        }

        protected static void VerifyFileExists(string path)
        {
            if (!File.Exists(HostingEnvironment.MapPath(path)))
                throw new FileNotFoundException("The shared file does not exist.", path);
        }


        #endregion

        #region IHasLog

        /// <inheritdoc />
        public ICodeLog Log => _logAdapter.Get(() => new LogAdapter(Log15)/*fallback Log*/); 

        private readonly GetOnce<ICodeLog> _logAdapter = new GetOnce<ICodeLog>();

        [PrivateApi] public ILog Log15 { get; } = new Log("Rzr.Comp");

        /// <summary>
        /// EXPLICIT NEW Log implementation (to ensure that new IHasLog.Log interface is implemented)
        /// </summary>
        [PrivateApi] ILog IHasLog.Log => Log15;

        #endregion

        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            // if (!(parent is IDynamicCodeRoot isDynCode)) return;
            _DynCodeRoot = codeRoot; // isDynCode;

            (Log15 as Log).LinkTo(_DynCodeRoot?.Log, "Rzr.Comp"); // real log
            _logAdapter.IsValueCreated = false;// = new LogAdapter(log); // Eav.Logging.ILog compatibility
            var wrapLog = Log15.Fn();
            wrapLog.Done("ok");
        }

    }
}