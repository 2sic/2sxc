using System.Configuration;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Dnn.Razor.Sys;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Engines.Sys;
using ToSic.Sxc.Render.Sys.Output;
using ToSic.Sxc.Render.Sys.Specs;

namespace ToSic.Sxc.Dnn.Razor;

/// <summary>
/// The razor engine, which compiles / runs engine templates
/// </summary>
/// <remarks>
/// This is the glue-ware to the "Engine". It just ensures API compatibility with the core Engine.
/// Internally it will use the DnnRazorCompiler to compile and run the Razor templates.
///
/// It also manages the EntryRazorComponent, which is the main Razor component for this engine.
/// </remarks>
[PrivateApi("used to be InternalApi_DoNotUse_MayChangeWithoutNotice till v16.09")]
[EngineDefinition(Name = "Razor")]
[ShowApiWhenReleased(ShowApiMode.Never)]
// ReSharper disable once UnusedMember.Global
internal class DnnRazorEngine(
    EngineSpecsService engineSpecsService,
    IBlockResourceExtractor blockResourceExtractor,
    EngineAppRequirements engineAppRequirements,
    DnnRazorCompiler razorCompiler)
    : ServiceBase("Dnn.RzEng", connect: [engineSpecsService, blockResourceExtractor, engineAppRequirements, razorCompiler]),
        IRazorEngine
{
    /// <inheritdoc />
    public RenderEngineResult Render(IBlock block, RenderSpecs specs)
    {
        var l = Log.Fn<RenderEngineResult>(timer: true);

        // Prepare #1: Specs
        var engineSpecs = engineSpecsService.GetSpecs(block);

        // Preflight: check if rendering is possible, or throw exceptions...
        var preFlightResult = engineAppRequirements.CheckExpectedNoRenderConditions(engineSpecs);
        if (preFlightResult != null)
            return l.ReturnAsError(preFlightResult);

        // Prepare #2: after Base.init also init the compiler (requires objects which were set up in base.Init)
        razorCompiler.SetupCompiler(engineSpecs);

        // Prepare #3: The entry component
        RazorComponentBase entryRazorComponent;
        try
        {
            var razorBuild = razorCompiler.InitWebpage(engineSpecs.TemplatePath, exitIfNoHotBuild: false);
            entryRazorComponent = razorBuild.Instance;
        }
        catch (ConfigurationErrorsException exc)    // Catch web.config Error on DNNs upgraded to 7
        {
            throw l.Done(new Exception("Configuration Error. Your web.config seems to be wrong in the 2sxc folder.", exc));
        }

        // Render and process / return
        var renderedTemplate = DnnRenderImplementation(entryRazorComponent, specs);
        var result = blockResourceExtractor.Process(renderedTemplate);
        return l.ReturnAsOk(result);
    }


    private RenderEngineResultRaw DnnRenderImplementation(RazorComponentBase webpage, RenderSpecs specs)
    {
        ILogCall<(TextWriter writer, List<Exception> exceptions)> l = Log.Fn<(TextWriter, List<Exception>)>();
        var (writer, exceptions) = razorCompiler.Render(webpage, new StringWriter(), specs);
        return new ()
        {
            Html = writer.ToString(),
            ExceptionsOrNull = exceptions
        };
    }
}