using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Metadata.Requirements.Sys;
using ToSic.Sxc.Engines.Sys;
using ToSic.Sys.Requirements;

namespace ToSic.Sxc.Engines;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EngineAppRequirements(IRequirementsService requirements) : EngineRequirementsBase("Eng.AppReq", connect: [requirements])
{
    public RenderEngineResult? CheckExpectedNoRenderConditions(EngineSpecs engineSpecs)
    {
        var l = Log.Fn<RenderEngineResult>();

        // Check App Requirements (new 16.08)
        var block = engineSpecs.Block;
        var appReqProblems = GetMessageForRequirements(block.Context.AppReaderRequired);
        if (appReqProblems != null)
            return l.Return(appReqProblems, "error");

        var view = engineSpecs.View;
        if (view.ContentType == "" || view.ContentItem != null || block.Configuration.Content.Any(e => e != null))
            return l.ReturnNull("all ok");

        var result = new RenderEngineResult
        {
            Html = EngineMessages.ToolbarForEmptyTemplate,
            ActivateJsApi = false,
            Assets = [],
            ErrorCode = null,
            ExceptionsOrNull = null, // should be null, to indicate no exceptions
        };
        return l.Return(result, "error");

    }

    private bool RequirementsMet(IAppReader appReader) 
        => !RequirementsStatus(appReader).SafeAny();

    private List<RequirementStatus> RequirementsStatus(IAppReader appReader)
        => appReader.GetPiggyBackExpiring("AppRequirementsStatus",
            // take the requirements reported by the app
            () => requirements.UnfulfilledRequirements(appReader.Specs.Metadata)
                // Merge with the basic requirements for 2sxc 17 to work
                //.Concat(requirements.UnfulfilledRequirements(SysFeatureSuggestions.CSharp08.ToListOfOne()).ToList())
                .ToList()
            ).Value;

    internal RenderEngineResult? GetMessageForRequirements(IAppReader? appReader)
    {
        var l = Log.Fn<RenderEngineResult>();

        // 1. Preflight
        // 1.1. make sure we have an App-State
        if (appReader == null)
            return l.ReturnNull("no appState");

        if (RequirementsMet(appReader))
            return l.ReturnNull("all seems ok");

        var result = BuildRenderEngineResult(RequirementsStatus(appReader));

        return l.Return(result, "error");
    }
}