using System.Text.Json;
using ToSic.Eav.Serialization;

namespace ToSic.Sxc.Engines;

internal class EngineMessages
{
    public const string Warning = "warning";

    [PrivateApi]
    internal static string ToolbarForEmptyTemplate = ErrorBoxWithMenu(
        "This is a new module without demo data "
        + "(see <a href='https://go.2sxc.org/no-demo-data' target='_blank'>help</a>). "
        + "Please add content. "
        + EmptyTemplateToolbar
    );

    private const string EmptyTemplateToolbar = "<ul class='sc-menu' data-toolbar='{\"sortOrder\":0,\"useModuleList\":true,\"action\":\"edit,layout\"}'></ul>";

    internal static string BasicToolbar
        = "<ul class='sc-menu' data-toolbar='" +
          JsonSerializer.Serialize(new { sortOrder = 0, useModuleList = true, action = "edit" },
              JsonOptions.SafeJsonForHtmlAttributes) +
          "'></ul>";

    internal static string ErrorBoxWithMenu(string contents) 
        => $"<div class='dnnFormMessage dnnFormInfo alert alert-warning'>{contents}{BasicToolbar}</div>";

    internal static string Box(string contents, string level = "info")
        => $"<div class='alert alert-{level}'>{contents}</div>";
}