using ToSic.Eav.LookUp;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.ServicesTests.Templates;

public class TemplatesHtmlTests(ITemplateService templateSvc, TemplatesTestsBaseHelper helper) // : TemplatesTestsBase
{
    private const string HtmlValue = "<div>message</div>";
    private ILookUp GetHtml() => templateSvc.CreateSource("html", new Dictionary<string, string>
    {
        { "Key1", "Val1" },
        { "Key2", "Val2" },
        { "Html", HtmlValue }
    });


    [Theory]
    [InlineData("<div>Val1</div>", "<div>[Html:Key1]</div>")]
    [InlineData("<strong>Val2</strong>", "<strong>[Html:Key2]</strong>")]
    [InlineData("<strong>" + HtmlValue + "</strong>", "<strong>[Html:HTml]</strong>")]
    public void HtmlAllowed(string expected, string value, string notes = default)
    {
        var svc = templateSvc;
        var result = svc.Empty(sources: [GetHtml()]).Parse(value, allowHtml: true);
        Equal(expected, result);//, $"Value: '{value}', notes: {notes}");
    }

    [Theory]
    [InlineData("<div>Val1</div>", "<div>[Html:Key1]</div>")]
    [InlineData("<strong>Val2</strong>", "<strong>[Html:Key2]</strong>")]
    [InlineData("<strong>&lt;div&gt;message&lt;/div&gt;</strong>", "<strong>[Html:Html]</strong>")]
    public void HtmlForbidden(string expected, string value, string notes = default)
    {
        var svc = templateSvc;
        var result = svc.Empty(sources: [GetHtml()]).Parse(value);
        Equal(expected, result);//, $"Value: '{value}', notes: {notes}");
    }

}