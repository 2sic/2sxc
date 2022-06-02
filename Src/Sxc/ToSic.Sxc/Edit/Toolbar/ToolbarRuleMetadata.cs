using System.Linq;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleMetadata: ToolbarRuleTargeted
    {
        internal const string CommandName = "metadata";

        internal ToolbarRuleMetadata(
            object target, 
            string typeName, 
            string ui = null, 
            string parameters = null,
            ToolbarContext context = null,
            ToolbarButtonDecoratorHelper helper = null
        ) : base(target, CommandName, operation: '+', ui: ui, parameters: parameters, context: context)
        {
            _typeName = typeName;
            _helper = helper;
        }
        private readonly string _typeName;
        private readonly ToolbarButtonDecoratorHelper _helper;

        protected ToolbarButtonDecorator Decorator => _decorator.Get(() => _helper?.GetDecorator(Context, _typeName, CommandName));
        private readonly ValueGetOnce<ToolbarButtonDecorator> _decorator = new ValueGetOnce<ToolbarButtonDecorator>();

        public override string GeneratedCommandParams() 
            => UrlParts.ConnectParameters(MetadataCommandParams(), base.GeneratedCommandParams());

        public override string GeneratedUiParams() 
            => UrlParts.ConnectParameters(MetadataUiParams(), base.GeneratedUiParams());

        private string MetadataUiParams() => Decorator?.AllRules();

        private string MetadataCommandParams()
        {
            if (string.IsNullOrWhiteSpace(_typeName)) return "error=NoContentType";
            if (_typeName.Contains(",")) return "error=CommaFoundInContentType";
            if (!(Target is IHasMetadata hasMetadata)) return "error=TargetWithoutMetadata";

            // 1. check if it's a valid target
            var targetId = hasMetadata.Metadata.Target;

            // Check if it already has this metadata
            var existing = hasMetadata.Metadata.OfType(_typeName).FirstOrDefault();

            // 2. build target string
            var mdFor = "for=" + targetId.TargetType + "," +
                        (targetId.KeyGuid != null ? "guid," + targetId.KeyGuid
                            : targetId.KeyString != null ? "string," + targetId.KeyString
                            : "number," + targetId.KeyNumber);

            // 4. add / update rule
            var newRule = "entityId=" + (existing?.EntityId ?? 0)
                                      + (existing == null
                                          ? "&contentType=" + _typeName + "&" + mdFor
                                          : "");
            return newRule;
        }
    }
}
