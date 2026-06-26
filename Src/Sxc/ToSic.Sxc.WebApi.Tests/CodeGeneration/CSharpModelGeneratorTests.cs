using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Apps.Sys.State.AppStateBuilder;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.Attributes;
using ToSic.Eav.Run.Startup;
using ToSic.Sxc.Code.Generate.Data;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sys.Users;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.CodeGeneration;

// TODO: SETUP ISN'T good - should use constructor dependency injection, see example ContentTypeFactoryIsConfigured
public class CSharpModelGeneratorTests
{
    [Fact]
    public void Generate_SkipsEphemeralFields()
    {
        using var context = CodeGeneratorTestContext.Create();

        AssertGeneratorSkipsEphemeral(new CSharpTypedDataModelsGenerator(context.User, context.AppReaders), context);
        AssertGeneratorSkipsEphemeral(new CSharpCustomModelsGenerator(context.User, context.AppReaders), context);
    }

    [Fact]
    public async Task AutoGenerate_MatchingConfiguration_WritesGeneratedFile()
    {
        using var context = CodeGeneratorTestContext.CreateWithAutoGenerateConfiguration();
        // TODO: SHOULD Be changed to test-harness like other tests
        using var generatorServiceProvider = new ServiceCollection().BuildServiceProvider();

        var appRoot = Path.Combine(Path.GetTempPath(), $"{nameof(CSharpModelGeneratorTests)}-{Guid.NewGuid():N}");
        Directory.CreateDirectory(appRoot);

        try
        {
            var generator = new TestAutoGenerateFileGenerator();
            var lazyGenerators = new LazySvc<IEnumerable<IFileGenerator>>(generatorServiceProvider);
            lazyGenerators.Inject([generator]);

            var fileSaver = new FileSaver(new TestSite(appRoot), context.AppReaders, new TestAppPathsMicroSvc(appRoot));
            var codeGenerate = new CopilotCodeGenerateService(fileSaver, lazyGenerators, context.AppReaders);
            var action = new CopilotContentTypeAutoGenerateAction(codeGenerate, context.AppReaders);
            var change = new ContentTypeChange(CodeGeneratorTestContext.AppId, context.ContentType.NameId, ContentTypeChangeSources.ContentType);

            var result = await action.Run(new LowCodeActionContext(), ActionData.Create(change));

            Assert.Empty(result.Exceptions);

            var specs = Assert.Single(generator.ReceivedSpecs);
            Assert.Equal(CodeGeneratorTestContext.AppId, specs.AppId);
            Assert.Equal([context.ContentType.NameId], specs.ContentTypes);
            Assert.True(File.Exists(Path.Combine(appRoot, "AppCode", "Data", TestAutoGenerateFileGenerator.FileName)));
        }
        finally
        {
            if (Directory.Exists(appRoot))
                Directory.Delete(appRoot, recursive: true);
        }
    }

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

    private sealed class CodeGeneratorTestContext : IDisposable
    {
        internal const int AppId = 42;
        //private const string FieldContentTypes = "ContentTypes";
        //private const string FieldNamespace = "Namespace";
        //private const string FieldTargetFolder = "TargetFolder";
        //private const string FieldPrefix = "Prefix";
        //private const string FieldSuffix = "Suffix";
        //private const string FieldEdition = "Edition";

        private readonly ServiceProvider _serviceProvider;

        public IContentType ContentType { get; }
        public IUser User { get; } = new TestUser();
        public IAppReaderFactory AppReaders { get; }

        private CodeGeneratorTestContext(ServiceProvider serviceProvider, IContentType contentType, IAppReader reader)
        {
            _serviceProvider = serviceProvider;
            ContentType = contentType;
            AppReaders = new TestAppReaderFactory(reader);
        }

        public static CodeGeneratorTestContext Create()
            => Create(false);

        public static CodeGeneratorTestContext CreateWithAutoGenerateConfiguration()
            => Create(true);

