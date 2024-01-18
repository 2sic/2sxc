namespace ToSic.Sxc.Images;

/// <summary>
/// The final sizes to be used when resizing
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class OneResize: ICanDump
{
    public int Width;
    public int Height;
    public string Url;
    public string Suffix;

    public string UrlWithSuffix => Url + Suffix;

    public Recipe Recipe;

    /// <summary>
    /// Will be set based on image metadata, to determine that the image should be shown completely (like a logo) and not cropped.
    /// This means the image could be a different size than expected
    /// </summary>
    public bool ShowAll { get; set; } = false;

    [PrivateApi]
    public string Dump() => $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(Url)}: {Url}, {nameof(Suffix)}: {Suffix}, " +
                            $"{nameof(ShowAll)}: {ShowAll}, {nameof(Recipe)}: " + "{" + Recipe?.Dump() + "}";
}