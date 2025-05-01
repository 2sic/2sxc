using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Context.Internal;

static class CmsContextPartHelpers
{
    /// <summary>
    /// Enhance the MetadataOf with recommendations,
    /// so that other systems can review this object and determine what additional metadata to suggest adding.
    /// </summary>
    /// <returns></returns>
    internal static IMetadataOf AddRecommendations(this IMetadataOf md, string[] recommendations = default)
    {
        if (md == null)
            return null;
        md.Target.Recommendations = recommendations ?? [Decorators.NoteDecoratorName];
        return md;
    }

}