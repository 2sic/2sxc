using Custom.Razor.Sys;
using Microsoft.AspNetCore.Mvc.Razor;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeErrorHelp;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Engines.Sys;
using ToSic.Sxc.Render.Sys;
using ToSic.Sxc.Render.Sys.Specs;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Razor;

/// <summary>
/// The razor engine, which compiles / runs engine templates
/// </summary>
[PrivateApi("used to be marked as internal, but it doesn't make sense to show in docs")]
[EngineDefinition(Name = "Razor")]
internal class RazorEngine(
    EngineBase.Dependencies services,
    LazySvc<IRazorRenderer> razorRenderer,
    LazySvc<IExecutionContextFactory> codeRootFactory,
    LazySvc<CodeErrorHelpService> errorHelp,
    LazySvc<IRenderingHelper> renderingHelper)
    : EngineBase(services, connect: [codeRootFactory, errorHelp, renderingHelper, razorRenderer]), IRazorEngine
{
    public override void Init(IBlock block)
    {
        var l = Log.Fn();
        EngineSpecs = Services.EngineSpecsService.GetSpecs(block);
        l.Done();
    }

    /// <inheritdoc/>
    protected override (string? Contents, List<Exception>? Exception) RenderEntryRazor(EngineSpecs engineSpecs, RenderSpecs specs)
    {
        var l = Log.Fn<(string?, List<Exception>?)>();
        var task = RenderTask(engineSpecs, specs);
        try
        {
            task.Wait();
            var result = task.Result;

            if (result.Exception == null)
                return l.ReturnAsOk((result.TextWriter?.ToString(), null));

            var errorMessage = renderingHelper.Value.Init(engineSpecs.Block).DesignErrorMessage([result.Exception], true);
            return l.Return((errorMessage, [result.Exception]));
        }
        catch (Exception ex)
        {
            var myEx = task.Exception?.InnerException ?? ex;
            return l.Return((myEx.ToString(), [myEx]));
        }
    }

    [PrivateApi]
    private async Task<(TextWriter? TextWriter, Exception? Exception)> RenderTask(EngineSpecs engineSpecs, RenderSpecs specs)
    {
        Log.A("will render into TextWriter");
        RazorView? page = null;
        try
        {
            if (string.IsNullOrEmpty(engineSpecs.TemplatePath))
                return (null, null);

            var result = await razorRenderer.Value.RenderToStringAsync(
                engineSpecs,
                specs.Data,
                rzv =>
                {
                    page = rzv; // keep for better errors
                    if (rzv.RazorPage is not IRazor asSxc)
                        return;

                    var dynCode = codeRootFactory.Value
                        .New(asSxc, engineSpecs.Block, Log,
                            compatibilityFallback: CompatibilityLevels.CompatibilityLevel12);

                    asSxc.ConnectToRoot(dynCode);
                    // Note: Don't set the purpose here any more, it's a deprecated feature in 12+
                }
            );
            var writer = new StringWriter();
            await writer.WriteAsync(result);
            return (writer, null);
        }
        catch (Exception maybeIEntityCast)
        {
            return (null, errorHelp.Value.AddHelpIfKnownError(maybeIEntityCast, page));
        }

        // WIP https://github.com/dotnet/aspnetcore/blob/master/src/Mvc/Mvc.Razor.RuntimeCompilation/src/RuntimeViewCompiler.cs#L397-L404
        // maybe also https://stackoverflow.com/questions/48206993/how-to-load-asp-net-core-razor-view-dynamically-at-runtime
        // later also check loading more DLLs on https://stackoverflow.com/questions/58685966/adding-assemblies-types-to-be-made-available-to-razor-page-at-runtime

    }
}