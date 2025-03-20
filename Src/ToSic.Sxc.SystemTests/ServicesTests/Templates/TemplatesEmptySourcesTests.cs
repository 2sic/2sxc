using ToSic.Sxc.Services;

namespace ToSic.Sxc.ServicesTests.Templates;

public class TemplatesEmptySourcesTests(ITemplateService templateSvc, TemplatesTestsBaseHelper helper)// : TemplatesTestsBase
{

    [Fact]
    public void SourcesDefault0()
    {
        var sources = templateSvc.Empty().GetSources();
        Equal(0, sources.Count());
    }

    [Fact]
    public void SourcesDefaultDeep()
    {
        var sources = templateSvc.Empty().GetSources(depth: 10);
        Equal(0, sources.Count());
    }

    [Fact]
    public void SourcesOnEmptyGet1()
    {
        var sources = templateSvc.Empty(sources: [helper.GetDicSource()]).GetSources();
        Equal(1, sources.Count());
    }

    [Fact]
    public void SourcesOnEmptyGet1Deep()
    {
        var sources = templateSvc.Empty(sources: [helper.GetDicSource()]).GetSources(depth: 10);
        Equal(1, sources.Count());
    }

    [Fact]
    public void SourcesOnEmptyGet2()
    {
        var sources = templateSvc.Empty(sources: [helper.GetDicSource(), helper.GetFnSource()]).GetSources();
        Equal(2, sources.Count());
    }

    [Fact]
    public void SourcesOnEmptyGet2Identical1()
    {
        var sources = templateSvc.Empty(sources: [helper.GetDicSource(), helper.GetDicSource()]).GetSources();
        Equal(1, sources.Count());
    }



}