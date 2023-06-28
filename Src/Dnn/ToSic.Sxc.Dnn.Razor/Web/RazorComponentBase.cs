using System.Web.WebPages;
using Custom.Hybrid;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Web;
using static ToSic.Eav.Parameters;
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
    public abstract class RazorComponentBase: WebPageBase, ICreateInstance, IHasCodeLog, IHasLog, IRazor, IDnnRazorCompatibility
    {
        /// <summary>
        /// Special helper to move all Razor logic into a separate class.
        /// For architecture of Composition over Inheritance.
        /// </summary>
        [PrivateApi]
        protected internal RazorHelper RazorHelper => _razorHelper ?? (_razorHelper = new RazorHelper().Init(this, (path, data) => base.RenderPage(path, data)));
        private RazorHelper _razorHelper;

        public IHtmlHelper Html => RazorHelper.Html;

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
            RazorHelper.ConfigurePage(parentPage, VirtualPath);
        }

        #region Compile Helpers

        [PrivateApi] string ICreateInstance.CreateInstancePath { get; set; }

        /// <summary>
        /// Creates instances of the shared pages with the given relative path
        /// </summary>
        /// <returns></returns>
        public dynamic CreateInstance(string virtualPath,
            string noParamOrder = Protector,
            string name = null,
            string relativePath = null,
            bool throwOnError = true) =>
            RazorHelper.CreateInstance(virtualPath, noParamOrder, name, throwOnError);

        #endregion

        #region IHasLog

        /// <inheritdoc />
        public ICodeLog Log => RazorHelper.CodeLog; 

        /// <summary>
        /// EXPLICIT Log implementation (to ensure that new IHasLog.Log interface is implemented)
        /// </summary>
        [PrivateApi] ILog IHasLog.Log => RazorHelper.Log;

        #endregion

        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            RazorHelper.ConnectToRoot(codeRoot);
            _DynCodeRoot = codeRoot;
        }
        
    }
}