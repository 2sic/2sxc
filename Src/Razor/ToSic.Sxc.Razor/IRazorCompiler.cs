using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Sys.HotBuild;

namespace ToSic.Sxc.Razor;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IRazorCompiler
{
    Task<(IView view, ActionContext context)> CompileView(string partialName, Action<RazorView> configure, IApp app, HotBuildSpec spec);
}