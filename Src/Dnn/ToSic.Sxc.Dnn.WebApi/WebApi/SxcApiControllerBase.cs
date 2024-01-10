using System.Web.Http.Controllers;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Dnn.WebApi;
using ToSic.Sxc.Dnn.WebApi.Logging;

namespace ToSic.Sxc.WebApi;

/// <summary>
/// This class is the base class of 2sxc API access
/// It will auto-detect the SxcBlock context
/// But it will NOT provide an App or anything like that
/// </summary>
[DnnLogExceptions]
[PrivateApi("This was only ever used as an internal base class, so it can be modified as needed - just make sure the derived types don't break")]
// Can't hide in Intellisense, because that would hide it for all derived classes too
// [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class SxcApiControllerBase(string logSuffix, string insightsGroup = default, string firstMessage = default)
    : DnnApiControllerWithFixes(logSuffix, insightsGroup, firstMessage)
{
    protected override void Initialize(HttpControllerContext controllerContext)
    {
        base.Initialize(controllerContext);
        DynHlp.InitializeBlockContext(controllerContext.Request);
    }

    internal DynamicApiCodeHelpers DynHlp => _dynHlp ??= new DynamicApiCodeHelpers(this, SysHlp);
    private DynamicApiCodeHelpers _dynHlp;
        
}