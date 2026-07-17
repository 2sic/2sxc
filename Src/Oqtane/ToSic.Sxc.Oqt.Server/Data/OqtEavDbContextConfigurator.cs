#nullable enable

using Microsoft.EntityFrameworkCore;
using Oqtane.Databases.Interfaces;
using Oqtane.Extensions;
using Oqtane.Infrastructure;
using ToSic.Eav.Persistence.Efc.Sys.DbContext;

namespace ToSic.Sxc.Oqt.Server.Data;

internal sealed class OqtEavDbContextConfigurator(ITenantManager tenantManager) : IEavDbContextConfigurator
{
    public void Configure(DbContextOptionsBuilder optionsBuilder, string connectionString)
        => optionsBuilder.UseOqtaneDatabase(Database, connectionString);

    public string RewriteName(string name) => Database.RewriteName(name);

    private IDatabase Database => _database ??= GetDatabase(tenantManager.GetTenant()?.DBType);
    private IDatabase? _database;

    internal static IDatabase GetDatabase(string? databaseType)
    {
        var type = Type.GetType(databaseType ?? "")
            ?? throw new InvalidOperationException($"Unable to resolve Oqtane database type '{databaseType}'.");

        return Activator.CreateInstance(type) as IDatabase
            ?? throw new InvalidOperationException($"Oqtane database type '{databaseType}' does not implement {nameof(IDatabase)}.");
    }
}
