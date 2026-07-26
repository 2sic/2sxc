using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Sys.ContentTypes;
using ToSic.Sxc.Data.Sys;

namespace ToSic.Sxc.ServicesTests.CmsService;

public class MockHtmlContentType
{
    public string? SomeText { get; set; }

    [ContentTypeField(InputTypeWIP = InputTypesWysiwyg.InputTypeWysiwyg)]
    public string? SomeHtml { get; set; }
}