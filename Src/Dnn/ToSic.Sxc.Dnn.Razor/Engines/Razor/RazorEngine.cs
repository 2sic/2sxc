using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Compilation;
using System.Web.WebPages;
using ToSic.Eav.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Razor;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Help;
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
        private readonly LazySvc<DnnRazorSourceAnalyzer> _sourceAnalyzer;

        public RazorEngine(MyServices helpers, DnnCodeRootFactory codeRootFactory, LazySvc<CodeErrorHelpService> errorHelp, LazySvc<DnnRazorSourceAnalyzer> sourceAnalyzer) : base(helpers)
        {
            ConnectServices(
                _codeRootFactory = codeRootFactory,
                _errorHelp = errorHelp,
                _sourceAnalyzer = sourceAnalyzer
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
        protected HttpContextBase HttpContextCurrent => 
            _httpContext.Get(() => HttpContext.Current == null ? null : new HttpContextWrapper(HttpContext.Current));
        private readonly GetOnce<HttpContextBase> _httpContext = new GetOnce<HttpContextBase>();

        [PrivateApi]
        private (TextWriter writer, List<Exception> exception) Render(TextWriter writer, object data)
        {
            var l = Log.Fn<(TextWriter writer, List<Exception> exception)>(message: "will render into TextWriter");
            var page = Webpage; // access the property once only
            try
            {
                if (data != null && page is ISetDynamicModel setDyn)
                    setDyn.SetDynamicModel(data);
            }
            catch (Exception e)
            {
                l.Ex("Problem with setting dynamic model, error will be ignored.", e);
            }

            try
            {
                page.ExecutePageHierarchy(new WebPageContext(HttpContextCurrent, page, data), writer, page);
            }
            catch (Exception maybeIEntityCast)
            {
                throw l.Ex(_errorHelp.Value.AddHelpIfKnownError(maybeIEntityCast, page));
            }
            return l.Return((writer, page.SysHlp.ExceptionsOrNull));
        }

        [PrivateApi]
        protected override (string, List<Exception>) RenderTemplate(object data)
        {
            var l = Log.Fn<(string, List<Exception>)>();
            var writer = new StringWriter();
            var result = Render(writer, data);
            return l.ReturnAsOk((result.writer.ToString(), result.exception));
        }

        private object CreateWebPageInstance()
        {
            var l = Log.Fn<object>();
            object page = null;
            Type compiledType;
            try
            {
                compiledType = BuildManager.GetCompiledType(TemplatePath);
            }
            catch (Exception ex)
            {
                // TODO: ADD MORE compile error help
                // 1. Read file
                // 2. Try to find base type - or warn if not found
                // 3. ...
                var razorType = _sourceAnalyzer.Value.TypeOfVirtualPath(TemplatePath);
                l.A($"Razor Type: {razorType}");
                throw l.Ex(_errorHelp.Value.AddHelpForCompileProblems(ex, razorType));
            }

            try
            {
                if (compiledType == null)
                    return l.ReturnNull("type not found");

                page = Activator.CreateInstance(compiledType);
                var pageObjectValue = RuntimeHelpers.GetObjectValue(page);
                return l.ReturnAsOk(pageObjectValue);
            }
            catch (Exception ex)
            {
                throw l.Ex(_errorHelp.Value.AddHelpIfKnownError(ex, page));
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

            pageToInit.Context = HttpContextCurrent;
            pageToInit.VirtualPath = TemplatePath;
            //var compatibility = Constants.CompatibilityLevel9Old;
            if (pageToInit is RazorComponent rzrPage)
            {
#pragma warning disable CS0618
                rzrPage.Purpose = Purpose;
#pragma warning restore CS0618
                //compatibility = Constants.CompatibilityLevel10;
            }

            var compatibility = (pageToInit as ICompatibilityLevel)?.CompatibilityLevel ?? Constants.CompatibilityLevel9Old;
            //if (pageToInit is IDynamicCode16)
            //    compatibility = Constants.CompatibilityLevel16;
            //else if (pageToInit is ICompatibleToCode12)
            //    compatibility = Constants.CompatibilityLevel12;

            if (pageToInit is SexyContentWebPage oldPage)
#pragma warning disable 618, CS0612
                oldPage.InstancePurpose = (InstancePurposes)Purpose;
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