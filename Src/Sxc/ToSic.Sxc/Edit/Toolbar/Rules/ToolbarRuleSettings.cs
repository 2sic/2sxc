namespace ToSic.Sxc.Edit.Toolbar;

internal class ToolbarRuleSettings(
#pragma warning disable CS9113 // Parameter is unread.
    NoParamOrder protect = default,
#pragma warning restore CS9113 // Parameter is unread.
    string show = null,
    string hover = null,
    string follow = null,
    string classes = null,
    string autoAddMore = null,
    string ui = "",
    string parameters = "") : ToolbarRule(CommandName, ui: ui, parameters: parameters)
{
    private const string CommandName = "settings";
    private readonly (string, string)[] _uiParams =
        [
            ("show", show),
            ("hover", hover),
            ("follow", follow),
            ("autoAddMore", autoAddMore),
            ("classes", classes),
        ];

    public override string GeneratedUiParams() => BuildValidParameterList(_uiParams);
}