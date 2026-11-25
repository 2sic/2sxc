using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Oqtane.Database.SqlServer;
using Oqtane.Infrastructure;
using Oqtane.Models;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Server.Data;
using static Xunit.Assert;

namespace ToSic.Sxc.Oqt.Data;

public class OqtEavDbContextConfiguratorTests
{
    private const string DefaultConnection = "Server=(local);Database=DefaultTenant;Integrated Security=True";
    private const string SelectedConnection = "Server=(local);Database=SelectedTenant;Integrated Security=True";

    [Fact]
    public void Configure_UsesSelectedTenantConnection_ForBackgroundIndexing()
    {
        var manager = new FakeTenantManager(7, "Selected");
        var sut = new OqtEavDbContextConfigurator(manager, Context(manager, SelectedConnection));
        using var background = new OqtBackgroundTenantScope(7);

        var database = ConfiguredDatabase(sut, DefaultConnection);

        Equal("SelectedTenant", database);
    }

    [Fact]
    public void Configure_PreservesSuppliedConnection_OutsideBackgroundIndexing()
    {
        var manager = new FakeTenantManager(7, "Missing");
        var context = new OqtTenantContext(manager, new ConfigurationBuilder().Build());
        var sut = new OqtEavDbContextConfigurator(manager, context);

        Equal("DefaultTenant", ConfiguredDatabase(sut, DefaultConnection));
    }

    private static OqtTenantContext Context(ITenantManager manager, string connection)
        => new(manager, new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Selected"] = connection,
        }).Build());

    private static string ConfiguredDatabase(OqtEavDbContextConfigurator sut, string suppliedConnection)
    {
        var options = new DbContextOptionsBuilder();
        sut.Configure(options, suppliedConnection);
        using var db = new DbContext(options.Options);
        return new SqlConnectionStringBuilder(db.Database.GetConnectionString()).InitialCatalog;
    }

    private sealed class FakeTenantManager(int tenantId, string connection) : ITenantManager
    {
        public Alias GetAlias() => new() { AliasId = 1, TenantId = tenantId, SiteId = 1 };
        public Tenant GetTenant() => new()
        {
            TenantId = tenantId,
            DBConnectionString = connection,
            DBType = typeof(SqlServerDatabase).AssemblyQualifiedName,
        };
        public void SetAlias(Alias alias) => throw new NotSupportedException();
        public void SetAlias(int id, int siteId) => throw new NotSupportedException();
        public void SetTenant(int id) => throw new NotSupportedException();
    }
}
