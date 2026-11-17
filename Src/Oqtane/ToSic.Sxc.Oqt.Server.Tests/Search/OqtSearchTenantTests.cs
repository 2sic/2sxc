using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Plumbing;
using static Xunit.Assert;

namespace ToSic.Sxc.Oqt.Search;

/// <summary>Checks how request and background execution share query and cache-key services.</summary>
public class OqtSearchTenantTests
{
    [Fact]
    public void QueryStringParams_ReturnsEmpty_ForBackgroundIndexingWithoutNavigation()
    {
        // Leaving navigation uninitialized reproduces the background-only failure.
        var http = new HttpBlazor(new HttpContextAccessor(), new FakeNavigationManager(false));
        using var tenant = new OqtBackgroundTenantScope(7);

        Empty(http.QueryStringParams.AllKeys);
    }

    [Fact]
    public void QueryStringParams_PreservesInteractiveNavigation_WithoutHttpContext()
    {
        var http = new HttpBlazor(new HttpContextAccessor(), new FakeNavigationManager(true));

        Equal("42", http.QueryStringParams["id"]);
        Null(http.QueryStringParams["authmoduleid"]);
    }

    [Fact]
    public void AppCacheKey_UsesBackgroundTenant_ThenRestoresRequestTenant()
    {
        using var services = new ServiceCollection().AddSingleton<IOqtTenantContext>(new FakeTenantContext()).BuildServiceProvider();
        var accessor = new HttpContextAccessor { HttpContext = new DefaultHttpContext { RequestServices = services } };
        var keys = new OqtAppCacheKeyService(accessor);

        using (new OqtBackgroundTenantScope(7))
            Equal("t07-z002-a00003", keys.AppCacheKey(new AppIdentity(2, 3)));

        Equal("t99-z002-a00003", keys.AppCacheKey(new AppIdentity(2, 3)));
    }

    private sealed class FakeNavigationManager : NavigationManager
    {
        public FakeNavigationManager(bool initialize)
        {
            if (initialize)
                Initialize("http://localhost/", "http://localhost/?id=42&authmoduleid=7");
        }
        protected override void NavigateToCore(string uri, bool forceLoad) => throw new NotSupportedException();
    }

    private sealed class FakeTenantContext : IOqtTenantContext
    {
        public OqtTenantContextInfo? Get() => GetRequired();
        public OqtTenantContextInfo GetRequired() => new(99, 3, "unused", "unused");
    }
}
