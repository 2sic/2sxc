using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Threading.Tasks;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal.CodeErrorHelp;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Razor
{
    internal class RazorRenderer : ServiceBase, IRazorRenderer
    {
        #region Constructor and DI

        private readonly ITempDataProvider _tempDataProvider;
        private readonly IRazorCompiler _razorCompiler;
        private readonly IThisAppCodeRazorCompiler _thisAppCodeRazorCompiler;
        private readonly SourceAnalyzer _sourceAnalyzer;

        public RazorRenderer(ITempDataProvider tempDataProvider, IRazorCompiler razorCompiler, IThisAppCodeRazorCompiler thisAppCodeRazorCompiler, SourceAnalyzer sourceAnalyzer) : base($"{SxcLogging.SxcLogName}.RzrRdr")
        {
            ConnectServices(
                _tempDataProvider = tempDataProvider,
                _razorCompiler = razorCompiler,
                _thisAppCodeRazorCompiler = thisAppCodeRazorCompiler,
                _sourceAnalyzer = sourceAnalyzer
            );
        }
        #endregion

        public async Task<string> RenderToStringAsync<TModel>(string templatePath, TModel model, Action<RazorView> configure, IApp app)
        {
            var l = Log.Fn<string>($"partialName:{templatePath},appId:{app.PhysicalPath}");

            // TODO: SHOULD OPTIMIZE so the file doesn't need to read multiple times
            // 1. probably change so the CodeFileInfo contains the source code
            var razorType = _sourceAnalyzer.TypeOfVirtualPath(templatePath);

            var (view, actionContext) = razorType.IsHotBuildSupported()
                ? await _thisAppCodeRazorCompiler.CompileView(templatePath, configure, app)
                : await _razorCompiler.CompileView(templatePath, configure, null);

            // Prepare to render
            await using var output = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(
                    actionContext.HttpContext,
                    _tempDataProvider),
                output,
                new HtmlHelperOptions()
            );
            await view.RenderAsync(viewContext);
            return l.ReturnAsOk(output.ToString());
        }
    }
}