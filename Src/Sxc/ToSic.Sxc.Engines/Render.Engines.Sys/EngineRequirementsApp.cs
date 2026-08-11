using ToSic.Eav.Apps;
using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Metadata.Requirements.Sys;
using ToSic.Sxc.Render.Output.Sys;
using ToSic.Sys.Caching.PiggyBack;
using ToSic.Sys.Requirements;

namespace ToSic.Sxc.Render.Engines.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EngineRequirementsApp(IRequirementsService requirementsService)
    : EngineRequirementsBase("Eng.AppReq", connect: [requirementsService])
{
    public OutputFragmentWithAssets? CheckExpectedNoRenderConditions(EngineSpecs engineSpecs)
    {
        var l = Log.Fn<OutputFragmentWithAssets>();

        // Check App Requirements (new 16.08)
        var block = engineSpecs.Block;
        var appReqProblems = GetMessageForRequirements(block.Context.AppReaderRequired);
        if (appReqProblems != null)
            return l.Return(appReqProblems, "error");

        var view = engineSpecs.View;
        if (view.ContentType == "" || view.ContentItem != null || block.Configuration.Content.Any(e => e != null))
            return l.ReturnNull("all ok");

        var result = new OutputFragmentWithAssets
        {
            Html = EngineMessages.ToolbarForEmptyTemplate,
            ActivateJsApi = false,
            Assets = [],
            ErrorCode = null,
            ExceptionsOrNull = null, // should be null, to indicate no exceptions
        };
        return l.Return(result, "error");

    }

    private List<RequirementStatus> RequirementsStatus(IAppReader appReader)
        => appReader.GetCache().PiggyBackGetExpiring(
                "AppRequirementsStatus",
                // take the requirements reported by the app
                () => requirementsService.UnfulfilledRequirements(appReader.Specs.Metadata).ToList()
            )
            .Value;

    internal OutputFragmentWithAssets? GetMessageForRequirements(IAppReader? appReader)
    {
        var l = Log.Fn<OutputFragmentWithAssets>();

        // 1. Preflight
        // 1.1. make sure we have an App-State
        if (appReader == null)
            return l.ReturnNull("no appState");

        if (RequirementsStatus(appReader).SafeNone())
            return l.ReturnNull("all seems ok");

        var result = BuildRenderEngineResult(RequirementsStatus(appReader));

        return l.Return(result, "error");
    }
}