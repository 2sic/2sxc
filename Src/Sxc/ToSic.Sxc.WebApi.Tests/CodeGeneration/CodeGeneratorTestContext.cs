using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.State.AppStateBuilder;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.Attributes;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.WebApi.Tests.CodeGeneration;

internal sealed class CodeGeneratorTestContext
{
    internal const int AppId = 42;

    public IContentType ContentType { get; }
    public IUser User { get; } = new UserMock
    {
        Id = 1,
        IdentityToken = "test:1",
        Username = "test",
        Name = "Test User",
        Email = "test@example.com",
        IsSystemAdmin = true,
        IsAnonymous = false,
    };
    public IAppReaderFactory AppReaders { get; }

    private CodeGeneratorTestContext(IContentType contentType, IAppReader reader)
    {
        ContentType = contentType;
        AppReaders = new TestAppReaderFactory(reader);
    }

    public static CodeGeneratorTestContext Create(
        ContentTypeAssemblyKit ctAssemblyKit,
        DataAssembler dataAssembler,
        ContentTypesFromCodeManager ctsFromCodeManager,
        IDataFactory dataFactory,
        IAppStateBuilder appStateBuilder)
        => Create(ctAssemblyKit, dataAssembler, ctsFromCodeManager, dataFactory, appStateBuilder, false);

    public static CodeGeneratorTestContext CreateWithAutoGenerateConfiguration(
        ContentTypeAssemblyKit ctAssemblyKit,
        DataAssembler dataAssembler,
        ContentTypesFromCodeManager ctsFromCodeManager,
        IDataFactory dataFactory,
        IAppStateBuilder appStateBuilder)
        => Create(ctAssemblyKit, dataAssembler, ctsFromCodeManager, dataFactory, appStateBuilder, true);

    private static CodeGeneratorTestContext Create(
        ContentTypeAssemblyKit ctAssemblyKit,
        DataAssembler dataAssembler,
        ContentTypesFromCodeManager ctsFromCodeManager,
        IDataFactory dataFactory,
        IAppStateBuilder appStateBuilder,
        bool includeAutoGenerateConfiguration)
    {
        var contentType = CreateContentType(ctAssemblyKit, dataAssembler);
        var contentTypes = new List<IContentType> { contentType };
        IEntity? autoGenerateConfiguration = null;

        if (includeAutoGenerateConfiguration)
        {
            var configurationType = ctsFromCodeManager.Get<DataCopilotConfiguration>();
            contentTypes.Add(configurationType);
            autoGenerateConfiguration = CreateAutoGenerateConfiguration(dataFactory, contentType.NameId);
        }

        var appBuilder = appStateBuilder.InitForPreset();
        appBuilder.Load("test code generation content types", _ =>
        {
            appBuilder.InitMetadata();
            appBuilder.InitContentTypes(contentTypes);

            if (autoGenerateConfiguration != null)
                appBuilder.Add(autoGenerateConfiguration, publishedId: null, log: false);
        });

        return new(contentType, appBuilder.Reader);
    }

    private static IContentType CreateContentType(ContentTypeAssemblyKit ctAssemblyKit, DataAssembler dataAssembler)
    {
        var attributeId = 0;
        var entityId = 1000;

        var title = ctAssemblyKit.Field.Create(
            appId: AppId,
            name: "Title",
            type: ValueTypes.String,
            isTitle: true,
            id: ++attributeId,
            sortOrder: attributeId
        );

        var hasData = ctAssemblyKit.Field.Create(
            appId: AppId,
            name: "HasData",
            type: ValueTypes.Boolean,
            isTitle: false,
            id: ++attributeId,
            sortOrder: attributeId,
            metadataItems: [CreateEphemeralMetadataEntity(ctAssemblyKit, dataAssembler, ref attributeId, ref entityId)]
        );

        return ctAssemblyKit.Type.CreateContentTypeTac(
            appId: AppId,
            name: "Article",
            id: 7,
            nameId: "Article",
            scope: ScopeConstants.Default,
            attributes: new List<IContentTypeAttribute> { title, hasData }
        );
    }

    private static IEntity CreateAutoGenerateConfiguration(IDataFactory dataFactory, string contentTypeNameId)
    {
        var config = new DataCopilotConfiguration
        {
            Id = 2000,
            CodeGenerator = TestAutoGenerateFileGenerator.GeneratorName,
            AutoGenerate = true,
            ContentTypes = contentTypeNameId,
        };

        return dataFactory.CreateTac(config);
    }

    private static IEntity CreateEphemeralMetadataEntity(
        ContentTypeAssemblyKit ctAssemblyKit,
        DataAssembler dataAssembler,
        ref int attributeId,
        ref int entityId)
    {
        var metadataAttribute = ctAssemblyKit.Field.Create(
            appId: AppId,
            name: AttributeMetadataConstants.MetadataFieldAllIsEphemeral,
            type: ValueTypes.Boolean,
            isTitle: false,
            id: ++attributeId,
            sortOrder: attributeId
        );

        var metadataType = ctAssemblyKit.Type.CreateContentTypeTac(
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
}
