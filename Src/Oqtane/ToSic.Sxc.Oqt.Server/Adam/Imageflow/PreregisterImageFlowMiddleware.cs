using Microsoft.AspNetCore.Builder;
using ToSic.Imageflow.Oqt.Server;

namespace ToSic.Sxc.Oqt.Server.Adam.Imageflow;

// IPreregisterImageFlowMiddleware interface belongs to oqtane imageflow module.
// PreregisterImageFlowMiddleware implementation enables dynamic registration of
// ImageflowRewriteMiddleware in oqtane imageflow module to be executed in
// request pipeline exactly before main imageflow middleware because we need to
// rewrite query string params before imageflow middleware take a care of them.
internal class PreregisterImageFlowMiddleware : IPreregisterImageFlowMiddleware
{
    public void Register(IApplicationBuilder app) => app.UseMiddleware<ImageflowRewriteMiddleware>();
}