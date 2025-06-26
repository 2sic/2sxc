using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal static class ToolbarContextExtensions
{
    public static string? ToRuleString(this ToolbarContext? tlbCtx)
        => tlbCtx == null
            ? null
            : tlbCtx.Custom.HasValue()
                ? tlbCtx.Custom
                : UrlParts.ConnectParameters(
                    $"{ToolbarContext.CtxZone}={tlbCtx.ZoneId}",
                    $"{ToolbarContext.CtxApp}={tlbCtx.AppId}"
                );
}