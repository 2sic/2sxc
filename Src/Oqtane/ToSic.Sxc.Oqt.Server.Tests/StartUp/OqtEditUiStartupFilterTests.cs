using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.StartUp;

namespace ToSic.Sxc.Oqt.StartUp;

public class OqtEditUiStartupFilterTests
{
    private const string EditorPath = "/Modules/ToSic.Sxc.Oqtane/dist/ng-edit/";
    private const string EditorQuery = "?pageId=100&tenantId=7&aliasId=42";
    private const int EditorSiteId = 79;

    [Fact]
    public void AddSxcOqtane_KeepsOtherStartupFilters_WhenRegisteringEditorFilter()
    {
        var services = new ServiceCollection()
            .AddTransient<IStartupFilter, FakeStartupFilter>()
            .AddSxcOqtane();

        Equal([typeof(FakeStartupFilter), typeof(OqtEditUiStartupFilter)],
            services.Where(s => s.ServiceType == typeof(IStartupFilter)).Select(s => s.ImplementationType));
    }

    [Theory]
    [InlineData(EditorPath, EditorQuery, EditorSiteId)]
    [InlineData("/modules/tosic.sxc.oqtane/dist/ng-edit", EditorQuery, EditorSiteId)]
    [InlineData("/Modules/ToSic.Sxc.Oqtane/dist/quick-dialog/", EditorQuery, EditorSiteId)]
    [InlineData(EditorPath, "?tenantId=7&aliasId=1", 1)]
    [InlineData(EditorPath, "?tenantId=8&aliasId=42", 1)]
    [InlineData(EditorPath, "?tenantId=7&aliasId=999", 1)]
    [InlineData(EditorPath, "?tenantId=7&aliasId=invalid", 1)]
    [InlineData(EditorPath, "?tenantId=invalid&aliasId=42", 1)]
    [InlineData(EditorPath, "?tenantId=7&aliasId=42&aliasId=1", 1)]
    [InlineData(EditorPath, "?aliasId=42", 1)]
    [InlineData(EditorPath, "?tenantId=7", 1)]
    [InlineData(EditorPath, "", 1)]
    [InlineData("/Modules/ToSic.Sxc.Oqtane/dist/ng-edit/main.js", EditorQuery, 1)]
    [InlineData("/Modules/ToSic.Sxc.Oqtane/dist/ng-edit/other", EditorQuery, 1)]
    [InlineData("/t79/api/sxc/cms/edit/load", EditorQuery, 1)]
    [InlineData("/t79", EditorQuery, 1)]
    public async Task Configure_SelectsSiteBeforeNextMiddleware_OnlyForValidEditorRequest(string path, string query, int expectedSiteId)
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();
        var context = CreateContext(scope.ServiceProvider, path, query);
        var app = new ApplicationBuilder(provider);
        var observedSiteId = 0;
        provider.GetRequiredService<IStartupFilter>().Configure(builder => builder.Run(http =>
        {
            var alias = http.RequestServices.GetRequiredService<ITenantManager>().GetAlias();
            // Oqtane's tenant middleware owns this key; an early duplicate would throw here.
            http.Items.Add(Constants.HttpContextAliasKey, alias);
            observedSiteId = alias.SiteId;
            return Task.CompletedTask;
        }))(app);

        await app.Build()(context);

