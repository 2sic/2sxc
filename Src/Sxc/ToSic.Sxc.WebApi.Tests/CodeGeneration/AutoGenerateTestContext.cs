using ToSic.Eav.Apps.Mocks;
using ToSic.Eav.Apps.Sys.State.AppStateBuilder;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.Data.Processing;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.WebApi.Tests.CodeGeneration;

internal sealed class AutoGenerateTestContext : IDisposable
{
    private readonly CopilotContentTypeAutoGenerateAction _action;
    private readonly ContentTypeChange _change;
    private readonly string _appRoot;

    public CodeGeneratorTestContext CodeContext { get; }
    public IContentType ContentType => CodeContext.ContentType;
    public TestAutoGenerateFileGenerator Generator { get; }
    public string GeneratedFilePath { get; }

    private AutoGenerateTestContext(
        CodeGeneratorTestContext codeContext,
        CopilotContentTypeAutoGenerateAction action,
        ContentTypeChange change,
        TestAutoGenerateFileGenerator generator,
        string appRoot,
        string generatedFilePath)
    {
        CodeContext = codeContext;
        _action = action;
        _change = change;
        Generator = generator;
        _appRoot = appRoot;
        GeneratedFilePath = generatedFilePath;
    }

    public static AutoGenerateTestContext Create(
        ContentTypeAssemblyKit ctAssemblyKit,
        DataAssembler dataAssembler,
        ContentTypesFromCodeManager ctsFromCodeManager,
        IDataFactory dataFactory,
        IAppStateBuilder appStateBuilder,
        LazySvc<IEnumerable<IFileGenerator>> generators,
        string? configuredContentTypes = null)
    {
        var codeContext = CodeGeneratorTestContext.CreateWithAutoGenerateConfiguration(
            ctAssemblyKit,
            dataAssembler,
            ctsFromCodeManager,
            dataFactory,
            appStateBuilder,
            configuredContentTypes);

        var appRoot = Path.Combine(Path.GetTempPath(), $"{nameof(CSharpModelGeneratorTests)}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(appRoot);
        var generator = new TestAutoGenerateFileGenerator();
        generators.Inject([generator]);

        return new(
            codeContext,
            CreateAction(appRoot, codeContext, generators),
            new ContentTypeChange(CodeGeneratorTestContext.AppId, codeContext.ContentType.NameId, ContentTypeChangeSources.ContentType),
            generator,
            appRoot,
            Path.Combine(appRoot, "AppCode", "Data", TestAutoGenerateFileGenerator.FileName));
    }

    public Task<Package<ContentTypeChange>> RunAsync()
        => _action.Handle(new(), _change.ToPackage());

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_appRoot))
                Directory.Delete(_appRoot, recursive: true);
        }
        catch
        {
            // Ignore cleanup failures; the test assertion result is more important.
        }
    }

    private static CopilotContentTypeAutoGenerateAction CreateAction(
        string appRoot,
        CodeGeneratorTestContext codeContext,
        LazySvc<IEnumerable<IFileGenerator>> generators)
    {
        var fileSaver = new FileSaver(MockSiteTestHelpers.CreateSite(appRoot), codeContext.AppReaders, new MockAppPathsMicroSvc(appRoot));
        var codeGenerate = new CopilotCodeGenerateService(fileSaver, generators, codeContext.AppReaders);
        return new(codeGenerate, codeContext.AppReaders);
    }
}
