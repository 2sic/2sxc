using System.Linq;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleMetadata: ToolbarRuleNamedBase
    {
        public override string Operator => "+";
        public override string Command => "metadata";

        public override string CommandParams
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_contentType)) return null;
                if (!(_target is IHasMetadata hasMetadata)) return null;

                // 1. check if it's a valid target
                var targetId = hasMetadata.Metadata.MetadataId;

                // Check if it already has this metadata
                var existing = hasMetadata.Metadata.OfType(_contentType).FirstOrDefault();

                // 2. build target string
                var mdFor = "for=" + targetId.TargetType + "," +
                            (targetId.KeyGuid != null ? "guid," + targetId.KeyGuid
                                : targetId.KeyString != null ? "string," + targetId.KeyString
                                : "number," + targetId.KeyNumber);

                // 4. add / update rule
                var newRule = "entityId=" + (existing?.EntityId ?? 0)
                                          + (existing == null
                                              ? "&contentType=" + _contentType + "&" + mdFor
                                              : "");
                return newRule;
            }
        }

        public ToolbarRuleMetadata(object target, string contentType)
        {
            _target = target;
            _contentType = contentType;
        }
        private readonly object _target;
        private readonly string _contentType;


    }
}
