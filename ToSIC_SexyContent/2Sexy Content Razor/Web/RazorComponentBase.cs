using System.IO;
using System.Web.Hosting;
using System.Web.WebPages;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Web;
using File = System.IO.File;

namespace ToSic.Sxc.Web
{
    /// <summary>
    /// The base page type for razor pages
    /// It's the foundation for RazorPage and the old SexyContent page
    /// It only contains internal wiring stuff, so not to be published
    /// </summary>
    [PrivateApi("internal class only!")]
    public abstract class RazorComponentBase: WebPageBase, ISharedCodeBuilder
    {
        public IHtmlHelper Html { get; internal set; }

        // 2019-11-28 2dm: see if we can drop this, I believe it's also attached to the DynCodeHelper
        //[PrivateApi]
        //protected internal Blocks.ICmsBlock Sexy { get; set; }
        [PrivateApi]
        protected internal Dnn.DynamicCode DynCode { get; set; }


        /// <summary>
        /// Override the base class ConfigurePage, and additionally update internal objects so sub-pages work just like the master
        /// </summary>
        /// <param name="parentPage"></param>
        protected override void ConfigurePage(WebPageBase parentPage)
        {
            base.ConfigurePage(parentPage);

            // Child pages need to get their context from the Parent
            Context = parentPage.Context;

            // Return if parent page is not a SexyContentWebPage
            if (!(parentPage is RazorComponentBase typedParent)) return;

            // Forward the context
            Html = typedParent.Html;
            //Sexy = typedParent.Sexy;
            DynCode = typedParent.DynCode;
        }

        #region Compile Helpers

        public string SharedCodeVirtualRoot { get; set; }

        /// <summary>
        /// Creates instances of the shared pages with the given relative path
        /// </summary>
        /// <returns></returns>
        public dynamic CreateInstance(string virtualPath,
            string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter,
            string name = null,
            string relativePath = null,
            bool throwOnError = true)
        {
            var path = NormalizePath(virtualPath);
            VerifyFileExists(path);
            return path.EndsWith(CodeCompiler.CsFileExtension)
                ? DynCode.CreateInstance(path, dontRelyOnParameterOrder, name, null, throwOnError)
                : CreateInstanceCshtml(path);
        }

        protected dynamic CreateInstanceCshtml(string path)
        {
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
    }


}