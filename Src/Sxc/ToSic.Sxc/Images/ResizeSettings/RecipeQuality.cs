using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// This is not ready yet.
    /// Goal is that different image formats and different sizes can have other quality specs.
    /// This is so WebP can use different quality params, or very small JGPs need a higher quality than lager JPGs
    /// </summary>
    [PublicApi]
    public class RecipeQuality
    {
        public RecipeQuality(RecipeQuality original, int forWidth, string forFormat, int quality)
        {
            ForFormat = forFormat ?? original.ForFormat;
            Quality = quality;
            ForWidth = forWidth;
        }

        public int ForWidth { get; }

        public string ForFormat { get; }

        public int Quality { get; }
    }
}
