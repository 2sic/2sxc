using ToSic.Eav.Apps;
using ToSic.Eav.Apps.State;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Internal.Requirements;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Engines;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class EngineAppRequirements(IRequirementsService requirements) : EngineRequirementsBase(requirements, "Eng.AppReq")
{
    internal bool RequirementsMet(IAppReader appState) 
        => !RequirementsStatus(appState).SafeAny();

    private List<RequirementStatus> RequirementsStatus(IAppReader appReader)
        => appReader.GetPiggyBackExpiring("AppRequirementsStatus",
            // take the requirements reported by the app
            () => requirements.UnfulfilledRequirements(appReader.Specs.Metadata)
                // Merge with the basic requirements for 2sxc 17 to work
                //.Concat(requirements.UnfulfilledRequirements(SysFeatureSuggestions.CSharp08.ToListOfOne()).ToList())
                .ToList()
            ).Value;

    internal RenderEngineResult GetMessageForRequirements(IAppReader appState)
    {
        var l = Log.Fn<RenderEngineResult>();

        // 1. Preflight
        // 1.1. make sure we have an App-State
        if (appState == null) return l.ReturnNull("no appState");

         if (RequirementsMet(appState))
            return l.ReturnNull("all seems ok");

        var result = BuildRenderEngineResult(RequirementsStatus(appState));

        return l.Return(result, "error");
    }
}