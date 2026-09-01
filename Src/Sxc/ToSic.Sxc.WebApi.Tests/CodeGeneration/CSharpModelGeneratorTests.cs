using ToSic.Eav.Apps.Sys.State.AppStateBuilder;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Sxc.Code.Generate.Data;
using ToSic.Sxc.Code.Generate.Sys;
using Xunit.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.CodeGeneration;

[Startup(typeof(StartupCodeGenerationTests))]
public class CSharpModelGeneratorTests(
    LazySvc<IEnumerable<IFileGenerator>> generators,
    ContentTypeAssemblyKit ctAssemblyKit,
    DataAssembler dataAssembler,
    ContentTypesFromCodeManager ctsFromCodeManager,
    IDataFactory dataFactory,
    IAppStateBuilder appStateBuilder)
{
    #region Data Model Generation

    [Fact]
    public void Generate_SkipsEphemeralFields()
    {
        var context = CodeGeneratorTestContext.Create(ctAssemblyKit, dataAssembler, ctsFromCodeManager, dataFactory, appStateBuilder);

        AssertGeneratorSkipsEphemeral(new CSharpTypedDataModelsGenerator(context.User, context.AppReaders), context);
        AssertGeneratorSkipsEphemeral(new CSharpCustomModelsGenerator(context.User, context.AppReaders), context);
    }

    #endregion

    #region Auto Generate

    [Fact]
    public async Task AutoGenerate_MatchingConfiguration_CompletesWithoutExceptions()
    {
        using var context = AutoGenerateTestContext.Create(ctAssemblyKit, dataAssembler, ctsFromCodeManager, dataFactory, appStateBuilder, generators);

        var result = await context.RunAsync();

        Assert.Empty(result.Exceptions);
    }

    [Fact]
    public async Task AutoGenerate_MatchingConfiguration_InvokesGeneratorOnce()
    {
        using var context = AutoGenerateTestContext.Create(ctAssemblyKit, dataAssembler, ctsFromCodeManager, dataFactory, appStateBuilder, generators);

        await context.RunAsync();

        Assert.Single(context.Generator.ReceivedSpecs);
    }

    [Fact]
    public async Task AutoGenerate_MatchingConfiguration_PassesAppId()
    {
        using var context = AutoGenerateTestContext.Create(ctAssemblyKit, dataAssembler, ctsFromCodeManager, dataFactory, appStateBuilder, generators);

        await context.RunAsync();

        var specs = Assert.Single(context.Generator.ReceivedSpecs);
        Assert.Equal(CodeGeneratorTestContext.AppId, specs.AppId);
    }

    [Fact]
    public async Task AutoGenerate_MatchingConfiguration_PassesContentType()
    {
        using var context = AutoGenerateTestContext.Create(ctAssemblyKit, dataAssembler, ctsFromCodeManager, dataFactory, appStateBuilder, generators);

        await context.RunAsync();

        var specs = Assert.Single(context.Generator.ReceivedSpecs);
        Assert.Equal([context.ContentType.NameId], specs.ContentTypes);
    }

    /// <summary>
    /// Regression: the auto-generate job used to narrow the generator's type universe to the changed
    /// content type, so every entity-field on it degraded to ITypedItem - see the ITypedItem fallback test below.
    /// </summary>
    [Fact]
    public async Task AutoGenerate_EntityField_KeepsTypedChildReference()
    {
        // Configuration with no explicit content types = the default set, which is the common setup
        using var context = AutoGenerateTestContext.Create(ctAssemblyKit, dataAssembler, ctsFromCodeManager, dataFactory, appStateBuilder, generators, configuredContentTypes: "");

        await context.RunAsync();

        var specs = Assert.Single(context.Generator.ReceivedSpecs);
        var body = GenerateBody(new CSharpTypedDataModelsGenerator(context.CodeContext.User, context.CodeContext.AppReaders), specs, context.ContentType.Name);

        Assert.Contains($"public {CodeGeneratorTestContext.EntityFieldAndTypeName} {CodeGeneratorTestContext.EntityFieldAndTypeName} =>", body);
        Assert.Contains($"_item.Child<{CodeGeneratorTestContext.EntityFieldAndTypeName}>(\"{CodeGeneratorTestContext.EntityFieldAndTypeName}\")", body);
    }

    /// <summary>
    /// Documents the coupling: a type universe narrowed to the one type being written cannot resolve
    /// cross-type references, so the entity-field falls back to ITypedItem.
    /// </summary>
    [Fact]
    public void Generate_EntityField_NarrowedToSingleType_FallsBackToITypedItem()
    {
        var context = CodeGeneratorTestContext.Create(ctAssemblyKit, dataAssembler, ctsFromCodeManager, dataFactory, appStateBuilder);

        var body = GenerateBody(
            new CSharpTypedDataModelsGenerator(context.User, context.AppReaders),
            new TestFileGeneratorSpecs { AppId = CodeGeneratorTestContext.AppId, ContentTypes = [context.ContentType.NameId] },
            context.ContentType.Name);

        Assert.Contains($"public ITypedItem {CodeGeneratorTestContext.EntityFieldAndTypeName} =>", body);
        Assert.Contains($"_item.Child(\"{CodeGeneratorTestContext.EntityFieldAndTypeName}\")", body);
    }

    [Fact]
    public async Task AutoGenerate_MatchingConfiguration_WritesGeneratedFile()
    {
        using var context = AutoGenerateTestContext.Create(ctAssemblyKit, dataAssembler, ctsFromCodeManager, dataFactory, appStateBuilder, generators);

        await context.RunAsync();

        Assert.True(File.Exists(context.GeneratedFilePath));
    }

    #endregion

    #region Helpers

    private static void AssertGeneratorSkipsEphemeral(IFileGenerator generator, CodeGeneratorTestContext context)
    {
        var body = generator
            .Generate(new TestFileGeneratorSpecs
            {
                AppId = CodeGeneratorTestContext.AppId,
                ContentTypes = [context.ContentType.NameId]
            })
            .Single()
            .Files
            .Single()
            .Body;

        Assert.Contains("Title =>", body);
        Assert.DoesNotContain("HasData =>", body);
    }

    /// <summary>
    /// Generate everything the specs ask for, then pick the file of one content type.
    /// </summary>
    private static string GenerateBody(IFileGenerator generator, IFileGeneratorSpecs specs, string typeName)
        => generator
            .Generate(specs)
            .Single()
            .Files
            .Single(f => f.FileName == $"{typeName}.Generated.cs")
            .Body;

    #endregion
}
