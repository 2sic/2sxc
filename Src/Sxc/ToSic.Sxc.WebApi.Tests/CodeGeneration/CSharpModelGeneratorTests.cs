using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.State.AppStateBuilder;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.Attributes;
using ToSic.Eav.Run.Startup;
using ToSic.Sxc.Code.Generate.Data;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sys.Users;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.CodeGeneration;

public class CSharpModelGeneratorTests
{
    [Fact]
    public void Generate_SkipsEphemeralFields()
    {
        using var context = CodeGeneratorTestContext.Create();

        AssertGeneratorSkipsEphemeral(new CSharpTypedDataModelsGenerator(context.User, context.AppReaders), context);
        AssertGeneratorSkipsEphemeral(new CSharpCustomModelsGenerator(context.User, context.AppReaders), context);
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
        {
            var services = new ServiceCollection();
            new StartupTestsEavDataBuild().ConfigureServices(services);
            services.AddEavApps();

            var serviceProvider = services.BuildServiceProvider()
                                  ?? throw new InvalidOperationException("Failed to build service provider");

            var contentType = CreateContentType(serviceProvider);

            var appBuilder = serviceProvider.GetRequiredService<IAppStateBuilder>().InitForPreset();
            appBuilder.Load("test code generation content types", _ =>
            {
                appBuilder.InitMetadata();
                appBuilder.InitContentTypes(new List<IContentType> { contentType });
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
