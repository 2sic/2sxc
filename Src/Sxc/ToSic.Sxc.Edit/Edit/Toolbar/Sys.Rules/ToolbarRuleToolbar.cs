
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Edit.Toolbar.Sys.Rules;

internal class ToolbarRuleToolbar: ToolbarRule
{
    internal const string RuleToolbar = "toolbar";
    internal const string Empty = "empty";
    internal const string Default = "default";

    public ToolbarRuleToolbar(string template = "", string? ui = ""): base(RuleToolbar, ui: ui ?? "")
    {
        CommandValue = template ?? "";
    }

    public bool IsDefault => TemplateName == Default;

    public string TemplateName => CommandValue.HasValue() ? CommandValue : Default;
}