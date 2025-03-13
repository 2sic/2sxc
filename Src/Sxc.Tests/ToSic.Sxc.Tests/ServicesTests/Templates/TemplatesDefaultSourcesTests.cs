﻿namespace ToSic.Sxc.Tests.ServicesTests.Templates;

// TODO: should change to have more standard DI based sources
// ATM it has 0
// but if the lookups were in DI it would have more, so we could test more

[TestClass]
public class TemplatesDefaultSourcesTests : TemplatesTestsBase
{

    [TestMethod]
    public void SourcesDefault0()
    {
        var sources = GetTemplateServices().Default().GetSources();
        AreEqual(0, sources.Count());
    }

    [TestMethod]
    public void SourcesDefaultDeep()
    {
        var sources = GetTemplateServices().Default().GetSources(depth: 10);
        AreEqual(0, sources.Count());
    }


    [TestMethod]
    public void SourcesOnEmptyGet1()
    {
        var sources = GetTemplateServices().Default(sources: [GetDicSource()]).GetSources();
        AreEqual(1, sources.Count());
    }

    [TestMethod]
    public void SourcesOnEmptyGet1Deep()
    {
        var sources = GetTemplateServices().Default(sources: [GetDicSource()]).GetSources(depth: 10);
        AreEqual(1, sources.Count());
    }

    [TestMethod]
    public void SourcesOnEmptyGet2()
    {
        var sources = GetTemplateServices().Default(sources: [GetDicSource(), GetFnSource()]).GetSources();
        AreEqual(2, sources.Count());
    }

    [TestMethod]
    public void SourcesOnEmptyGet2Identical1()
    {
        var sources = GetTemplateServices().Default(sources: [GetDicSource(), GetDicSource()]).GetSources();
        AreEqual(1, sources.Count());
    }



}