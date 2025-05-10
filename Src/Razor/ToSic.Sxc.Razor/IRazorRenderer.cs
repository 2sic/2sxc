using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Threading.Tasks;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Razor;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IRazorRenderer
{
    Task<string> RenderToStringAsync<TModel>(string templatePath, TModel model, Action<RazorView> configure = null, IApp app = null, HotBuildSpec hotBuildSpec = default);
}