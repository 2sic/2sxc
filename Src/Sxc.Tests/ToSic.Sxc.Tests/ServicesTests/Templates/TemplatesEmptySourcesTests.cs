using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.ServicesTests.Templates;

[TestClass]
public class TemplatesEmptySourcesTests : TemplatesTestsBase
{

    [TestMethod]
    public void SourcesDefault0()
    {
        var sources = GetTemplateServices().Empty().GetSources();
        Assert.AreEqual(0, sources.Count());
    }

    [TestMethod]
    public void SourcesDefaultDeep()
    {
        var sources = GetTemplateServices().Empty().GetSources(depth: 10);
        Assert.AreEqual(0, sources.Count());
    }

    [TestMethod]
    public void SourcesOnEmptyGet1()
    {
        var sources = GetTemplateServices().Empty(sources: [GetDicSource()]).GetSources();
        Assert.AreEqual(1, sources.Count());
    }

    [TestMethod]
    public void SourcesOnEmptyGet1Deep()
    {
        var sources = GetTemplateServices().Empty(sources: [GetDicSource()]).GetSources(depth: 10);
        Assert.AreEqual(1, sources.Count());
    }

    [TestMethod]
    public void SourcesOnEmptyGet2()
    {
        var sources = GetTemplateServices().Empty(sources: [GetDicSource(), GetFnSource()]).GetSources();
        Assert.AreEqual(2, sources.Count());
    }

    [TestMethod]
    public void SourcesOnEmptyGet2Identical1()
    {
        var sources = GetTemplateServices().Empty(sources: [GetDicSource(), GetDicSource()]).GetSources();
        Assert.AreEqual(1, sources.Count());
    }



}