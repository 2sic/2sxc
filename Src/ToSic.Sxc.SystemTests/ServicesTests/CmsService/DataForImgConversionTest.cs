using static ToSic.Sxc.ServicesTests.CmsService.DataForCmsServiceTests;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class DataForImgConversionTest
{
    public static TheoryData<ImgConversionTest> ImageConversions =>
    [
        new()
        {
            AName = "Basic test.png - found in folders",
            ImgName = ImageName,
        },
        new()
        {
            AName = "Basic 123.png (no path) - NOT found in folders",
            ImgName = "123.png",
        },
        new()
        {
            AName = "Basic 123.png (with path) - NOT found in folders",
            ImgName = "123.png",
            Expected = $"<picture class='wysiwyg-width1of5'><source type='image/png' srcset='{FolderUrl}/123.png'><img src='{FolderUrl}/123.png' class='wysiwyg-width1of5'></picture>",
        },
        new()
        {
            AName = "Basic with path",
            ImgName = "img.png",
            ImgNameAndFolder = $"{FolderUrl}/img.png"
        },
        new()
        {
            AName = "jpg with path",
            ImgName = "img.jpg",
            ImgNameAndFolder = $"{FolderUrl}/img.jpg",
            MimeType = "image/jpeg"
        },
        new()
        {
            AName = "Extensive with tst.jpg",
            Original = $"<img src='{FolderUrl}/tst.jpg' data-cmsid='file:tst.jpg' class='img-fluid' loading='lazy' alt='description' width='1230' height='760' style='width:auto;'>",
            Expected = $"<picture class='img-fluid'><source type='image/jpeg' srcset='{FolderUrl}/tst.jpg?w=1230'><img src='{FolderUrl}/tst.jpg?w=1230' loading='lazy' height='760' alt='description' class='img-fluid' style='width:auto;'></picture>",
        }
    ];

}