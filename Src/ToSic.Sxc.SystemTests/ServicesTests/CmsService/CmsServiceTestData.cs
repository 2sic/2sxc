using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class CmsServiceTestData(DataBuilder dataBuilder)
{
    public const int AppId = -1;
    public const string SomeTextField = "SomeText";
    public const string SomeHtmlField = "SomeHtml";

    internal const string ImageName = "test.png";
    internal const string AltText = "testing";
    internal const string ImageTag = $"<img src='{ImageName}' data-cmsid='file:{ImageName}' class='wysiwyg-width1of5' alt='{AltText}'>";

    internal static MockSxcFolder GenerateFolderWithTestPng()
    {
        var mockFile = new MockSxcFile
        {
            FullName = ImageName,
            Url = "http://test.png/"
        };
        var folder = new MockSxcFolder
        {
            Files = [mockFile]
        };
        return folder;
    }

    public IEntity TstDataEntity(string text = "", string html = "", IContentType? contentType = null)
    {
        var values = new Dictionary<string, object>
        {
            { CmsServiceTestData.SomeTextField, text },
            { CmsServiceTestData.SomeHtmlField, html }
        };
        return dataBuilder.CreateEntityTac(appId: CmsServiceTestData.AppId, entityId: 1, contentType: contentType, values: values, titleField: CmsServiceTestData.SomeTextField);
    }

}