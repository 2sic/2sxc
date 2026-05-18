using ToSic.Eav.Data;
using ToSic.Sxc.Backend.SaveHelpers;

namespace ToSic.Sxc.WebApi.Tests.SaveHelpers;

public class UniqueValueLookupTests
{
    [Fact]
    public void IsUnique_FindsExistingScalarConflict()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var existing = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Existing"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var conflict = ctx.CreateUniqueValueLookup().FindConflict(
            ctx.CreateDataSource(existing),
            new("Article", "Slug", ValueTypes.String, "same-slug")
        );

        Assert.Same(existing, conflict);
    }

    [Fact]
    public void IsUnique_FiltersByContentType()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var articleType = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));
        var productType = ctx.CreateType("Product",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var unrelated = ctx.CreateEntity(productType, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Product"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var conflict = ctx.CreateUniqueValueLookup().FindConflict(
            ctx.CreateDataSource(unrelated),
            new("Article", "Slug", ValueTypes.String, "same-slug")
        );

        Assert.Null(conflict);
    }

    [Fact]
    public void IsUnique_ExcludesCurrentEntity()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var entityGuid = Guid.NewGuid();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var current = ctx.CreateEntity(type, entityGuid,
            ctx.InvariantAttribute("Title", ValueTypes.String, "Current"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var conflict = ctx.CreateUniqueValueLookup().FindConflict(
            ctx.CreateDataSource(current),
            new("Article", "Slug", ValueTypes.String, "same-slug", CurrentGuid: entityGuid)
        );

        Assert.Null(conflict);
    }

    [Fact]
    public void IsUnique_ReportsOtherEntityWhenCurrentEntityAlsoMatches()
    {
        using var ctx = IsUniqueValidatorTestContext.Create();
        var entityGuid = Guid.NewGuid();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var current = ctx.CreateEntity(type, entityGuid,
            ctx.InvariantAttribute("Title", ValueTypes.String, "Current"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));
        var duplicate = ctx.CreateEntity(type, Guid.NewGuid(),
            ctx.InvariantAttribute("Title", ValueTypes.String, "Duplicate"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var conflict = ctx.CreateUniqueValueLookup().FindConflict(
            ctx.CreateDataSource(current, duplicate),
            new("Article", "Slug", ValueTypes.String, "same-slug", CurrentGuid: entityGuid)
        );

        Assert.Same(duplicate, conflict);
    }
}
