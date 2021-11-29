using System.Linq;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Services
{
    public class ToolbarRuleService: IToolbarRuleService
    {
        // todo
        // - add parameter protection
        // - detect content-type based on Decorator

        public ToolbarRuleBase Metadata(object target, string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType)) return new ToolbarRuleGeneric(null);
            if (!(target is IHasMetadata hasMetadata)) return new ToolbarRuleGeneric(null);

            // 1. check if it's a valid target
            var targetId = hasMetadata.Metadata.MetadataId;

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

            return new ToolbarRuleGeneric(newRule);
        }

        public ToolbarFluidWIP New() => new ToolbarFluidWIP();
    }
}
