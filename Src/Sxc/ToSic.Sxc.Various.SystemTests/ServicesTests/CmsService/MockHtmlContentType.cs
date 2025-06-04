using ToSic.Eav.Data.ContentTypes.Sys;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class MockHtmlContentType
{
    public string? SomeText { get; set; }

    [ContentTypeAttributeSpecs(InputTypeWIP = ToSic.Sxc.Compatibility.Internal.InputTypes.InputTypeWysiwyg)]
    public string? SomeHtml { get; set; }
}