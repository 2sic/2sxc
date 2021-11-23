using System.Linq;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Services
{
    public class ToolbarRuleService: IToolbarRuleService
    {
        // todo
        // - add parameter protection
        // - detect content-type based on Decorator

        public string Metadata(
            object target,
            string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType)) return null;
            if (!(target is IHasMetadata hasMetadata)) return null;

            // 1. check if it's a valid target
            ITarget targetId = hasMetadata.Metadata.MetadataId;

            // Check if it already has this metadata
            var existing = hasMetadata.Metadata.OfType(contentType).FirstOrDefault();

            // 2. build target string
            var mdFor = "for=" + targetId.TargetType + "," +
                        (targetId.KeyGuid != null ? "guid," + targetId.KeyGuid
                            : targetId.KeyString != null ? "string," + targetId.KeyString
                            : "number," + targetId.KeyNumber);

            // 4. add / update rule
            var newRule = "+metadata?"
                          + "&entityId=" + (existing?.EntityId ?? 0)
                          + (existing == null
                              ? "&contentType=" + contentType + "&" + mdFor
                              : "");

            return newRule;
        }
    }
}
