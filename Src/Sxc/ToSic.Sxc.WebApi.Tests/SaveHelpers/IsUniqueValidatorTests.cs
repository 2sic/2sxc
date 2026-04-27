using ToSic.Eav.Data;
using ToSic.Sxc.Backend.SaveHelpers;

namespace ToSic.Sxc.WebApi.Tests.SaveHelpers;

public class IsUniqueValidatorTests
{
    #region Theory Data

    public static TheoryData<ValueTypes, object> UniqueScalarValues => new()
    {
        { ValueTypes.Number, 42m },
        { ValueTypes.Boolean, true },
        { ValueTypes.DateTime, new DateTime(2024, 2, 3, 4, 5, 6, DateTimeKind.Utc) }
    };

    public static TheoryData<ValueTypes, object> UnsupportedUniqueFieldValues => new()
    {
        { ValueTypes.Empty, "same-marker" },
        { ValueTypes.Object, new { Marker = "same-marker" } },
        { ValueTypes.Json, "{\"marker\":\"same-marker\"}" },
        { ValueTypes.Undefined, "same-marker" }
    };

    public static TheoryData<string> UniqueEntityValues => new()
    {
        { "11111111-1111-1111-1111-111111111111" },
        { "11111111-1111-1111-1111-111111111111,22222222-2222-2222-2222-222222222222" }
    };

    #endregion

    #region Existing Entity Conflicts

