using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Sxc.Oqt.Server.Configuration;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sys.Configuration;

namespace ToSic.Sxc.Oqt.Configuration;

public class OqtGlobalConfigurationTests
{
    #region ConnectionString Resolution
    [Fact]
    public void GetThis_ReturnsTenantConnectionString_WhenTenantContextAvailable()
    {
        // Arrange
        const string expected = "Data Source=.;Initial Catalog=TenantDb;Integrated Security=True;";

        var services = new ServiceCollection()
            .AddScoped<IOqtTenantContext>(_ => new FakeOqtTenantContext(
                new OqtTenantContextInfo(
                    TenantId: 7,
                    SiteId: 1,
                    ConnectionStringName: "TenantDb",
                    ConnectionString: expected
                )));

        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var httpContext = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
        var httpContextAccessor = new HttpContextAccessor { HttpContext = httpContext };

        var sut = new OqtGlobalConfiguration(httpContextAccessor);

        // Act
        var result = ((IGlobalConfiguration)sut).GetThis(nameof(GlobalConfigDb.ConnectionString));

        // Assert
        Equal(expected, result);
    }
    #endregion

    private sealed class FakeOqtTenantContext(OqtTenantContextInfo? context) : IOqtTenantContext
    {
        public OqtTenantContextInfo? Get() => context;

        public OqtTenantContextInfo GetRequired()
            => context ?? throw new InvalidOperationException("Test context not configured.");
    }
}
