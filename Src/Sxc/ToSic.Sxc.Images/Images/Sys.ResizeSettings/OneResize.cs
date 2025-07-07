namespace ToSic.Sxc.Images.Sys.ResizeSettings;

/// <summary>
/// The final sizes to be used when resizing
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal record OneResize: ICanDump
{
    public int Width { get; init; }
    public int Height { get; set; }
    public string? Url { get; init; }
    public string? Suffix { get; init; }

    public string UrlWithSuffix() => $"{Url}{Suffix}";

    public Recipe? Recipe { get; init; }

    /// <summary>
    /// Will be set based on image metadata, to determine that the image should be shown completely (like a logo) and not cropped.
    /// This means the image could be a different size than expected
    /// </summary>
    public bool ShowAll { get; init; } = false;

    [PrivateApi]
    public string Dump() => $"{nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(Url)}: {Url}, {nameof(Suffix)}: {Suffix}, " +
                            $"{nameof(ShowAll)}: {ShowAll}, {nameof(Recipe)}: " + "{" + Recipe?.Dump() + "}";
}