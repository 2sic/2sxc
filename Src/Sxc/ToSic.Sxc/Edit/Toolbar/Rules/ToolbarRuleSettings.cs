namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ToolbarRuleSettings: ToolbarRule
    {
        private const string CommandName = "settings";

        public ToolbarRuleSettings(
            string noParamOrder = Eav.Parameters.Protector,
            string show = null,
            string hover = null,
            string follow = null,
            string classes = null,
            string autoAddMore = null,
            string ui = "",
            string parameters = "")
            : base(CommandName, ui: ui, parameters: parameters)
            => _uiParams = new[]
            {
                ("show", show),
                ("hover", hover),
                ("follow", follow),
                ("autoAddMore", autoAddMore),
                ("classes", classes),
            };

        private readonly (string, string)[] _uiParams;

        public override string GeneratedUiParams() => BuildValidParameterList(_uiParams);
    }
}
