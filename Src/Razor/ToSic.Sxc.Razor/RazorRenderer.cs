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
using ToSic.Sxc.Code.Help;

namespace ToSic.Sxc.Razor
{
    internal class RazorRenderer : ServiceBase, IRazorRenderer
    {
        #region Constructor and DI

        private readonly ITempDataProvider _tempDataProvider;
        private readonly IRazorCompiler _razorCompiler;
        private readonly IMyAppCodeRazorCompiler _myAppCodeRazorCompiler;
        private readonly SourceAnalyzer _sourceAnalyzer;

        public RazorRenderer(ITempDataProvider tempDataProvider, IRazorCompiler razorCompiler, IMyAppCodeRazorCompiler myAppCodeRazorCompiler, SourceAnalyzer sourceAnalyzer) : base($"{Constants.SxcLogName}.RzrRdr")
        {
            ConnectServices(
                _tempDataProvider = tempDataProvider,
                _razorCompiler = razorCompiler,
                _myAppCodeRazorCompiler = myAppCodeRazorCompiler,
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

            var (view, actionContext) = razorType.MyApp 
                ? await _myAppCodeRazorCompiler.CompileView(templatePath, configure, app)
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