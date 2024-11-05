using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Tests.ServicesTests.Templates;

[TestClass]
public class TemplatesHtmlTests : TemplatesTestsBase
{
    private const string HtmlValue = "<div>message</div>";
    private ILookUp GetHtml() => GetTemplateServices().CreateSource("html", new Dictionary<string, string>
    {
        { "Key1", "Val1" },
        { "Key2", "Val2" },
        { "Html", HtmlValue }
    });


    [DataRow("<div>Val1</div>", "<div>[Html:Key1]</div>")]
    [DataRow("<strong>Val2</strong>", "<strong>[Html:Key2]</strong>")]
    [DataRow("<strong>" + HtmlValue + "</strong>", "<strong>[Html:HTml]</strong>")]
    [TestMethod]
    public void HtmlAllowed(string expected, string value, string notes = default)
    {
        var svc = GetTemplateServices();
        var result = svc.Empty(sources: [GetHtml()]).Parse(value, allowHtml: true);
        Assert.AreEqual(expected, result, $"Value: '{value}', notes: {notes}");
    }

    [DataRow("<div>Val1</div>", "<div>[Html:Key1]</div>")]
    [DataRow("<strong>Val2</strong>", "<strong>[Html:Key2]</strong>")]
    [DataRow("<strong>&lt;div&gt;message&lt;/div&gt;</strong>", "<strong>[Html:Html]</strong>")]
    [TestMethod]
    public void HtmlForbidden(string expected, string value, string notes = default)
    {
        var svc = GetTemplateServices();
        var result = svc.Empty(sources: [GetHtml()]).Parse(value);
        Assert.AreEqual(expected, result, $"Value: '{value}', notes: {notes}");
    }

}