using System.Text.Json;
using ToSic.Eav.Serialization;

namespace ToSic.Sxc.Engines;

internal class EngineMessages
{
    public const string Warning = "warning";

    [PrivateApi]
    internal static string ToolbarForEmptyTemplate = ErrorBoxWithMenu("No demo item exists for the selected template. ");

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