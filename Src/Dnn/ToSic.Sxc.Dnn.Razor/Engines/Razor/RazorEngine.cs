using System;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Compilation;
using System.Web.WebPages;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Razor;
using ToSic.Sxc.Code;
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
    public partial class RazorEngine : EngineBase, IRazorEngine
    {
        //private RazorComponentBase _webpage;
        //private readonly object _initLock = new object();
        //private bool _webpageInitialized = false;

        #region Constructor / DI

        public RazorEngine(EngineBaseDependencies helpers, DnnCodeRootFactory codeRootFactory) : base(helpers) =>
            ConnectServices(
                _codeRootFactory = codeRootFactory
            );
        private readonly DnnCodeRootFactory _codeRootFactory;

        #endregion


        // 2022-03-03 2dm trying to not fix the problem yet, but see more logging
        [PrivateApi]
        protected RazorComponentBase Webpage
        {
            get
            {
                Log.A($"Webpage get: {_webpage}");
                return _webpage;
            }
            set
            {
                Log.A($"Webpage set: {value}");
                _webpage = value;
            }
            
        }
        private RazorComponentBase _webpage;
        //protected RazorComponentBase Webpage
        //{
        //    get
        //    {
        //        if (_webpage != null) return _webpage;
        //        // if Webpage is not initialized, we need to wait on its initialization.
        //        Init();
        //        return _webpage; // it will still return null when TemplatePath is empty
        //    }
        //    set => _webpage = value;
        //}

        /// <inheritdoc />
        [PrivateApi]
        protected override void Init()
        {
            // 2022-03-03 2dm trying to not fix the problem yet, but see more logging
            //if (_webpageInitialized) return;
            try
            {
                // 2022-03-03 2dm trying to not fix the problem yet, but see more logging
                InitWebpage();

                // 2022-03-03 STV code
                //// ensure thread safe one-time initialization with lock (blocking)
                //lock (_initLock)
                //{
                //    if (_webpageInitialized) return;
                //    InitWebpage();
                //    if (!string.IsNullOrEmpty(TemplatePath)) _webpageInitialized = true;
                //}
            }
            // Catch web.config Error on DNNs upgraded to 7
            catch (ConfigurationErrorsException exc)
            {
                var e = new Exception("Configuration Error: Please follow this checklist to solve the problem: http://swisschecklist.com/en/i4k4hhqo/2Sexy-Content-Solve-configuration-error-after-upgrading-to-DotNetNuke-7", exc);
                Log.Ex(e);
                throw e;
            }
        }

        [PrivateApi]
        protected HttpContextBase HttpContext 
            => System.Web.HttpContext.Current == null ? null : new HttpContextWrapper(System.Web.HttpContext.Current);

        [PrivateApi]
        public void Render(TextWriter writer) => Log.Do(message: "will render into TextWriter", action: l =>
        {
            try
            {
                Webpage.ExecutePageHierarchy(new WebPageContext(HttpContext, Webpage, null), writer, Webpage);
            }
            catch (Exception maybeIEntityCast)
            {
                l.Ex(maybeIEntityCast);
                ErrorHelp.AddHelpIfKnownError(maybeIEntityCast);
                throw;
            }
        });

        [PrivateApi]
        protected override string RenderTemplate()
        {
            var wrapLog = Log.Fn<string>();
            var writer = new StringWriter();
            Render(writer);
            return wrapLog.ReturnAsOk(writer.ToString());
        }

        private object CreateWebPageInstance()
        {
            var wrapLog = Log.Fn<object>();
            try
            {
                var compiledType = BuildManager.GetCompiledType(TemplatePath);
                object objectValue = null;
                if (compiledType != null)
                    objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
                return wrapLog.ReturnAsOk(objectValue);
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
                ErrorHelp.AddHelpIfKnownError(ex);
                throw;
            }
        }

        private bool InitWebpage()
        {
            var wrapLog = Log.Fn<bool>();
            if (string.IsNullOrEmpty(TemplatePath)) return wrapLog.ReturnFalse("null path");

            var objectValue = RuntimeHelpers.GetObjectValue(CreateWebPageInstance());
            // ReSharper disable once JoinNullCheckWithUsage
            if (objectValue == null)
                throw new InvalidOperationException($"The webpage found at '{TemplatePath}' was not created.");

            if (!(objectValue is RazorComponentBase pageToInit))
                throw new InvalidOperationException($"The webpage at '{TemplatePath}' must derive from RazorComponentBase.");
            Webpage = pageToInit;

            pageToInit.Context = HttpContext;
            pageToInit.VirtualPath = TemplatePath;
            var compatibility = Constants.CompatibilityLevel9Old;
            if (pageToInit is RazorComponent rzrPage)
            {
#pragma warning disable CS0618
                rzrPage.Purpose = Purpose;
#pragma warning restore CS0618
                compatibility = Constants.CompatibilityLevel10;
            }

            if (pageToInit is IDynamicCode12)
                compatibility = Constants.CompatibilityLevel12;

            if(pageToInit is SexyContentWebPage oldPage)
#pragma warning disable 618, CS0612
                oldPage.InstancePurpose = (InstancePurposes) Purpose;
#pragma warning restore 618, CS0612
            InitHelpers(pageToInit, compatibility);
            return wrapLog.ReturnTrue("ok");
        }

        private void InitHelpers(RazorComponentBase webPage, int compatibility) => Log.Do(() =>
        {
            var dynCode = _codeRootFactory.BuildDynamicCodeRoot(webPage);
            // only do this if not already initialized
            //if (dynCode.Block != null)
            dynCode.InitDynCodeRoot(Block, Log, compatibility);
            webPage.ConnectToRoot(dynCode);

            // New in 10.25 - ensure jquery is not included by default
            if (compatibility > Constants.MaxLevelForAutoJQuery) CompatibilityAutoLoadJQueryAndRvt = false;
        });

    }
}