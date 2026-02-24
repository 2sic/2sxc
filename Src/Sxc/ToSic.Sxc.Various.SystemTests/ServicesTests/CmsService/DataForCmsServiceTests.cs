using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class DataForCmsServiceTests(DataAssembler dataAssembler)
{
    public const int AppId = -1;
    public const string SomeTextField = "SomeText";
    public const string SomeHtmlField = "SomeHtml";

    internal const string ImgExtension = "png";
    internal const string ImageName = "test.png";
    internal const string AltText = "testing";
    internal const string ImageTag = $"<img src='{ImageName}' data-cmsid='file:{ImageName}' class='wysiwyg-width1of5' alt='{AltText}'>";

    internal const string FolderName = "TestFolder";
    internal const string FolderBase = "/Portals/0/Adam/9876";
    internal const string FolderUrl = FolderBase + "/TestFolder";

    internal static MockSxcFolder GenerateFolderWithTestPng()
    {
        var mockFile = new MockSxcFile
        {
            Extension = ImgExtension,
            FullName = ImageName,
            Url = $"{FolderUrl}/{ImageName}"
        };
        var folder = new MockSxcFolder
        {
            Name = FolderName,
            Files = [mockFile],
            Url = $"{FolderUrl}/",
        };
        return folder;
    }

    public IEntity TstDataEntity(string text = "", string html = "", IContentType? contentType = null)
    {
        var values = new Dictionary<string, object>
        {
            { SomeTextField, text },
            { SomeHtmlField, html }
        };
        return dataAssembler.CreateEntityTac(appId: AppId, entityId: 1, contentType: contentType, values: values, titleField: SomeTextField);
    }


}