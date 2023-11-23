using System.Web.WebPages;
using Custom.Hybrid;
using ToSic.Eav.Run;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Web;
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
    public abstract class RazorComponentBase: WebPageBase, IRazor, IHasCodeLog, IHasLog, IDnnRazorCompatibility, ICompatibilityLevel
    {
        #region Constructor / Setup

        /// <summary>
        /// Special helper to move all Razor logic into a separate class.
        /// For architecture of Composition over Inheritance.
        /// </summary>
        [PrivateApi]
        internal DnnRazorHelper SysHlp => _sysHlp ??= new DnnRazorHelper().Init(this, (path, data) => base.RenderPage(path, data));
        private DnnRazorHelper _sysHlp;


        /// <inheritdoc />
        [PrivateApi]
        public IDynamicCodeRoot _DynCodeRoot { get; private set; }

        /// <inheritdoc />
        [PrivateApi]
        public void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            SysHlp.ConnectToRoot(codeRoot);
            _DynCodeRoot = codeRoot;
        }

        /// <summary>
        /// Override the base class ConfigurePage, and additionally update internal objects so sub-pages work just like the master
        /// </summary>
        /// <param name="parentPage"></param>
        [PrivateApi]
        protected override void ConfigurePage(WebPageBase parentPage)
        {
            base.ConfigurePage(parentPage);
            SysHlp.ConfigurePage(parentPage, VirtualPath);
        }

        /// <summary>
        /// Must be set on each derived class
        /// </summary>
        [PrivateApi]
        public abstract int CompatibilityLevel { get; }

        #endregion

        #region Secret Stuff like IHasLog or Compile Helpers

        /// <summary>
        /// EXPLICIT Log implementation (to ensure that new IHasLog.Log interface is implemented)
        /// </summary>
        [PrivateApi] ILog IHasLog.Log => SysHlp.Log;

        /// <inheritdoc />
        public string Path => VirtualPath;


        #endregion

        #region Core Properties which should appear in docs

        /// <inheritdoc />
        public virtual ICodeLog Log => SysHlp.CodeLog;

        /// <inheritdoc />
        public virtual IHtmlHelper Html => SysHlp.Html;

        #endregion

    }
}