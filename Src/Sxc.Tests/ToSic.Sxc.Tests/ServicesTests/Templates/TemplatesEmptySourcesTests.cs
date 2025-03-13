namespace ToSic.Sxc.Tests.ServicesTests.Templates;

[TestClass]
public class TemplatesEmptySourcesTests : TemplatesTestsBase
{

    [TestMethod]
    public void SourcesDefault0()
    {
        var sources = GetTemplateServices().Empty().GetSources();
        AreEqual(0, sources.Count());
    }

    [TestMethod]
    public void SourcesDefaultDeep()
    {
        var sources = GetTemplateServices().Empty().GetSources(depth: 10);
        AreEqual(0, sources.Count());
    }

    [TestMethod]
    public void SourcesOnEmptyGet1()
    {
        var sources = GetTemplateServices().Empty(sources: [GetDicSource()]).GetSources();
        AreEqual(1, sources.Count());
    }

    [TestMethod]
    public void SourcesOnEmptyGet1Deep()
    {
        var sources = GetTemplateServices().Empty(sources: [GetDicSource()]).GetSources(depth: 10);
        AreEqual(1, sources.Count());
    }

    [TestMethod]
    public void SourcesOnEmptyGet2()
    {
        var sources = GetTemplateServices().Empty(sources: [GetDicSource(), GetFnSource()]).GetSources();
        AreEqual(2, sources.Count());
    }

    [TestMethod]
    public void SourcesOnEmptyGet2Identical1()
    {
        var sources = GetTemplateServices().Empty(sources: [GetDicSource(), GetDicSource()]).GetSources();
        AreEqual(1, sources.Count());
    }



}