namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleSettings: ToolbarRule
    {

        public ToolbarRuleSettings(
            string noParamOrder = Eav.Parameters.Protector, 
            string show = null, 
            string hover = null, 
            string follow = null, 
            string classes = null,
            string autoAddMore = null,
            string ui = "", 
            string parameters = "") : base("settings", ui: ui, parameters: parameters)
        {
            _show = show;
            _hover = hover;
            _follow = follow;
            _classes = classes;
            _autoAddMore = autoAddMore;
        }
        private readonly string _show;
        private readonly string _hover;
        private readonly string _follow;
        private readonly string _classes;
        private readonly string _autoAddMore;

        public override string GeneratedUiParams()
        {
            return BuildValidParameterList(new[]
            {
                ("show", _show),
                ("hover", _hover),
                ("follow", _follow),
                ("autoAddMore", _autoAddMore),
                ("classes", _classes),
            });
        }
        
    }
}
