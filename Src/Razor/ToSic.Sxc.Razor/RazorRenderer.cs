using Custom.Hybrid;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Threading.Tasks;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Razor;

internal class RazorRenderer : ServiceBase, IRazorRenderer
{
    #region Constructor and DI

    private readonly ITempDataProvider _tempDataProvider;
    private readonly IRazorCompiler _razorCompiler;
    private readonly IAppCodeRazorCompiler _appCodeRazorCompiler;
    private readonly SourceAnalyzer _sourceAnalyzer;

    public RazorRenderer(ITempDataProvider tempDataProvider, IRazorCompiler razorCompiler, IAppCodeRazorCompiler appCodeRazorCompiler, SourceAnalyzer sourceAnalyzer) : base($"{SxcLogging.SxcLogName}.RzrRdr")
    {
        ConnectServices(
            _tempDataProvider = tempDataProvider,
            _razorCompiler = razorCompiler,
            _appCodeRazorCompiler = appCodeRazorCompiler,
            _sourceAnalyzer = sourceAnalyzer
        );
    }
    #endregion

    public async Task<string> RenderToStringAsync<TModel>(string templatePath, TModel model, Action<RazorView> configure, IApp app = null, HotBuildSpec spec = default)
    {
        var l = Log.Fn<string>($"{nameof(templatePath)}: '{templatePath}'; {nameof(app.PhysicalPath)}: '{app?.PhysicalPath}'; {spec}");

        // TODO: SHOULD OPTIMIZE so the file doesn't need to read multiple times
        // 1. probably change so the CodeFileInfo contains the source code
        var razorType = _sourceAnalyzer.TypeOfVirtualPath(templatePath);

        var (view, actionContext) = razorType.IsHotBuildSupported()
            ? await _appCodeRazorCompiler.CompileView(templatePath, configure, app, spec)
            : await _razorCompiler.CompileView(templatePath, configure);

        var viewDataDictionary = CreateViewDataDictionaryForRazorViewWithGenericBaseTypeOrNull(view) ?? 
            new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new()) { Model = model };

        // Prepare to render
        await using var output = new StringWriter();

        var viewContext = new ViewContext(
            actionContext,
            view,
            viewDataDictionary,
            new TempDataDictionary(
                actionContext.HttpContext,
                _tempDataProvider),
            output,
            new HtmlHelperOptions()
        );
        await view.RenderAsync(viewContext);
        return l.ReturnAsOk(output.ToString());
    }
    
    /// <summary>
    /// Creates a ViewDataDictionary object based on the provided IView for RazorView with generic base type
    /// </summary>
    /// <param name="view">The IView object.</param>
    /// <returns>ViewDataDictionary or null</returns>
    private dynamic CreateViewDataDictionaryForRazorViewWithGenericBaseTypeOrNull(IView view)
    {
        var l = Log.Fn<dynamic>($"{nameof(view.Path)}: '{view.Path}'");

        if (view is not RazorView rsv)
            return l.ReturnNull("Not a RazorView");

        var baseType = rsv.RazorPage.GetType().BaseType;
        if (baseType is not { IsGenericType: true } || baseType.GetGenericTypeDefinition() != typeof(RazorTyped<>))
            return l.ReturnNull("Base type is not generic");

        var baseTypeGenericTypeArgument = baseType.GenericTypeArguments[0];
        l.A($"Base type is generic: {baseTypeGenericTypeArgument}");

        // Create an instance of ViewDataDictionary<TModel> 
        dynamic viewDataDictionary = Activator.CreateInstance(
            typeof(ViewDataDictionary<>).MakeGenericType(baseTypeGenericTypeArgument),
            [new EmptyModelMetadataProvider(), new ModelStateDictionary()]
        );
        l.A($"Created ViewDataDictionary<{baseTypeGenericTypeArgument}>");

        // Set the Model property
        viewDataDictionary!.Model = ((dynamic)rsv.RazorPage).Model;
        l.A($"Set Model to {viewDataDictionary.Model}");

        l.Done("ok");
        return viewDataDictionary;
    }
}