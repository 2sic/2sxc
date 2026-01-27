using System.Configuration;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Dnn.Razor.Sys;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Engines.Sys;
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
internal class DnnRazorEngine(EngineBase.Dependencies helpers, DnnRazorCompiler razorCompiler)
    : EngineBase(helpers, connect: [razorCompiler]),
        IRazorEngine
{
    /// <inheritdoc />
    [PrivateApi]
    public override void Init(IBlock block)
    {
        var l = Log.Fn();
        base.Init(block);
        // after Base.init also init the compiler (requires objects which were set up in base.Init)
        razorCompiler.SetupCompiler(EngineSpecs);
        try
        {
            EntryRazorComponent = InitWebpageAndOldProperties(EngineSpecs.TemplatePath)?.Instance;
        }
        // Catch web.config Error on DNNs upgraded to 7
        catch (ConfigurationErrorsException exc)
        {
            throw l.Done(new Exception("Configuration Error. Your web.config seems to be wrong in the 2sxc folder.", exc));
        }
        l.Done();
    }

    [PrivateApi]
    private RazorComponentBase EntryRazorComponent
    {
        get => Log.Getter(() => field);
        set => Log.Do(cName: $"set{nameof(EntryRazorComponent)}", action: () => field = value);
    }


    [PrivateApi]
    protected override (string, List<Exception>) RenderEntryRazor(EngineSpecs engineSpecs, RenderSpecs specs)
    {
        var (writer, exceptions) = DnnRenderImplementation(EntryRazorComponent, specs);
        return (writer.ToString(), exceptions);
    }

    private (TextWriter writer, List<Exception> exceptions) DnnRenderImplementation(RazorComponentBase webpage, RenderSpecs specs)
    {
        ILogCall<(TextWriter writer, List<Exception> exceptions)> l = Log.Fn<(TextWriter, List<Exception>)>();
        var writer = new StringWriter();
        var result = razorCompiler.Render(webpage, writer, specs);
        return l.ReturnAsOk(result);
    }


    private RazorBuildTempResult<RazorComponentBase> InitWebpageAndOldProperties(string templatePath)
    {
        var l = Log.Fn<RazorBuildTempResult<RazorComponentBase>>();
        var razorBuild = razorCompiler.InitWebpage(templatePath, exitIfNoHotBuild: false);
        var pageToInit = razorBuild.Instance;

        return l.ReturnAsOk(new(pageToInit, razorBuild.UsesHotBuild));

    }
}