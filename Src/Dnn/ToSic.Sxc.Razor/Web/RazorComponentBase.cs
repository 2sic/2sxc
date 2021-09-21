using System;
using System.IO;
using System.Web.Hosting;
using System.Web.WebPages;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Engines.Razor;
using File = System.IO.File;

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// The base page type for razor pages
    /// It's the foundation for RazorPage and the old SexyContent page
    /// It only contains internal wiring stuff, so not to be published
    /// </summary>
    [PrivateApi("internal class only!")]
    public abstract partial class RazorComponentBase: WebPageBase, ICreateInstance, IHasLog, ICoupledDynamicCode
    {
        public IHtmlHelper Html { get; internal set; }

        [PrivateApi]
        public IDynamicCodeRoot _DynCodeRoot { get; set; }


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

            // New in v12: the HtmlHelper must know about this page from now on, so we can't re-use the one from the parent
            Html = new HtmlHelper(this);

            // Forward the context
            _DynCodeRoot = typedParent._DynCodeRoot;
            try
            {
                Log.Add("@RenderPage:" + VirtualPath);
            } catch { /* ignore */ }
        }

        #region Compile Helpers

        /// <summary>
        /// Consistency across platforms - because Path exists in .net core, but in .net Framework it has a different name.
        /// </summary>>
        public string Path => VirtualPath;

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
            var wrapLog = Log.Call<dynamic>($"{virtualPath}, ..., {name}");
            var path = NormalizePath(virtualPath);
            VerifyFileExists(path);
            var result = path.EndsWith(CodeCompiler.CsFileExtension)
                ? _DynCodeRoot.CreateInstance(path, noParamOrder, name, null, throwOnError)
                : CreateInstanceCshtml(path);
            return wrapLog("ok", result);
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
        public ILog Log => _log ?? (_log = new Log("Dyn.Temp"));

        private ILog _log;
        #endregion

        public void DynamicCodeCoupling(IDynamicCodeRoot parent)
        {
            if (!(parent is IDynamicCodeRoot isDynCode)) return;
            
            _DynCodeRoot = isDynCode;
            _log = new Log("Rzr.Comp", _DynCodeRoot?.Log);
            var wrapLog = Log.Call();
            wrapLog("ok");
        }
    }


}