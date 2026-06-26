using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.DataSource;
using ToSic.Eav.Services;
using Xunit.DependencyInjection;

namespace ToSic.Sxc.WebApi.Tests.SaveHelpers;

[Startup(typeof(StartupCoreDataSourcesAndTestData))]
public class UniqueValueLookupTests
(
    DataAssembler dataAssembler,
    ContentTypeAssembler contentTypeAssembler,
    IDataSourcesService dataSourcesService,
    DataSourceBase.Dependencies dataSourceDependencies)
{
    [Fact]
    public void IsUnique_FindsExistingScalarConflict()
    {
        using var ctx = CreateContext();
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
        using var ctx = CreateContext();
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
        using var ctx = CreateContext();
        var entityGuid = Guid.NewGuid();
        var type = ctx.CreateType("Article",
            ctx.CreateField("Title", ValueTypes.String, isTitle: true),
            ctx.CreateField("Slug", ValueTypes.String, isUnique: true));

        var current = ctx.CreateEntity(type, entityGuid,
            ctx.InvariantAttribute("Title", ValueTypes.String, "Current"),
            ctx.InvariantAttribute("Slug", ValueTypes.String, "same-slug"));

        var conflict = ctx.CreateUniqueValueLookup().FindConflict(
            ctx.CreateDataSource(current),
            new("Article", "Slug", ValueTypes.String, "same-slug", CurrentEntity: current)
        );

        Assert.Null(conflict);
    }

    [Fact]
    public void IsUnique_ReportsOtherEntityWhenCurrentEntityAlsoMatches()
    {
        using var ctx = CreateContext();
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
            new("Article", "Slug", ValueTypes.String, "same-slug", CurrentEntity: current)
        );

        Assert.Same(duplicate, conflict);
    }

    private IsUniqueValidatorTestContext CreateContext()
        => IsUniqueValidatorTestContext.Create(dataAssembler, contentTypeAssembler, dataSourcesService, dataSourceDependencies);
}
