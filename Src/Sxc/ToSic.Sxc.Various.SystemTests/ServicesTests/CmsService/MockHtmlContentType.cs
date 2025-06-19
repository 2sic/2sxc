using ToSic.Eav.Data.ContentTypes.Sys;
using ToSic.Sxc.Data.Sys;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class MockHtmlContentType
{
    public string? SomeText { get; set; }

    [ContentTypeAttributeSpecs(InputTypeWIP = InputTypes.InputTypeWysiwyg)]
    public string? SomeHtml { get; set; }
}