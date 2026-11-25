using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ToSic.Sxc.Oqt.Server.Plumbing;

namespace ToSic.Sxc.Oqt.Server.Controllers;

/// <summary>Ensures the request site alias is available before preparing the block.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class OqtInitializeAliasAttribute : ActionFilterAttribute
{
    public OqtInitializeAliasAttribute() => Order = int.MinValue + 1;

    public override void OnActionExecuting(ActionExecutingContext context)
        // SiteState can already carry an alias supplied by a background caller.
        => ((ControllerBase)context.Controller).GetApiService<AliasResolver>().InitIfEmpty();
}