        Equal(expectedSiteId, observedSiteId);
        False(context.User.Identity?.IsAuthenticated == true);
    }

    [Theory]
    [InlineData(false, StatusCodes.Status403Forbidden)]
    [InlineData(true, StatusCodes.Status200OK)]
    public async Task Configure_PreservesModuleEditorLogin_WhenAliasIsResolvedBeforeCookieValidation(bool withFilter, int expectedStatus)
    {
        using var provider = CreateServices();
        var cookie = await SignIn(provider, "1editor");
        using var scope = provider.CreateScope();
        var context = CreateContext(scope.ServiceProvider, EditorPath, EditorQuery);
        context.Request.Headers.Cookie = cookie;

        await CreateAuthenticatedPipeline(provider, withFilter)(context);

        Equal(expectedStatus, context.Response.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("no-site-membership")]
    public async Task Configure_DoesNotGrantAccess_WhenUserIsAnonymousOrNotASiteMember(string? username)
    {
        using var provider = CreateServices();
        using var scope = provider.CreateScope();
        var context = CreateContext(scope.ServiceProvider, EditorPath, EditorQuery);
        if (username != null)
            context.Request.Headers.Cookie = await SignIn(provider, username);

        await CreateAuthenticatedPipeline(provider, withFilter: true)(context);

        Equal(StatusCodes.Status403Forbidden, context.Response.StatusCode);
    }

    private static ServiceProvider CreateServices()
    {
        IServiceCollection services = new ServiceCollection();
        foreach (var registration in new ServiceCollection().AddSxcOqtane().Where(s => s.ServiceType == typeof(IStartupFilter)))
            services.Add(registration);
        services.AddScoped<ITenantManager, FakeTenantManager>();
        services.AddSingleton<IAliasRepository, FakeAliasRepository>();
        services.AddLogging();
        services.AddSingleton<IDataProtectionProvider>(new EphemeralDataProtectionProvider());
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options => options.Events.OnValidatePrincipal = context =>
            {
                // Model Oqtane's per-site cookie validation with the real ASP.NET Core cookie handler.
                var alias = context.HttpContext.RequestServices.GetRequiredService<ITenantManager>().GetAlias();
                if (alias.SiteId != EditorSiteId || context.Principal?.Identity?.Name != "1editor")
                    context.RejectPrincipal();
                return Task.CompletedTask;
            });
        return services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
    }

    private static RequestDelegate CreateAuthenticatedPipeline(IServiceProvider provider, bool withFilter)
    {
        var app = new ApplicationBuilder(provider);
        Action<IApplicationBuilder> configure = builder =>
        {
            builder.UseAuthentication();
            builder.Run(context =>
            {
                context.Response.StatusCode = context.User.Identity?.IsAuthenticated == true
                    ? StatusCodes.Status200OK
                    : StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            });
        };
        if (withFilter)
            configure = provider.GetRequiredService<IStartupFilter>().Configure(configure);
        configure(app);
        return app.Build();
    }

    private static async Task<string> SignIn(IServiceProvider provider, string username)
    {
        using var scope = provider.CreateScope();
        var context = CreateContext(scope.ServiceProvider, "/t79/login", "");
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, "Registered Users")],
            CookieAuthenticationDefaults.AuthenticationScheme));

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return context.Response.Headers.SetCookie.Single()!.Split(';')[0];
    }

    private static DefaultHttpContext CreateContext(IServiceProvider services, string path, string query)
    {
        var context = new DefaultHttpContext { RequestServices = services };
        context.Request.Scheme = "http";
        context.Request.Host = new HostString("localhost");
        context.Request.Path = path;
        context.Request.QueryString = new QueryString(query);
        return context;
    }

    private sealed class FakeStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => next;
    }

    private sealed class FakeTenantManager : ITenantManager
    {
        private Alias _alias = new() { AliasId = 1, TenantId = 7, SiteId = 1, Name = "localhost" };
        public Alias GetAlias() => _alias;
        public void SetAlias(Alias alias) => _alias = alias;
        public Tenant GetTenant() => throw new NotSupportedException();
        public void SetAlias(int tenantId, int siteId) => throw new NotSupportedException();
        public void SetTenant(int tenantId) => throw new NotSupportedException();
    }

    private sealed class FakeAliasRepository : IAliasRepository
    {
        public Alias GetAlias(int aliasId, bool tracking)
            => aliasId switch
            {
                1 => new Alias { AliasId = 1, TenantId = 7, SiteId = 1, Name = "localhost" },
                42 => new Alias { AliasId = 42, TenantId = 7, SiteId = EditorSiteId, Name = "localhost/t79" },
                _ => null!,
            };

        public Alias GetAlias(int aliasId) => GetAlias(aliasId, tracking: true);
        public IEnumerable<Alias> GetAliases() => throw new NotSupportedException();
        public Alias GetAlias(string url) => throw new NotSupportedException();
        public Alias GetAlias(int tenantId, int siteId) => throw new NotSupportedException();
        public Alias AddAlias(Alias alias) => throw new NotSupportedException();
        public Alias UpdateAlias(Alias alias) => throw new NotSupportedException();
        public void DeleteAlias(int aliasId) => throw new NotSupportedException();
    }
}
