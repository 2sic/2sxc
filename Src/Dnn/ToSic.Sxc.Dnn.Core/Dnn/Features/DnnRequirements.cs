using ToSic.Eav.Internal.Features;
using ToSic.Eav.Internal.Requirements;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Dnn.Features;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DnnRequirements(IRequirementsService requirements) : EngineRequirementsBase(requirements, "Eng.DnnReq")
{
    internal bool RequirementsMet() 
        => !RequirementsStatus().SafeAny();

    private List<RequirementStatus> RequirementsStatus()
        => requirements.UnfulfilledRequirements([SysFeatureSuggestions.CSharp08]);

    internal RenderEngineResult GetMessageForRequirements()
    {
        var l = Log.Fn<RenderEngineResult>();

        if (RequirementsMet())
            return l.ReturnNull("all seems ok");

        var result = BuildRenderEngineResult(RequirementsStatus());
        return l.Return(result, "error");
    }
}