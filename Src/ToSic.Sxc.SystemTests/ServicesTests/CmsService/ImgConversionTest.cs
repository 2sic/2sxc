namespace ToSic.Sxc.ServicesTests.CmsService;

public class ImgConversionTest
{
    public string AName { get; init; } = "todo";

    public string ImgName { get; init; } = DataForCmsServiceTests.ImageName;

#if NETCOREAPP
    [field: System.Diagnostics.CodeAnalysis.AllowNull, System.Diagnostics.CodeAnalysis.MaybeNull]
#endif
    public string ImgNameAndFolder
    {
        get => field ??= $"{DataForCmsServiceTests.FolderUrl}/{ImgName}";
        init => field = value;
    }

    public string ImgClass { get; init; } = "wysiwyg-width1of5";

    public string MimeType { get; init; } = "image/png";

#if NETCOREAPP
    [field: System.Diagnostics.CodeAnalysis.AllowNull, System.Diagnostics.CodeAnalysis.MaybeNull]
#endif
    public string Original
    {
        get => field ??= $"<img src='{ImgNameAndFolder}' data-cmsid='file:{ImgName}' class='{ImgClass}'>";
        init => field = value;
    }

#if NETCOREAPP
    [field: System.Diagnostics.CodeAnalysis.AllowNull, System.Diagnostics.CodeAnalysis.MaybeNull]
#endif
    public string Expected
    {
        get => field ??= $"<picture class='{ImgClass}'><source type='{MimeType}' srcset='{ImgNameAndFolder}'><img src='{ImgNameAndFolder}' class='{ImgClass}'></picture>";
        init => field = value;
    }


}