        private static CodeGeneratorTestContext Create(bool includeAutoGenerateConfiguration)
        {
            var services = new ServiceCollection();
            new StartupTestsEavDataBuild().ConfigureServices(services);
            services.AddEavApps();

            var serviceProvider = services.BuildServiceProvider()
                                  ?? throw new InvalidOperationException("Failed to build service provider");

            var contentType = CreateContentType(serviceProvider);
            var contentTypes = new List<IContentType> { contentType };
            IEntity? autoGenerateConfiguration = null;

            if (includeAutoGenerateConfiguration)
            {
                var configurationType = CreateAutoGenerateConfigurationType(serviceProvider);
                contentTypes.Add(configurationType);
                autoGenerateConfiguration = CreateAutoGenerateConfiguration(serviceProvider, configurationType, contentType.NameId);
            }

            var appBuilder = serviceProvider.GetRequiredService<IAppStateBuilder>().InitForPreset();
            appBuilder.Load("test code generation content types", _ =>
            {
                appBuilder.InitMetadata();
                appBuilder.InitContentTypes(contentTypes);

                if (autoGenerateConfiguration != null)
                    appBuilder.Add(autoGenerateConfiguration, publishedId: null, log: false);
            });

            return new(serviceProvider, contentType, appBuilder.Reader);
        }

        private static IContentType CreateContentType(IServiceProvider serviceProvider)
        {
            var contentTypeAssembler = serviceProvider.GetRequiredService<ContentTypeAssembler>();
            var dataAssembler = serviceProvider.GetRequiredService<DataAssembler>();
            var attributeId = 0;
            var entityId = 1000;

            var title = contentTypeAssembler.Attribute.Create(
                appId: AppId,
                name: "Title",
                type: ValueTypes.String,
                isTitle: true,
                id: ++attributeId,
                sortOrder: attributeId
            );

            var hasData = contentTypeAssembler.Attribute.Create(
                appId: AppId,
                name: "HasData",
                type: ValueTypes.Boolean,
                isTitle: false,
                id: ++attributeId,
                sortOrder: attributeId,
                metadataItems: [CreateEphemeralMetadataEntity(contentTypeAssembler, dataAssembler, ref attributeId, ref entityId)]
            );

            return contentTypeAssembler.Type.CreateContentTypeTac(
                appId: AppId,
                name: "Article",
                id: 7,
                nameId: "Article",
                scope: ScopeConstants.Default,
                attributes: new List<IContentTypeAttribute> { title, hasData }
            );
        }

        private static IContentType CreateAutoGenerateConfigurationType(IServiceProvider serviceProvider)
        {
            var codeContentTypeManager = serviceProvider.GetRequiredService<CodeContentTypesManager>();
            return codeContentTypeManager.Get<DataCopilotConfiguration>(); // note: should use Tac, but that's in another test helper project
            
            //var contentTypeAssembler = serviceProvider.GetRequiredService<ContentTypeAssembler>();
            //var attributeId = 100;

            //IContentTypeAttribute Attribute(string name, ValueTypes type, bool isTitle = false)
            //    => contentTypeAssembler.Attribute.Create(
            //        appId: AppId,
            //        name: name,
            //        type: type,
            //        isTitle: isTitle,
            //        id: ++attributeId,
            //        sortOrder: attributeId
            //    );

            //return contentTypeAssembler.Type.CreateContentTypeTac(
            //    appId: AppId,
            //    name: CopilotContentTypeAutoGenerateAction.DataCopilotConfiguration.DataCopilotConfigurationContentType,
            //    id: 8,
            //    nameId: CopilotContentTypeAutoGenerateAction.DataCopilotConfiguration.DataCopilotConfigurationContentType,
            //    scope: ScopeConstants.Default,
            //    attributes:
            //    [
            //        Attribute(CopilotCodeGenerateService.FieldCodeGenerator, ValueTypes.String, isTitle: true),
            //        Attribute(CopilotCodeGenerateService.FieldAutoGenerate, ValueTypes.Boolean),
            //        Attribute(FieldContentTypes, ValueTypes.String),
            //        Attribute(FieldNamespace, ValueTypes.String),
            //        Attribute(FieldTargetFolder, ValueTypes.String),
            //        Attribute(FieldPrefix, ValueTypes.String),
            //        Attribute(FieldSuffix, ValueTypes.String),
            //        Attribute(FieldEdition, ValueTypes.String),
            //    ]
            //);
        }

