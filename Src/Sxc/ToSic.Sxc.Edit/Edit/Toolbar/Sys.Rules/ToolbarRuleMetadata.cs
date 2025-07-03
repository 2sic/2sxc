using ToSic.Eav.Metadata;
using ToSic.Sxc.Web.Sys.Url;
using static ToSic.Sxc.Edit.Toolbar.Sys.EntityEditInfo;

namespace ToSic.Sxc.Edit.Toolbar.Sys.Rules;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ToolbarRuleMetadata(
    object target,
    string typeName,
    char operation,
    string? ui = null,
    string? parameters = null,
    ToolbarContext? context = null,
    ToolbarButtonDecoratorHelper? decoHelper = null
    ) : ToolbarRuleTargeted(target, CommandName, operation: operation, ui: ui, parameters: parameters, context: context, decoHelper: decoHelper)
{
    internal const string CommandName = "metadata";

    protected override string DecoratorTypeName => typeName;

    public override string GeneratedCommandParams() 
        => UrlParts.ConnectParameters(MetadataCommandParams(), base.GeneratedCommandParams());

    private string MetadataCommandParams()
    {
        if (string.IsNullOrWhiteSpace(typeName)) return "error=NoContentType";
        if (typeName.Contains(",")) return "error=CommaFoundInContentType";
        if (Target is not IHasMetadata hasMetadata) return "error=TargetWithoutMetadata";

        // 1. check if it's a valid target
        var targetId = hasMetadata.Metadata.Target;

        // Check if it already has this metadata
        var existing = hasMetadata.Metadata.OfType(typeName).FirstOrDefault();

        // 2. build target string
        var mdFor = "for=" + targetId.TargetType + "," +
                    (targetId.KeyGuid != null ? $"guid,{targetId.KeyGuid}"
                        : targetId.KeyString != null ? $"string,{targetId.KeyString}"
                        : $"number,{targetId.KeyNumber}");

        // 4. add / update rule
        var newRule = $"{KeyEntityId}={existing?.EntityId ?? 0}"
                      + (existing == null
                          ? $"&{KeyContentType}={typeName}&{mdFor}"
                          : "");
        return newRule;
    }
}