using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.Services;
using ToSic.Sxc.Backend.SaveHelpers;
using Xunit.DependencyInjection;

namespace ToSic.Sxc.WebApi.Tests.SaveHelpers;

[Startup(typeof(StartupCoreDataSourcesAndTestData))]
public class UniqueValueValidationTests
(
    DataAssembler dataAssembler,
    ContentTypeAssemblyKit ctAssemblyKit,
    IDataSourcesService dataSourcesService,
    DataSourceBase.Dependencies dataSourceDependencies)
{
    #region Unique Validation

    [Fact]
    public void DuplicateExistingValue_ReturnsDuplicateConflict()
    {
        using var ctx = CreateContext();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var result = Validate(ctx, [type], [existing], new(type.NameId, "Slug", "same-slug"));

        Assert.False(result.IsValid);
        Assert.Equal("duplicate", result.Reason);
        Assert.Equal(existing.EntityId, result.ConflictEntityId);
        Assert.Equal(existing.GetBestTitle(), result.ConflictTitle);
    }

    [Fact]
    public void CurrentEntityWithSameValue_ReturnsOk()
    {
        using var ctx = CreateContext();
        var entityGuid = Guid.NewGuid();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var current = ctx.CreateEntity(type, entityGuid,
            ctx.InvariantAttribute("Title", ValueTypes.String, "Current"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var result = Validate(ctx, [type], [current], new(type.NameId, "Slug", "same-slug", CurrentEntity: current));

        Assert.True(result.IsValid);
        Assert.Equal("ok", result.Reason);
    }

    [Fact]
    public void BlankValue_ReturnsBlank()
    {
        using var ctx = CreateContext();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var result = Validate(ctx, [type], [], new(type.NameId, "Slug", "   "));

        Assert.True(result.IsValid);
        Assert.Equal("blank", result.Reason);
    }

    [Fact]
    public void DuplicateExistingDateValue_WithUiIsoRequest_ReturnsDuplicateConflict()
    {
        using var ctx = CreateContext();
        var date = new DateTime(2026, 5, 20, 0, 0, 0, DateTimeKind.Unspecified);
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("PublishDate", ValueTypes.DateTime, isUnique: true));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("PublishDate", ValueTypes.DateTime, date));

        var result = Validate(ctx, [type], [existing], new(type.NameId, "PublishDate", "2026-05-20T00:00:00.000Z"));

        Assert.False(result.IsValid);
        Assert.Equal("duplicate", result.Reason);
        Assert.Equal(existing.EntityId, result.ConflictEntityId);
    }

    [Fact]
    public void UrlPathFieldWithoutMetadata_StillValidatesAsUnique()
    {
        using var ctx = CreateContext();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, uniqueSetting: null, inputType: "string-url-path", includeUniqueMetadata: false));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var result = Validate(ctx, [type], [existing], new(type.NameId, "Slug", "same-slug"));

        Assert.False(result.IsValid);
        Assert.Equal("duplicate", result.Reason);
    }

    [Fact]
    public void DefaultLanguageWildcardRequest_IsHandledAsInvariant()
    {
        using var ctx = CreateContext();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var result = Validate(ctx, [type], [existing], new(type.NameId, "Slug", "same-slug", UniqueValueValidationRules.DefaultLanguageWildcard));

        Assert.False(result.IsValid);
        Assert.Equal("duplicate", result.Reason);
        Assert.Equal(existing.EntityId, result.ConflictEntityId);
    }

    [Fact]
    public void PlainNonUniqueField_ReturnsNotApplicable()
    {
        using var ctx = CreateContext();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String));

        var result = Validate(ctx, [type], [], new(type.NameId, "Slug", "same-slug"));

        Assert.True(result.IsValid);
        Assert.Equal("not-applicable", result.Reason);
    }

    [Fact]
    public void MissingContentType_ReturnsTypeNotFound()
    {
        using var ctx = CreateContext();
        var result = Validate(ctx, [], [], new("Missing.Type", "Slug", "same-slug"));

        Assert.True(result.IsValid);
        Assert.Equal("type-not-found", result.Reason);
    }

    #endregion

    private static UniqueValueValidationResult Validate(
        IsUniqueValidatorTestContext ctx,
        IContentType[] contentTypes,
        IEntity[] existingEntities,
        UniqueValueValidationRequest request)
        => UniqueValueValidation.Validate(
            new TestContentTypes(contentTypes),
            ctx.CreateDataSource(existingEntities),
            ctx.CreateUniqueValueLookup(),
            request);

    private sealed class TestContentTypes(IContentType[] contentTypes) : IAppReadContentTypes
    {
        public IEnumerable<IContentType> ContentTypes => contentTypes;

        public IContentType? TryGetContentType(string name)
            => contentTypes.FirstOrDefault(type => type.Is(name));

        public IContentType GetContentType(string name)
            => TryGetContentType(name)
               ?? throw new ArgumentException($"Can't find content type with name '{name}'", nameof(name));
    }

    private IsUniqueValidatorTestContext CreateContext()
        => IsUniqueValidatorTestContext.Create(dataAssembler, ctAssemblyKit, dataSourcesService, dataSourceDependencies);
}
