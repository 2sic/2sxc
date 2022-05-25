using System.Linq;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleMetadata: ToolbarRuleTargeted
    {
        public ToolbarRuleMetadata(
            object target, 
            string contentTypes, 
            string ui = null, 
            string parameters = null,
            ToolbarContext context = null
        ) : base(target, "metadata", operation: '+', ui: ui, parameters: parameters, context: context)
        {
            _contentTypes = contentTypes;
        }
        private readonly string _contentTypes;

        public override string GeneratedCommandParams() 
            => UrlParts.ConnectParameters(MetadataCommandParams(), base.GeneratedCommandParams());

        private string MetadataCommandParams()
        {
            if (string.IsNullOrWhiteSpace(_contentTypes)) return "error=NoContentType";
            if (_contentTypes.Contains(",")) return "error=CommaFoundInContentType";
            if (!(Target is IHasMetadata hasMetadata)) return "error=TargetWithoutMetadata";

            // 1. check if it's a valid target
            var targetId = hasMetadata.Metadata.Target;

            // Check if it already has this metadata
            var existing = hasMetadata.Metadata.OfType(_contentTypes).FirstOrDefault();

            // 2. build target string
            var mdFor = "for=" + targetId.TargetType + "," +
                        (targetId.KeyGuid != null ? "guid," + targetId.KeyGuid
                            : targetId.KeyString != null ? "string," + targetId.KeyString
                            : "number," + targetId.KeyNumber);

            // 4. add / update rule
            var newRule = "entityId=" + (existing?.EntityId ?? 0)
                                      + (existing == null
                                          ? "&contentType=" + _contentTypes + "&" + mdFor
                                          : "");
            return newRule;
        }
    }
}
