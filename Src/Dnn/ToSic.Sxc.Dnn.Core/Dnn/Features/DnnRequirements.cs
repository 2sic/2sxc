using ToSic.Sxc.Render.Engines.Sys;
using ToSic.Sxc.Render.Output.Sys;
using ToSic.Sys.Capabilities.SysFeatures;
using ToSic.Sys.Requirements;
using IRequirementsService = ToSic.Eav.Metadata.Requirements.Sys.IRequirementsService;

namespace ToSic.Sxc.Dnn.Features;

/// <summary>
/// This lets DNN razor know if there are any requirements.
/// </summary>
/// <remarks>
/// As of v20 which requires DNN 9, all requirements are actually fulfilled, making this a bit obsolete.
/// But since we expect to need it again when newer C# compilers are introduced, we'll keep this to make it easier to start again.
/// </remarks>
/// <param name="requirements"></param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnRequirements(IRequirementsService requirements) : ServiceBase("Eng.DnnReq", connect: [requirements])
{
    internal bool RequirementsMet() 
        => !RequirementsStatus.SafeAny();

    private ICollection<RequirementStatus> RequirementsStatus => _requirementsStatusCache
        ??= requirements.UnfulfilledRequirements([SysFeatureSuggestions.CSharp08]);

    private static ICollection<RequirementStatus> _requirementsStatusCache;

    internal OutputFragmentWithAssets GetMessageForRequirements()
    {
        var l = Log.Fn<OutputFragmentWithAssets>();

        if (RequirementsMet())
            return l.ReturnNull("all seems ok");

        var result = EngineRequirementsHelpers.BuildRenderEngineResult(RequirementsStatus);
        return l.Return(result, "error");
    }
}