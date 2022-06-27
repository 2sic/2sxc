using static ToSic.Sxc.Edit.Toolbar.EntityEditInfo;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleCopy: ToolbarRuleForEntity
    {
        internal const string CommandName = "copy";

        internal ToolbarRuleCopy(
            object target, 
            string ui = null, 
            string parameters = null,
            ToolbarContext context = null,
            ToolbarButtonDecoratorHelper helper = null
        ) : base(CommandName, target, (char)ToolbarRuleOperations.BtnAdd, ui: ui, parameters: parameters, context: context, helper: helper)
        {
            PropSerializeSetAll(false);
            PropSerializeMap[KeyEntityId] = true;
            PropSerializeMap[KeyContentType] = true;
        }

    }
}
