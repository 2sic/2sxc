using System;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Compilation;
using System.Web.WebPages;
using ToSic.Eav.Documentation;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Razor;
using ToSic.Sxc.Dnn;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Engines
{
    /// <summary>
    /// The razor engine, which compiles / runs engine templates
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    [EngineDefinition(Name = "Razor")]
    // ReSharper disable once UnusedMember.Global
    public partial class RazorEngine : EngineBase
    {
        private readonly Lazy<DnnDynamicCodeRoot> _dnnDynCodeLazy;

        #region Constructor / DI

        public RazorEngine(EngineBaseDependencies helpers, Lazy<DnnDynamicCodeRoot> dnnDynCodeLazy) : base(helpers)
        {
            _dnnDynCodeLazy = dnnDynCodeLazy;
        }

        #endregion


        [PrivateApi]
        protected RazorComponentBase  Webpage { get; set; }

        /// <inheritdoc />
        [PrivateApi]
        protected override void Init()
        {
            try
            {
                InitWebpage();
            }
            // Catch web.config Error on DNNs upgraded to 7
            catch (ConfigurationErrorsException exc)
            {
                var e = new Exception("Configuration Error: Please follow this checklist to solve the problem: http://swisschecklist.com/en/i4k4hhqo/2Sexy-Content-Solve-configuration-error-after-upgrading-to-DotNetNuke-7", exc);
                throw e;
            }
        }

        [PrivateApi]
        protected HttpContextBase HttpContext 
            => System.Web.HttpContext.Current == null ? null : new HttpContextWrapper(System.Web.HttpContext.Current);

        [PrivateApi]
        public void Render(TextWriter writer)
        {
            Log.Add("will render into TextWriter");
            try
            {
                Webpage.ExecutePageHierarchy(new WebPageContext(HttpContext, Webpage, null), writer, Webpage);
            }
            catch (Exception maybeIEntityCast)
            {
                Code.ErrorHelp.AddHelpIfKnownError(maybeIEntityCast);
                throw;
            }
        }

        /// <inheritdoc/>
        protected override string RenderTemplate()
        {
            Log.Add("will render razor template");
            var writer = new StringWriter();
            Render(writer);
            return writer.ToString();
        }

        private object CreateWebPageInstance()
        {
            try
            {
                var compiledType = BuildManager.GetCompiledType(TemplatePath);
                object objectValue = null;
                if (compiledType != null)
                    objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
                return objectValue;
            }
            catch (Exception ex)
            {
                Code.ErrorHelp.AddHelpIfKnownError(ex);
                throw;
            }
        }

        private void InitWebpage()
        {
            if (string.IsNullOrEmpty(TemplatePath)) return;

            var objectValue = RuntimeHelpers.GetObjectValue(CreateWebPageInstance());
            // ReSharper disable once JoinNullCheckWithUsage
            if (objectValue == null)
                throw new InvalidOperationException($"The webpage found at '{TemplatePath}' was not created.");

            Webpage = objectValue as RazorComponentBase;

            if (Webpage == null)
                throw new InvalidOperationException($"The webpage at '{TemplatePath}' must derive from RazorComponentBase.");

            Webpage.Context = HttpContext;
            Webpage.VirtualPath = TemplatePath;
            var compatibility = 9;
            if (Webpage is RazorComponent rzrPage)
            {
                rzrPage.Purpose = Purpose;
                compatibility = 10;
            }
#pragma warning disable 618
            if(Webpage is SexyContentWebPage oldPage)
                oldPage.InstancePurpose = (InstancePurposes) Purpose;
#pragma warning restore 618
            InitHelpers(Webpage, compatibility);
        }

        private void InitHelpers(RazorComponentBase webPage, int compatibility)
        {
            webPage.Html = new Razor.HtmlHelper(webPage);
            webPage._DynCodeRoot = _dnnDynCodeLazy.Value.Init(Block, Log, compatibility);

            #region New in 10.25 - ensure jquery is not included by default

            if (compatibility == 10) CompatibilityAutoLoadJQueryAndRVT = false;

            #endregion

        }


    }
}