        private static IEntity CreateAutoGenerateConfiguration(
            IServiceProvider serviceProvider,
            IContentType configurationType,
            string contentTypeNameId)
        {
            var config = new DataCopilotConfiguration()
            {
                Id = 2000,
                CodeGenerator = TestAutoGenerateFileGenerator.GeneratorName,
                AutoGenerate = true,
                ContentTypes = contentTypeNameId,
            };

            var codeCtManager = serviceProvider.Build<IDataFactory>();
            return codeCtManager.Create(config);
            
            //const int entityId = 2000;
            //var dataAssembler = serviceProvider.GetRequiredService<DataAssembler>();

            //return dataAssembler.CreateEntityTac(
            //    appId: AppId,
            //    contentType: configurationType,
            //    values: new Dictionary<string, object>
            //    {
            //        { CopilotCodeGenerateService.FieldCodeGenerator, TestAutoGenerateFileGenerator.GeneratorName },
            //        { CopilotCodeGenerateService.FieldAutoGenerate, true },
            //        { FieldContentTypes, contentTypeNameId },
            //        { FieldNamespace, "" },
            //        { FieldTargetFolder, "" },
            //        { FieldPrefix, "" },
            //        { FieldSuffix, "" },
            //        { FieldEdition, "" }
            //    },
            //    entityId: entityId,
            //    repositoryId: entityId,
            //    guid: Guid.NewGuid(),
            //    titleField: CopilotCodeGenerateService.FieldCodeGenerator,
            //    owner: "test:auto-generate"
            //);
        }

        private static IEntity CreateEphemeralMetadataEntity(
            ContentTypeAssembler contentTypeAssembler,
            DataAssembler dataAssembler,
            ref int attributeId,
            ref int entityId)
        {
            var metadataAttribute = contentTypeAssembler.Attribute.Create(
                appId: AppId,
                name: AttributeMetadataConstants.MetadataFieldAllIsEphemeral,
                type: ValueTypes.Boolean,
                isTitle: false,
                id: ++attributeId,
                sortOrder: attributeId
            );

            var metadataType = contentTypeAssembler.Type.CreateContentTypeTac(
                appId: AppId,
                name: AttributeMetadataConstants.TypeGeneral,
                nameId: AttributeMetadataConstants.TypeGeneral,
                scope: "TestMetadata",
                attributes: new List<IContentTypeAttribute> { metadataAttribute }
            );

            return dataAssembler.CreateEntityTac(
                appId: AppId,
                contentType: metadataType,
                values: new Dictionary<string, object>
                {
                    { AttributeMetadataConstants.MetadataFieldAllIsEphemeral, true }
                },
                entityId: ++entityId,
                repositoryId: entityId,
                guid: Guid.NewGuid(),
                owner: "test:metadata"
            );
        }

        public void Dispose()
            => _serviceProvider.Dispose();
    }

    private sealed class TestAutoGenerateFileGenerator : IFileGenerator
    {
        internal const string GeneratorName = "TestAutoGenerateGenerator";
        internal const string FileName = "AutoGenerated.txt";

        private readonly List<IFileGeneratorSpecs> _receivedSpecs = [];

        public IReadOnlyList<IFileGeneratorSpecs> ReceivedSpecs => _receivedSpecs;
        public string NameId => Name;
        public string Name => GeneratorName;
        public string Version => "1.0.0";
        public string Description => "Test auto-generate file generator";
        public string DescriptionHtml => Description;
        public string OutputLanguage => "Text";
        public string OutputType => "Test";

