using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.ServicesTests.Templates;

[TestClass]
public class TemplatesEmptyTests: TemplatesTestsBase
{

    [TestMethod]
    public void EmptyServiceIsReused()
    {
        var svc = GetTemplateServices();
        var instance1 = svc.Empty();
        var instance2 = svc.Empty();
        Assert.AreEqual(instance1, instance2);
    }


    [DataRow("Hello World")]
    [TestMethod]
    public void EmptyNoData(string expectedAndValue)
    {
        var svc = GetTemplateServices();
        var result = svc.Empty().Parse(expectedAndValue);
        Assert.AreEqual(expectedAndValue, result);
    }

    [DataRow("Hello World", "Hello World")]
    [DataRow("Hello Val1", "Hello [Dic:Key1]")]
    [DataRow("Hello Val2", "Hello [Dic:Key2]")]
    [DataRow("Hello ", "Hello [Dic:unknown]")]
    [DataRow("Hello There", "Hello [Dic:unknown||There]")]
    [DataRow("Hello Val2", "Hello [Dic:unknown||[Dic:Key2]]")]
    [DataRow("Hello ", "Hello [Unknown:Key1]")]
    [TestMethod]
    public void EmptyDictionary(string expected, string value, string notes = default)
    {
        var svc = GetTemplateServices();
        var result = svc.Empty(sources: [GetDicSource("dic")]).Parse(value);
        Assert.AreEqual(expected, result, $"Value: '{value}', notes: {notes}");
    }

    [DataRow("Hello World", "Hello World")]
    [DataRow("Hello Val1", "Hello [Fn:Key1]")]
    [DataRow("Hello Val2", "Hello [Fn:Key2]")]
    [DataRow("Hello WrappedVal1Done", "Hello [Fn:Key1|Wrapped{0}Done]")]
    [TestMethod]
    public void EmptyFunction(string expected, string value, string notes = default)
    {
        var svc = GetTemplateServices();
        var result = svc.Empty(sources: [GetFnSource("fn")]).Parse(value);
        Assert.AreEqual(expected, result, $"Value: '{value}', notes: {notes}");
    }
    [DataRow("Hello World", "Hello World")]
    [DataRow("Hello 42", "Hello [Fn:Key1]")]
    [DataRow("Hello 27", "Hello [Fn:Key2]")]
    [DataRow("Hello 42", "Hello [Fn:Key1|G]")]
    [DataRow("Hello 27", "Hello [Fn:Key2|G]")]
    [DataRow("Hello 42.00", "Hello [Fn:Key1|F]")]
    [DataRow("Hello 27.00", "Hello [Fn:Key2|F]")]
    [DataRow("Hello <42>", "Hello [Fn:Key1|<##>]")]
    [DataRow("Hello 42.00", "Hello [Fn:Key1|00.00]")]
    [TestMethod]
    public void EmptyFunctionFormatter(string expected, string value, string notes = default)
    {
        var svc = GetTemplateServices();
        var result = svc.Empty(sources: [GetFnNumberSourcesWithFormat("fn")]).Parse(value);
        Assert.AreEqual(expected, result, $"Value: '{value}', notes: {notes}");
    }

}