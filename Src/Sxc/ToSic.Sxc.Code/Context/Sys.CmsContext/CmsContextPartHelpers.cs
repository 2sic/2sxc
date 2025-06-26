using ToSic.Eav.Metadata;
using ToSic.Eav.Metadata.Sys;

namespace ToSic.Sxc.Context.Internal;

static class CmsContextPartHelpers
{
    /// <summary>
    /// Enhance the MetadataOf with recommendations,
    /// so that other systems can review this object and determine what additional metadata to suggest adding.
    /// </summary>
    /// <returns></returns>
    [return: NotNullIfNotNull(nameof(md))]
    internal static IMetadata? AddRecommendations(this IMetadata? md, string[]? recommendations = default)
    {
        if (md == null)
            return null;
        md.Target.Recommendations = recommendations ?? [KnownDecorators.NoteDecoratorName];
        return md;
    }

}