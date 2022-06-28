using static ToSic.Sxc.Edit.Toolbar.EntityEditInfo;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleCopy: ToolbarRuleForEntity
    {
        internal const string CommandName = "copy";

        internal ToolbarRuleCopy(
            object target = null,
            string contentType = null,
            char operation = '?',
            string ui = null, 
            string parameters = null,
            ToolbarContext context = null,
            ToolbarButtonDecoratorHelper helper = null
        ) : base(CommandName, target, operation, contentType: contentType, ui: ui, parameters: parameters, context: context, helper: helper,
            propsToSerialize: new []{ KeyEntityId, KeyContentType})
        {
        }

    }
}
