using ToSic.Eav.Apps.State;
using ToSic.Eav.Data.PiggyBack;
using ToSic.Eav.Internal.Requirements;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Engines;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class EngineAppRequirements(IRequirementsService requirements) : EngineRequirementsBase(requirements, "Eng.AppReq")
{
    internal bool RequirementsMet(IAppStateInternal appState) 
        => !RequirementsStatus(appState).SafeAny();

    private List<RequirementStatus> RequirementsStatus(IAppStateInternal appState)
        => appState.GetPiggyBackExpiring("AppRequirementsStatus",
            // take the requirements reported by the app
            () => requirements.UnfulfilledRequirements(appState.Metadata)
                // Merge with the basic requirements for 2sxc 17 to work
                //.Concat(requirements.UnfulfilledRequirements(SysFeatureSuggestions.CSharp08.ToListOfOne()).ToList())
                .ToList()
            ).Value;

    internal RenderEngineResult GetMessageForRequirements(IAppStateInternal appState)
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