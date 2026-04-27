using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.Data.Sys.Dimensions;
using ToSic.Eav.Data.Sys.Entities.Sources;
using ToSic.Sxc.Backend.SaveHelpers;

namespace ToSic.Sxc.WebApi.Tests.SaveHelpers;

internal sealed class IsUniqueValidatorTestContext : IDisposable
{
    private const int AppId = 42;

    private readonly ServiceProvider _serviceProvider;
    private int _attributeId;
    private int _entityId = 1000;

    private IsUniqueValidatorTestContext(ServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        DataAssembler = serviceProvider.GetRequiredService<DataAssembler>();
        ContentTypeAssembler = serviceProvider.GetRequiredService<ContentTypeAssembler>();
    }

    public DataAssembler DataAssembler { get; }

    public ContentTypeAssembler ContentTypeAssembler { get; }

    public IsUniqueValidator CreateValidator()
        => new(new Log("Tst.Unq"));

    public static IsUniqueValidatorTestContext Create()
    {
        var services = new ServiceCollection();
        new StartupTestsEavDataBuild().ConfigureServices(services);
        var serviceProvider = services.BuildServiceProvider()
                              ?? throw new InvalidOperationException("Failed to build service provider");
        return new(serviceProvider);
    }

    public IContentType CreateType(string name, params IContentTypeAttribute[] attributes)
        => ContentTypeAssembler.Type.CreateContentTypeTac(
            appId: AppId,
            name: name,
            nameId: name,
            scope: "TestScope",
            attributes: attributes.ToList()
        );

    public IContentTypeAttribute CreateField(string name, ValueTypes type, bool isUnique = false, bool isTitle = false, string? inputType = default)
        => CreateField(name, type, uniqueSetting: isUnique ? true : null, isTitle: isTitle, inputType: inputType, includeUniqueMetadata: isUnique);

    public IContentTypeAttribute CreateField(string name, ValueTypes type, bool? uniqueSetting, bool isTitle = false, string? inputType = default, bool includeUniqueMetadata = true)
    {
        List<IEntity>? metadataItems = null;
        if (includeUniqueMetadata)
            (metadataItems ??= []).Add(CreateUniqueMetadataEntity(uniqueSetting));

        if (!string.IsNullOrWhiteSpace(inputType))
            (metadataItems ??= []).Add(CreateInputTypeMetadataEntity(inputType!));

        return ContentTypeAssembler.Attribute.Create(
            appId: AppId,
            name: name,
            type: type,
            isTitle: isTitle,
            id: ++_attributeId,
            sortOrder: _attributeId,
            metadataItems: metadataItems
        );
    }

    public IEntity CreateEntity(IContentType contentType, Guid guid, params IAttribute[] attributes)
    {
        var typedValues = attributes.ToDictionary(attribute => attribute.Name, attribute => attribute, StringComparer.InvariantCultureIgnoreCase);
        var titleField = contentType.Attributes.FirstOrDefault(attribute => attribute.IsTitle)?.Name;

        return DataAssembler.CreateEntityTac(
            appId: AppId,
            contentType: contentType,
            typedValues: typedValues,
            entityId: ++_entityId,
            repositoryId: _entityId,
            guid: guid,
            titleField: titleField,
            owner: "test:1"
        );
    }

    public IAttribute InvariantAttribute(string name, ValueTypes type, object? value)
        => BuildAttribute(name, type, (value, Array.Empty<string>()));

    public IAttribute InvariantAttributeValues(string name, ValueTypes type, params object?[] values)
        => BuildAttribute(name, type, values.Select(value => (Value: value, Languages: Array.Empty<string>())).ToArray());

    public IAttribute LocalizedAttribute(string name, ValueTypes type, params (object? Value, string Language)[] values)
        => BuildAttribute(name, type, values.Select(value => (value.Value, new[] { value.Language })).ToArray());

    public IAttribute EntityRelationshipAttributeUsingRepoIds(string name, params IEntity[] relatedEntities)
        => DirectEntitiesSource.Using(sourceAndList =>
        {
            sourceAndList.List.AddRange(relatedEntities);
            var references = relatedEntities
                .Select(entity => (int?)(entity.RepositoryId > 0 ? entity.RepositoryId : entity.EntityId))
                .ToList();
            var relationship = DataAssembler.Relationship.Relationship(
                DataAssembler.Relationship.ToSource(references, sourceAndList.Source)
            );
            return DataAssembler.Attribute.Create(name, ValueTypes.Entity, [relationship]);
        });

    private IAttribute BuildAttribute(string name, ValueTypes type, params (object? Value, string[] Languages)[] values)
    {
        var rawValues = values
            .Select(value => DataAssembler.Value.Create(type, value.Value, BuildLanguages(value.Languages)))
            .ToList();

        return DataAssembler.Attribute.Create(name, type, rawValues);
    }

    private static IImmutableList<ILanguage> BuildLanguages(IEnumerable<string> languages)
        => languages
            .Select((language, index) => (ILanguage)new Language(language, readOnly: false, dimensionId: index + 1))
            .ToImmutableList();

    private IEntity CreateUniqueMetadataEntity(bool? isUnique)
    {
        var metadataAttribute = ContentTypeAssembler.Attribute.Create(
            appId: AppId,
            name: "IsUnique",
            type: ValueTypes.Boolean,
            isTitle: false,
            id: ++_attributeId,
            sortOrder: _attributeId
        );

        var metadataType = ContentTypeAssembler.Type.CreateContentTypeTac(
            appId: AppId,
            name: "UniqueFieldMetadata",
            nameId: "UniqueFieldMetadata",
            scope: "TestMetadata",
            attributes: new List<IContentTypeAttribute> { metadataAttribute }
        );

        var values = new Dictionary<string, object>();
        if (isUnique.HasValue)
            values["IsUnique"] = isUnique.Value;

        return DataAssembler.CreateEntityTac(
            appId: AppId,
            contentType: metadataType,
            values: values,
            entityId: ++_entityId,
            repositoryId: _entityId,
            guid: Guid.NewGuid(),
            owner: "test:metadata"
        );
    }

    private IEntity CreateInputTypeMetadataEntity(string inputType)
    {
        var metadataAttribute = ContentTypeAssembler.Attribute.Create(
            appId: AppId,
            name: "InputType",
            type: ValueTypes.String,
            isTitle: false,
            id: ++_attributeId,
            sortOrder: _attributeId
        );

        var metadataType = ContentTypeAssembler.Type.CreateContentTypeTac(
            appId: AppId,
            name: "@All",
            nameId: "@All",
            scope: "TestMetadata",
            attributes: new List<IContentTypeAttribute> { metadataAttribute }
        );

        return DataAssembler.CreateEntityTac(
            appId: AppId,
            contentType: metadataType,
            values: new Dictionary<string, object> { { "InputType", inputType } },
            entityId: ++_entityId,
            repositoryId: _entityId,
            guid: Guid.NewGuid(),
            owner: "test:metadata"
        );
    }

    public void Dispose()
        => _serviceProvider.Dispose();
}
