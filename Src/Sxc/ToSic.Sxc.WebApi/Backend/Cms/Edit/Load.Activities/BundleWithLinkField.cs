using ToSic.Eav.Ui.Edit.Sys;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

internal class BundleWithLinkField
{
    /// <summary>
    /// Entity Guid
    /// </summary>
    public required Guid Guid;

    /// <summary>
    /// Entity ContentType
    /// </summary>
    public required string ContentTypeName;

    /// <summary>
    /// Hyperlink fields - for further processing.
    /// </summary>
    public required Dictionary<string, Dictionary<string, string>>? HyperlinkFields;

    /// <summary>
    /// String fields, for further processing, since they could contain links.
    /// </summary>
    public required Dictionary<string, Dictionary<string, string>>? StringFields;

    internal static List<BundleWithLinkField> ExtractLinkFields(EditLoadDto editData, bool includeStringFields = false)
    {
        var bundlesHavingLinks = editData.Items
            // Only these with hyperlinks
            .Where(b =>
            {
                var hasLinks = b.Entity?.Attributes?.Hyperlink?.Any() ?? false;
                var hasString = includeStringFields && (b.Entity?.Attributes?.String?.Any() ?? false);
                return hasLinks || hasString;
            })
            .Select(b => new BundleWithLinkField
            {
                Guid = b.Entity!.Guid,
                HyperlinkFields = b.Entity.Attributes.Hyperlink!,
                StringFields = b.Entity.Attributes.String!,
                ContentTypeName = b.Entity.Type.Name,
            });
        return [.. bundlesHavingLinks];
    }
}