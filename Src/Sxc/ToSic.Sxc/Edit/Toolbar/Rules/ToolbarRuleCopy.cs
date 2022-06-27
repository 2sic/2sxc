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
        ) : base(target, CommandName, (char)ToolbarRuleOperations.Add, ui: ui, parameters: parameters, context: context, helper: helper)
        {
            ParamEntityIdUsed = true;
            ParamContentTypeUsed = true;
        }

    }
}
