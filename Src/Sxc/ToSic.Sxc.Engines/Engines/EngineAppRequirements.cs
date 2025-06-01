using ToSic.Eav.Apps;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Metadata.Requirements.Sys;
using ToSic.Sys.Requirements;

namespace ToSic.Sxc.Engines;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EngineAppRequirements(IRequirementsService requirements) : EngineRequirementsBase("Eng.AppReq", connect: [requirements])
{
    internal bool RequirementsMet(IAppReader appReader) 
        => !RequirementsStatus(appReader).SafeAny();

    private List<RequirementStatus> RequirementsStatus(IAppReader appReader)
        => appReader.GetPiggyBackExpiring("AppRequirementsStatus",
            // take the requirements reported by the app
            () => requirements.UnfulfilledRequirements(appReader.Specs.Metadata)
                // Merge with the basic requirements for 2sxc 17 to work
                //.Concat(requirements.UnfulfilledRequirements(SysFeatureSuggestions.CSharp08.ToListOfOne()).ToList())
                .ToList()
            ).Value;

    internal RenderEngineResult GetMessageForRequirements(IAppReader appReader)
    {
        var l = Log.Fn<RenderEngineResult>();

        // 1. Preflight
        // 1.1. make sure we have an App-State
        if (appReader == null) return l.ReturnNull("no appState");

         if (RequirementsMet(appReader))
            return l.ReturnNull("all seems ok");

        var result = BuildRenderEngineResult(RequirementsStatus(appReader));

        return l.Return(result, "error");
    }
}