    [Fact]
    public void DuplicateExistingEntityValue_BlocksSave()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var pending = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.NotNull(exception);
        Assert.Contains("Article.Slug", exception.Value);
        Assert.Contains("saved entity", exception.Value);
    }

    [Fact]
    public void ExistingMatchInNonFirstRawValue_BlocksSave()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttributeValues("Slug", ValueTypes.String, "other-slug", "same-slug"));

        var pending = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.NotNull(exception);
        Assert.Contains("Article.Slug", exception.Value);
    }

    [Fact]
    public void UnrelatedContentTypesWithSameUniqueFieldAndValue_DoNotBlockSave()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var articleType = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));
        var productType = ctx.CreateType("Product",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var unrelatedExisting = Enumerable.Range(0, 100)
            .Select(index => ctx.CreateEntity(productType, Guid.NewGuid(),
                ctx.InvariantAttribute("Title", ValueTypes.String, $"Product {index}"),
                ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug")))
            .ToList();

        var pending = ctx.CreateEntity(articleType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac(unrelatedExisting, [pending]);

        Assert.Null(exception);
    }

    [Fact]
    public void UpdatingSameEntityWithSameUniqueValue_Passes()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var entityGuid = Guid.NewGuid();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var existing = ctx.CreateEntity(type, entityGuid,
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var pending = ctx.CreateEntity(type, entityGuid,
            ctx.InvariantAttribute("Title", ValueTypes.String, "Updated"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.Null(exception);
    }

    [Fact]
    public void UpdatingFirstExistingDuplicateWithSameUniqueValue_BlocksSave()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var firstGuid = Guid.NewGuid();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var firstExisting = ctx.CreateEntity(type, firstGuid,
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing 1"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var secondExisting = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing 2"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var thirdExisting = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing 3"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var fourthExisting = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing 4"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var pendingFirst = ctx.CreateEntity(type, firstGuid,
            ctx.InvariantAttribute("Title", ValueTypes.String, "Updated 1"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac(
            [firstExisting, secondExisting, thirdExisting, fourthExisting],
            [pendingFirst]);

        Assert.NotNull(exception);
        Assert.Contains("Article.Slug", exception.Value);
        Assert.Contains("saved entity", exception.Value);
        Assert.Contains(secondExisting.EntityId.ToString(), exception.Value);
    }

    #endregion

    #region Same Request Conflicts

    [Fact]
    public void TwoItemsInSameSavePackageWithSameUniqueValue_BlockSave()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var first = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "First"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var second = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Second"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([], [first, second]);

        Assert.NotNull(exception);
        Assert.Contains("same request", exception.Value);
    }

    [Fact]
    public void UpdatedEntityAndAnotherPendingDuplicate_PreferSameRequestConflict()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var entityGuid = Guid.NewGuid();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var existing = ctx.CreateEntity(type, entityGuid,
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var updated = ctx.CreateEntity(type, entityGuid,
            ctx.InvariantAttribute("Title", ValueTypes.String, "Updated"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var second = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Second"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [updated, second]);

        Assert.NotNull(exception);
        Assert.Contains("same request", exception.Value);
        Assert.DoesNotContain("saved entity", exception.Value);
        Assert.Equal(1, exception.Value.Split(["Article.Slug"], StringSplitOptions.None).Length - 1);
    }

    #endregion

    #region Localization And Normalization

    [Fact]
    public void TranslatedValuesOnlyConflictInsideSameLanguageBucket()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.LocalizedAttribute("Slug", ValueTypes.String, ("same-slug", "en-us")));

        var pendingOtherLanguage = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Other Lang"),
            ctx.LocalizedAttribute("Slug", ValueTypes.String, ("same-slug", "de-de")));

        var pendingSameLanguage = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Same Lang"),
            ctx.LocalizedAttribute("Slug", ValueTypes.String, ("same-slug", "en-us")));

        var noConflict = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pendingOtherLanguage]);
        var conflict = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pendingSameLanguage]);

        Assert.Null(noConflict);
        Assert.NotNull(conflict);
        Assert.Contains("language: en-us", conflict.Value);
    }

    [Fact]
    public void InvariantAndTranslatedValuesWithSameText_DoNotConflictAcrossLanguageBuckets()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var pending = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.LocalizedAttribute("Slug", ValueTypes.String, ("same-slug", "en-us")));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.Null(exception);
    }

    [Fact]
    public void StringDuplicatesDifferingByCaseAndWhitespace_BlockSave()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "  Same-Slug  "));

        var pending = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.NotNull(exception);
    }

    [Fact]
    public void BlankStringsDoNotBlockSave()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "   "));

        var pending = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, ""));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.Null(exception);
    }

    [Fact]
    public void PlainStringWithoutIsUniqueMetadata_UsesDefaultFalse()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var pending = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.Null(exception);
    }

    [Fact]
    public void StringUrlWithNullIsUniqueMetadata_UsesDefaultTrue()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, uniqueSetting: null, inputType: "string-url-path"));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var pending = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.NotNull(exception);
        Assert.Contains("Article.Slug", exception.Value);
    }

    [Fact]
    public void StringUrlWithExplicitFalseIsUniqueMetadata_OverridesDefaultTrue()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, uniqueSetting: false, inputType: "string-url-path"));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var pending = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.Null(exception);
    }

    [Fact]
    public void StringUrlWithOtherInputTypeAndNullIsUniqueMetadata_UsesDefaultFalse()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, uniqueSetting: null, inputType: "string-url-other"));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var pending = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.Null(exception);
    }

    #endregion

    #region Type Specific Handling

    [Theory]
    [MemberData(nameof(UniqueScalarValues))]
    public void ScalarNonStringDuplicates_BlockSave(ValueTypes type, object value)
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var contentType = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("UniqueValue", type, isUnique: true));

        var existing = ctx.CreateEntity(contentType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("UniqueValue", type, value));

        var pending = ctx.CreateEntity(contentType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("UniqueValue", type, value));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.NotNull(exception);
        Assert.Contains("Article.UniqueValue", exception.Value);
    }

    [Theory]
    [MemberData(nameof(UniqueEntityValues))]
    public void EntityDuplicates_BlockSave(string value)
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var contentType = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Related", ValueTypes.Entity, isUnique: true));

        var existing = ctx.CreateEntity(contentType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Related", ValueTypes.Entity, value));

        var pending = ctx.CreateEntity(contentType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Related", ValueTypes.Entity, value));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.NotNull(exception);
        Assert.Contains("Article.Related", exception.Value);
        Assert.Contains($"value '{value.Split(',')[0]}'", exception.Value);
        Assert.Contains("language: invariant", exception.Value);
        Assert.DoesNotContain("[invariant]:", exception.Value);
    }

    [Fact]
    public void EntityDuplicates_BlockSave_WhenExistingUsesRepoIdsAndPendingUsesGuids()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var relatedGuid = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var relatedType = ctx.CreateType("Category",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true));
        var relatedEntity = ctx.CreateEntity(relatedType, relatedGuid,
            ctx.InvariantAttribute("Title", ValueTypes.String, "Shared Child"));

        var contentType = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Related", ValueTypes.Entity, isUnique: true));

        var existing = ctx.CreateEntity(contentType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.EntityRelationshipAttributeUsingRepoIds("Related", relatedEntity));

        var pending = ctx.CreateEntity(contentType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Related", ValueTypes.Entity, relatedGuid.ToString()));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.NotNull(exception);
        Assert.Contains("Article.Related", exception.Value);
        Assert.Contains("value 'Shared Child'", exception.Value);
        Assert.DoesNotContain($"value '{relatedGuid}'", exception.Value);
    }

    [Fact]
    public void EntityDuplicates_BlockSave_WhenRelationshipListsShareAChild()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var sharedGuid = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var secondGuid = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var thirdGuid = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var contentType = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Related", ValueTypes.Entity, isUnique: true));

        var existing = ctx.CreateEntity(contentType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Related", ValueTypes.Entity, $"{sharedGuid},{secondGuid}"));

        var pending = ctx.CreateEntity(contentType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Related", ValueTypes.Entity, $"{sharedGuid},{thirdGuid}"));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.NotNull(exception);
        Assert.Contains("Article.Related", exception.Value);
        Assert.Contains(sharedGuid.ToString(), exception.Value);
    }

    [Fact]
    public void EmptyEntityRelationshipsDoNotBlockSave()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var contentType = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Related", ValueTypes.Entity, isUnique: true));

        var existing = ctx.CreateEntity(contentType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Related", ValueTypes.Entity, null));

        var pending = ctx.CreateEntity(contentType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Related", ValueTypes.Entity, null));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.Null(exception);
    }

    [Theory]
    [MemberData(nameof(UnsupportedUniqueFieldValues))]
    public void UnsupportedFieldTypesAreIgnoredEvenIfMarkedUnique(ValueTypes fieldType, object value)
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var contentType = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Marker", fieldType, isUnique: true));

        var existing = ctx.CreateEntity(contentType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Marker", fieldType, value));

        var pending = ctx.CreateEntity(contentType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Pending"),
            ctx.InvariantAttribute("Marker", fieldType, value));

        var exception = ctx.CreateValidator().UniqueValuesOnlyTac([existing], [pending]);

        Assert.Null(exception);
    }

    #endregion
}
