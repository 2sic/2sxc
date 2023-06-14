using System;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Compilation;
using System.Web.WebPages;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Razor;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Errors;
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
        #region Constructor / DI

        private readonly LazySvc<CodeErrorHelpService> _errorHelp;
        private readonly DnnCodeRootFactory _codeRootFactory;

        public RazorEngine(MyServices helpers, DnnCodeRootFactory codeRootFactory, LazySvc<CodeErrorHelpService> errorHelp) : base(helpers)
        {
            ConnectServices(
                _codeRootFactory = codeRootFactory,
                _errorHelp = errorHelp
            );
        }


        #endregion


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

        /// <inheritdoc />
        [PrivateApi]
        protected override void Init()
        {
            var l = Log.Fn();
            try
            {
                InitWebpage();
            }
            // Catch web.config Error on DNNs upgraded to 7
            catch (ConfigurationErrorsException exc)
            {
                var e = new Exception("Configuration Error. Your web.config seems to be wrong in the 2sxc folder.", exc);
                //old till 2023-05-11 " Please follow this checklist to solve the problem: http://swisschecklist.com/en/i4k4hhqo/2Sexy-Content-Solve-configuration-error-after-upgrading-to-DotNetNuke-7", exc);
                // see https://web.archive.org/web/20131201093234/http://swisschecklist.com/en/i4k4hhqo/2Sexy-Content-Solve-configuration-error-after-upgrading-to-DotNetNuke-7
                throw l.Done(e);
            }
            l.Done();
        }

        [PrivateApi]
        protected HttpContextBase HttpContext 
            => System.Web.HttpContext.Current == null ? null : new HttpContextWrapper(System.Web.HttpContext.Current);

        [PrivateApi]
        public void Render(TextWriter writer, object data)
        {
            var l = Log.Fn(message: "will render into TextWriter");
            try
            {
                if (data != null && Webpage is ISetDynamicModel setDyn)
                    setDyn.SetDynamicModel(data);
            }
            catch (Exception e)
            {
                l.E("Problem with setting dynamic model");
                l.Ex(e);
            }

            try
            {
                var page = Webpage; // access the property once only
                page.ExecutePageHierarchy(new WebPageContext(HttpContext, page, data), writer, page);
            }
            catch (Exception maybeIEntityCast)
            {
                throw l.Ex(_errorHelp.Value.AddHelpIfKnownError(maybeIEntityCast));
            }
            l.Done();
        }

        [PrivateApi]
        protected override string RenderTemplate(object data)
        {
            var l = Log.Fn<string>();
            var writer = new StringWriter();
            Render(writer, data);
            return l.ReturnAsOk(writer.ToString());
        }

        private object CreateWebPageInstance()
        {
            var l = Log.Fn<object>();
            try
            {
                var compiledType = BuildManager.GetCompiledType(TemplatePath);
                object objectValue = null;
                if (compiledType != null)
                    objectValue = RuntimeHelpers.GetObjectValue(Activator.CreateInstance(compiledType));
                return l.ReturnAsOk(objectValue);
            }
            catch (Exception ex)
            {
                throw l.Ex(_errorHelp.Value.AddHelpIfKnownError(ex));
            }
        }

        private bool InitWebpage()
        {
            var l = Log.Fn<bool>();
            if (string.IsNullOrEmpty(TemplatePath)) return l.ReturnFalse("null path");

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

            if (pageToInit is IDynamicCode16)
                compatibility = Constants.CompatibilityLevel16;
            else if (pageToInit is ICompatibleToCode12)
                compatibility = Constants.CompatibilityLevel12;

            if(pageToInit is SexyContentWebPage oldPage)
#pragma warning disable 618, CS0612
                oldPage.InstancePurpose = (InstancePurposes) Purpose;
#pragma warning restore 618, CS0612
            InitHelpers(pageToInit, compatibility);
            return l.ReturnTrue("ok");
        }

        private void InitHelpers(RazorComponentBase webPage, int compatibility)
        {
            var l = Log.Fn($"{nameof(compatibility)}: {compatibility}");
            var dynCode = _codeRootFactory.BuildDynamicCodeRoot(webPage);
            dynCode.InitDynCodeRoot(Block, Log, compatibility);
            webPage.ConnectToRoot(dynCode);

            // New in 10.25 - ensure jquery is not included by default
            if (compatibility > Constants.MaxLevelForAutoJQuery)
                CompatibilityAutoLoadJQueryAndRvt = false;
            l.Done();
        }

    }
}