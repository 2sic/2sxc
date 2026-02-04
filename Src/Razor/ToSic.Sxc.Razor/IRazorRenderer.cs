using Microsoft.AspNetCore.Mvc.Razor;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Engines.Sys;

namespace ToSic.Sxc.Razor;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IRazorRenderer
{
    Task<string> RenderToStringAsync<TModel>(EngineSpecs engineSpecs, /*string templatePath,*/ TModel model, Action<RazorView> configure/*, IApp app, HotBuildSpec hotBuildSpec*/);
}