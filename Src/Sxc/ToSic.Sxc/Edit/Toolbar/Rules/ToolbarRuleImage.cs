using ToSic.Eav.Apps.Decorators;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleImage: ToolbarRuleMetadata
    {
        private const string ImageCommand = "image";

        internal ToolbarRuleImage(object target, string ui = null, string parameters = null,
            ToolbarContext context = null,
            ToolbarButtonDecoratorHelper helper = null
        ) : base(target, ImageDecorator.NiceTypeName, (char)ToolbarRuleOps.OprAdd, ui, parameters, context: context, helper: helper)
        {
            Command = ImageCommand;
        }
    }
}
