using ToSic.Eav.Metadata.Requirements.Sys;
using ToSic.Sxc.Render.Engines.Sys;
using ToSic.Sxc.Render.Output.Sys;
using ToSic.Sys.Capabilities.SysFeatures;
using ToSic.Sys.Requirements;

namespace ToSic.Sxc.Dnn.Features;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnRequirements(IRequirementsService requirements) : EngineRequirementsBase("Eng.DnnReq", connect: [requirements])
{
    internal bool RequirementsMet() 
        => !RequirementsStatus().SafeAny();

    private ICollection<RequirementStatus> RequirementsStatus()
        => requirements.UnfulfilledRequirements([SysFeatureSuggestions.CSharp08]);

    internal OutputFragmentWithAssets GetMessageForRequirements()
    {
        var l = Log.Fn<OutputFragmentWithAssets>();

        if (RequirementsMet())
            return l.ReturnNull("all seems ok");

        var result = BuildRenderEngineResult(RequirementsStatus());
        return l.Return(result, "error");
    }
}