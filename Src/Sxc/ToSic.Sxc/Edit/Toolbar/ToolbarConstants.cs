namespace ToSic.Sxc.Edit.Toolbar;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ToolbarConstants
{
    private const string ForI18N = "i18n:";
    private const string ToolbarPrefixRaw = "Toolbar.";
    public const string ToolbarLabelPrefix = ForI18N + ToolbarPrefixRaw;

    internal const string RuleToolbar = "toolbar";
    internal const string Empty = "empty";
    internal const string Default = "default";

    internal const string RuleShow = "show";
    internal const string RuleColor = "color";
    internal const string RuleTooltip = "title";
    internal const string RuleGroup = "group";
    internal const string RuleClass = "class";
    internal const string RulePosition = "pos";

    internal const string NoteFormatHtml = "html";
    internal const string NoteSettingHtml = "asHtml";

    // Parameter prefixes, so that e.g. form params become "form:key=value"
    public const string RuleParamPrefixPrefill = "prefill:";
    public const string RuleParamPrefixForm = "form:";
    public const string RuleParamPrefixFilter = "filter:";
}