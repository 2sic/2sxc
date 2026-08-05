using Microsoft.AspNetCore.Mvc.Razor;
using ToSic.Sxc.Render.Engines.Sys;

namespace ToSic.Sxc.Razor;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IRazorRenderer
{
    Task<string> RenderToStringAsync<TModel>(EngineSpecs engineSpecs, TModel model, Action<RazorView> configure);
}