using ToSic.Eav.Apps.Decorators;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleImage: ToolbarRuleMetadata
    {
        private const string ImageCommand = "image";

        public ToolbarRuleImage(object target, string ui = null, string parameters = null) : base(target, ImageDecorator.NiceTypeName, ui, parameters)
        {
            Command = ImageCommand;
        }
    }
}
