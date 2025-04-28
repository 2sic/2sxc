using ToSic.Sxc.Services;

namespace ToSic.Sxc.ServicesTests.Templates;

// TODO: should change to have more standard DI based sources
// ATM it has 0
// but if the lookups were in DI it would have more, so we could test more


public class TemplatesDefaultSourcesTests(ITemplateService templateSvc, TemplatesTestsBaseHelper helper) //: TemplatesTestsBase
{
    // This is as of v19.03 2025-03
    // Both frameworks have
    // - QueryStringLookUp
    // - LookUpInLookUps
    // NetCore also has
    // - Ticks (DNN provides that so it's not included in .net 472)
    // - DateTime (DNN provides that so it's not included in .net 472)
#if NETCOREAPP
    private const int SourcesAddedByDefault = 4;
#else
    private const int SourcesAddedByDefault = 2;
#endif

    [Fact]
    public void SourcesDefault0()
    {
        var sources = templateSvc.Default().GetSources();
        Equal(SourcesAddedByDefault, sources.Count());
    }

    [Fact]
    public void SourcesDefaultDeep()
    {
        var sources = templateSvc.Default().GetSources(depth: 10);
        Equal(SourcesAddedByDefault, sources.Count());
    }


    [Fact]
    public void SourcesOnEmptyAdd1()
    {
        var sources = templateSvc.Default(sources: [helper.GetDicSource()]).GetSources();
        Equal(1, sources.Count());
    }

    [Fact]
    public void SourcesOnEmptyAdd1GetDeep()
    {
        var sources = templateSvc.Default(sources: [helper.GetDicSource()]).GetSources(depth: 10);
        Equal(SourcesAddedByDefault + 1, sources.Count());
    }

    [Fact]
    public void SourcesOnEmptyAdd2()
    {
        var sources = templateSvc.Default(sources: [helper.GetDicSource(), helper.GetFnSource()]).GetSources();
        Equal(2, sources.Count());
    }

    [Fact]
    public void SourcesOnEmptyGet2Identical1()
    {
        var sources = templateSvc.Default(sources: [helper.GetDicSource(), helper.GetDicSource()]).GetSources();
        Equal(1, sources.Count());
    }



}