        public IGeneratedFileSet[] Generate(IFileGeneratorSpecs specs)
        {
            _receivedSpecs.Add(specs);

            return
            [
                new GeneratedFileSet
                {
                    Name = "Test auto-generate output",
                    Description = "Test auto-generate output",
                    Generator = Name,
                    Path = GenerateConstants.PathToAppCode,
                    Files =
                    [
                        new GeneratedFile
                        {
                            FileName = FileName,
                            Path = "Data",
                            Body = "auto-generated"
                        }
                    ]
                }
            ];
        }
    }

    private sealed record TestFileGeneratorSpecs : IFileGeneratorSpecs
    {
        public string? Configuration { get; init; }
        public int AppId { get; init; }
        public string? Edition { get; init; }
        public DateTime DateTime { get; init; } = DateTime.Now;
        public string? Namespace { get; init; }
        public string? TargetPath { get; init; }
        public ICollection<string>? ContentTypes { get; init; }
        public string? Prefix { get; init; }
        public string? Suffix { get; init; }
    }

    private sealed class TestAppReaderFactory(IAppReader reader) : IAppReaderFactory
    {
        public IAppReader GetOrKeep(IAppIdentity appIdOrReader) => reader;
        public IAppReader GetZonePrimary(int zoneId) => reader;
        public IAppReader? TryGetSystemPreset(bool nullIfNotLoaded) => reader;
        public IAppIdentityPure AppIdentity(int appId) => new AppIdentityPure(reader.ZoneId, appId);
        public IAppReader? ToReader(IAppStateCache state) => reader;
        public IAppReader Get(int appId) => reader;
        public IAppReader? TryGet(IAppIdentity appIdentity) => reader;
        public IAppReader Get(IAppIdentity appIdentity) => reader;
        public IAppReader GetSystemPreset() => reader;
    }

    private sealed class TestAppPathsMicroSvc(string root) : IAppPathsMicroSvc
    {
        public IAppPaths Get(IAppReader appReader) => new TestAppPaths(root);
        public IAppPaths Get(IAppReader appReader, ISite? siteOrNull) => new TestAppPaths(root);
    }

    private sealed class TestAppPaths(string physicalPath) : IAppPaths
    {
        public string Path => "/";
        public string PhysicalPath { get; } = physicalPath;
        public string PathShared => "/";
        public string PhysicalPathShared { get; } = physicalPath;
        public string RelativePath => "/";
        public string RelativePathShared => "/";
    }

    // TODO: this is not ideal - test site models should already exist, and should be reused - possibly improved and merged (search for MockSite)
    // this is just more code to maintain
    private sealed class TestSite(string appsRootPhysicalFull) : ISite
    {
        public ISite Init(int siteId, ILog? parentLogOrNull) => this;
        public int Id { get; } = 1;
        public string Name { get; } = "Test";
        public string AppsRootPhysical { get; } = appsRootPhysicalFull;
        public string AppsRootPhysicalFull { get; } = appsRootPhysicalFull;
        public string AppAssetsLinkTemplate { get; } = "/app/{appFolder}";
        public string ContentPath { get; } = "/";
        public string Url { get; } = "/";
        public string UrlRoot { get; } = "/";
        public string CurrentCultureCode { get; } = "en-us";
        public string DefaultCultureCode { get; } = "en-us";
        public int ZoneId { get; } = 1;
    }

    // TODO: this is not ideal - test user models already exists - see UserMock, and should be reused
    // this is just more code to maintain
    private sealed class TestUser : IUser
    {
        public int Id => 1;
        public string IdentityToken => "test:1";
        public Guid Guid => Guid.Empty;
        public string Username => "test";
        public string Name => "Test User";
        public string Email => "test@example.com";
        public List<int> Roles => [];
        public bool IsSystemAdmin => true;
        public bool IsSiteAdmin => true;
        public bool IsContentAdmin => true;
        public bool IsContentEditor => true;
        public bool IsSiteDeveloper => true;
        public bool IsAnonymous => false;
    }
}
