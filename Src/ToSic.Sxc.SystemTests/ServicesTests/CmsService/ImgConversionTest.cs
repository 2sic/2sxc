namespace ToSic.Sxc.ServicesTests.CmsService;

public class ImgConversionTest
{
    public string AName { get; init; } = "todo";

    public string ImgNameAndFolder { get; init; } = "img.png";

    public string ImgClass { get; init; } = "wysiwyg-width1of5";

    public string MimeType { get; init; } = "image/png";

    public string OriginalHtml
    {
        get => field ??= $"<img src='{ImgNameAndFolder}' data-cmsid='file:{ImgNameAndFolder}' class='{ImgClass}'>";
        set => field = value;
    }

    public string Expected
    {
        get => field ??= $"<picture class='{ImgClass}'><source type='{MimeType}' srcset='{ImgNameAndFolder}'><img src='{ImgNameAndFolder}' class='{ImgClass}'></picture>";
        set => field = value;
    }
}