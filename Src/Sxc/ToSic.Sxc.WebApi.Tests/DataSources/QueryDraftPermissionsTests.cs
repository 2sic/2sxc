using Xunit;

namespace ToSic.Sxc.WebApi.Tests.DataSources;

public class QueryDraftPermissionsTests
{
    [Theory]
    [InlineData(false, true, 2)]
    [InlineData(true, false, 1)]
    public void PublishingFilter_UsesTargetAppPermissions_WhenAmbientPermissionsDiffer(bool ambient, bool target, int expected)
    {
        using var ctx = new QueryDraftPermissionsTestContext(ambient);
        var filter = ctx.Filter(ctx.Lookup(target));

        Assert.Equal(expected, filter.List.Count());
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void QueryFactory_UsesTargetAppPermissions_ForShowDraftsParameter(bool ambient, bool target)
    {
        using var ctx = new QueryDraftPermissionsTestContext(ambient);

        Assert.Equal(target, ctx.QueryParameter(ctx.Lookup(target)));
    }

    [Theory]
    [InlineData(false, true, 2)]
    [InlineData(true, false, 1)]
    public void Relationships_UseTargetAppPermissions_WhenAmbientPermissionsDiffer(bool ambient, bool target, int expected)
    {
        using var ctx = new QueryDraftPermissionsTestContext(ambient);

        Assert.Equal(expected, ctx.Relationships(ctx.Lookup(target)).List.Count());
    }

    [Theory]
    [InlineData(false, 1)]
    [InlineData(true, 2)]
    public void DraftReaders_KeepAmbientFallback_WithoutPreparedPermissions(bool ambient, int expected)
    {
        using var ctx = new QueryDraftPermissionsTestContext(ambient);
        var lookup = ctx.Lookup(null);

        Assert.Equal(expected, ctx.Filter(lookup).List.Count());
        Assert.Equal(ambient, ctx.QueryParameter(lookup));
    }

    [Theory]
    [InlineData(false, true, 2)]
    [InlineData(true, false, 1)]
    public void PublishingFilter_PreservesExplicitShowDrafts_OverQueryDefault(bool target, bool setting, int expected)
    {
        using var ctx = new QueryDraftPermissionsTestContext();

        Assert.Equal(expected, ctx.Filter(ctx.Lookup(target), setting).List.Count());
    }

    [Theory]
    [InlineData(1, 43, false)]
    [InlineData(1, 43, true)]
    [InlineData(2, 42, false)]
    [InlineData(2, 42, true)]
    public void DraftReaders_KeepExistingFallback_ForAnotherAppOrZone(int zoneId, int appId, bool ambient)
    {
        using var ctx = new QueryDraftPermissionsTestContext(ambient);
        var lookup = ctx.Lookup(!ambient, new AppIdentity(zoneId, appId));

        Assert.Equal(ambient ? 2 : 1, ctx.Filter(lookup).List.Count());
        Assert.Equal(ambient, ctx.QueryParameter(lookup));
    }

    [Theory]
    [InlineData(false, true, null, 2)]
    [InlineData(false, true, "draft", 2)]
    [InlineData(false, true, "live", 1)]
    [InlineData(true, false, null, 1)]
    [InlineData(false, false, "draft", 1)]
    public void UserPermissions_UsesSuppliedContextAndLivePreference_WithoutChangingAmbientCache(
        bool ambient, bool target, string? cookie, int expected)
    {
        using var ctx = new QueryDraftPermissionsTestContext(ambient, cookie);
        var service = ctx.CurrentContext;
        var cached = service.UserPermissions();

        var permissions = service.UserPermissions(QueryDraftPermissionsTestContext.Context(target));
        var lookup = ctx.Lookup(permissions.ShowDraftData);

        Assert.Equal(expected, ctx.Filter(lookup).List.Count());
        Assert.Equal(expected == 2, ctx.QueryParameter(lookup));
        Assert.Same(cached, service.UserPermissions());
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AppTypeStream_CacheSeparatesDraftDecisions(bool draftsFirst)
    {
        using var ctx = new QueryDraftPermissionsTestContext();

        Assert.Equal(draftsFirst ? 2 : 1, ctx.CachedTypeStream(ctx.Lookup(draftsFirst)).List.Count());
        Assert.Equal(draftsFirst ? 1 : 2, ctx.CachedTypeStream(ctx.Lookup(!draftsFirst)).List.Count());
    }

    [Fact]
    public void AppTypeStream_CacheUsesDraftDecision_ForAmbientFallbackToo()
    {
        using var ctx = new QueryDraftPermissionsTestContext(ambientShowDrafts: false, ambientContentAdmin: true);

        Assert.Equal(2, ctx.CachedTypeStream(ctx.Lookup(true)).List.Count());
        Assert.Single(ctx.CachedTypeStream(ctx.Lookup(null)).List);
    }

    [Fact]
    public void DraftReaders_KeepIndependentDecisions_ForInterleavedQueries()
    {
        using var ctx = new QueryDraftPermissionsTestContext();
        var withDrafts = ctx.Filter(ctx.Lookup(true));
        var withoutDrafts = ctx.Filter(ctx.Lookup(false));

        Assert.Single(withoutDrafts.List);
        Assert.Equal(2, withDrafts.List.Count());
    }
}
