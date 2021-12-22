using System.Linq;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleMetadata: ToolbarRule
    {
        public ToolbarRuleMetadata(object target, string contentTypes) : base("metadata", operation: '+')
        {
            _target = target;
            _contentTypes = contentTypes;
        }
        private readonly object _target;
        private readonly string _contentTypes;

        public override string GeneratedCommandParams()
        {
            if (string.IsNullOrWhiteSpace(_contentTypes)) return "error=NoContentType";
            if (_contentTypes.Contains(",")) return "error=CommaFoundInContentType";
            if (!(_target is IHasMetadata hasMetadata)) return "error=TargetWithoutMetadata";

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
