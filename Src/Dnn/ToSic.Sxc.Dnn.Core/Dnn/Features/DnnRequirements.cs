using ToSic.Eav.Internal.Features;
using ToSic.Eav.Internal.Requirements;
using ToSic.Sxc.Engines;
using ToSic.Sys.Requirements;

namespace ToSic.Sxc.Dnn.Features;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnRequirements(IRequirementsService requirements) : EngineRequirementsBase("Eng.DnnReq", connect: [requirements])
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