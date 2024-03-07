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
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Razor;

internal class RazorRenderer(
    ITempDataProvider tempDataProvider,
    IRazorCompiler appCodeRazorCompiler)
    : ServiceBase($"{SxcLogging.SxcLogName}.RzrRdr",
        connect: [tempDataProvider, appCodeRazorCompiler]), IRazorRenderer
{

    public async Task<string> RenderToStringAsync<TModel>(string templatePath, TModel model, Action<RazorView> configure, IApp app = null, HotBuildSpec hotBuildSpec = default)
    {
        var l = Log.Fn<string>($"{nameof(templatePath)}: '{templatePath}'; {nameof(app.PhysicalPath)}: '{app?.PhysicalPath}'; {hotBuildSpec}");

        var (view, actionContext) = await appCodeRazorCompiler.CompileView(templatePath, configure, app, hotBuildSpec);

        var viewDataDictionary = CreateViewDataDictionaryForRazorViewWithGenericBaseTypeOrNull(view, model) 
            ?? new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new()) { Model = model };

        // Prepare to render
        await using var output = new StringWriter();

        var viewContext = new ViewContext(
            actionContext,
            view,
            viewDataDictionary,
            new TempDataDictionary(
                actionContext.HttpContext,
                tempDataProvider),
            output,
            new HtmlHelperOptions()
        );
        await view.RenderAsync(viewContext);
        return l.ReturnAsOk(output.ToString());
    }

    /// <summary>
    /// Creates a ViewDataDictionary object based on the provided IView for RazorView with generic base type
    /// taking care of setting ViewDataDictionary type is of the same type as the base type and
    /// the Model property if the model is not null is of the same type as the base type.
    /// </summary>
    /// <param name="view">The IView object.</param>
    /// <param name="model"></param>
    /// <returns>ViewDataDictionary or null</returns>
    /// <remarks>
    /// This code is executed for main razor view only.
    /// Main razor page view normally do not have instance of Model data (except in special case when model data is 
    /// eventually provided in uncommon rendering that could happen from our custom razor render code call)
    /// </remarks>
    private dynamic CreateViewDataDictionaryForRazorViewWithGenericBaseTypeOrNull(IView view, object model)
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
        if (model != null && model.GetType() == baseTypeGenericTypeArgument)
        {
            viewDataDictionary!.Model = model; /* ((dynamic)rsv.RazorPage).Model*/;
            l.A($"Set Model to {viewDataDictionary.Model}");
        }


        l.Done("ok");
        return viewDataDictionary;
    }
}