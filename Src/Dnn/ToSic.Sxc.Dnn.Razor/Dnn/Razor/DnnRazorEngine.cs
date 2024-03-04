using System.Configuration;
using System.IO;
using ToSic.SexyContent.Engines;
using ToSic.SexyContent.Razor;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Dnn.Razor.Internal;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Dnn.Razor;

/// <summary>
/// The razor engine, which compiles / runs engine templates
/// </summary>
[PrivateApi("used to be InternalApi_DoNotUse_MayChangeWithoutNotice till v16.09")]
[EngineDefinition(Name = "Razor")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
// ReSharper disable once UnusedMember.Global
internal partial class DnnRazorEngine(EngineBase.MyServices helpers, DnnRazorCompiler razorCompiler)
    : EngineBase(helpers, connect: [razorCompiler]),
        IRazorEngine, IEngineDnnOldCompatibility
{
    /// <inheritdoc />
    [PrivateApi]
    public override void Init(IBlock block)
    {
        var l = Log.Fn();
        base.Init(block);
        // after Base.init also init the compiler (requires objects which were set up in base.Init)
        razorCompiler.SetupCompiler(new(App.AppId, Edition, App.Name), Block);
        try
        {
            EntryRazorComponent = InitWebpageAndOldProperties(TemplatePath)?.Instance;
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
    private RazorComponentBase EntryRazorComponent
    {
        get => Log.Getter(() => _entryRazorComponent);
        set => Log.Setter(() => _entryRazorComponent = value);
    }
    private RazorComponentBase _entryRazorComponent;



    [PrivateApi]
    protected override (string, List<Exception>) RenderEntryRazor(RenderSpecs specs)
    {
        var (writer, exceptions) = RenderImplementation(EntryRazorComponent, specs);
        return (writer.ToString(), exceptions);
    }

    private (TextWriter writer, List<Exception> exceptions) RenderImplementation(RazorComponentBase webpage, RenderSpecs specs)
    {
        ILogCall<(TextWriter writer, List<Exception> exceptions)> l = Log.Fn<(TextWriter, List<Exception>)>();
        var writer = new StringWriter();
        var result = razorCompiler.Render(webpage, writer, specs.Data);
        return l.ReturnAsOk(result);
    }


    private RazorBuildTempResult<RazorComponentBase> InitWebpageAndOldProperties(string templatePath)
    {
        var l = Log.Fn<RazorBuildTempResult<RazorComponentBase>>();
        var razorBuild = razorCompiler.InitWebpage(templatePath, exitIfNoHotBuild: false);
        var pageToInit = razorBuild.Instance;

        if (pageToInit is RazorComponent rzrPage)
        {
#pragma warning disable CS0618
            rzrPage.Purpose = Purpose;
#pragma warning restore CS0618
        }

#pragma warning disable 618, CS0612
        if (pageToInit is SexyContentWebPage oldPage)
            oldPage.InstancePurpose = (InstancePurposes)Purpose;
#pragma warning restore 618, CS0612

        return l.ReturnAsOk(new(pageToInit, razorBuild.UsesHotBuild));

    }


    /// <summary>
    /// Special old mechanism to always request jQuery and Rvt
    /// </summary>
    public bool OldAutoLoadJQueryAndRvt => EntryRazorComponent._CodeApiSvc.Cdf.CompatibilityLevel <= CompatibilityLevels.MaxLevelForAutoJQuery;

}