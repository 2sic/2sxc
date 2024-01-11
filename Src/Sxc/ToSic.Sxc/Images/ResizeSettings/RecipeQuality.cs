namespace ToSic.Sxc.Images;

/// <summary>
/// This is not ready yet.
/// Goal is that different image formats and different sizes can have other quality specs.
/// This is so WebP can use different quality params, or very small JGPs need a higher quality than lager JPGs
/// </summary>
[PublicApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class RecipeQuality(RecipeQuality original, int forWidth, string forFormat, int quality)
{
    public int ForWidth { get; } = forWidth;

    public string ForFormat { get; } = forFormat ?? original.ForFormat;

    public int Quality { get; } = quality;
}