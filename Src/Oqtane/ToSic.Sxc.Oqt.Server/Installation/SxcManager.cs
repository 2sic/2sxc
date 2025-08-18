using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System.Reflection;
using ToSic.Sxc.Oqt.Shared.Models;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ToSic.Sxc.Oqt.Server.Installation;

/// <summary>
/// This is probably some kind of installer-class.
/// </summary>
/// <remarks>
/// WARNING: Careful when renaming / moving, the name is listed in the ModuleInfo.cs in the Client.
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcManager(ISqlRepository sql, IServiceScopeFactory serviceScopeFactory, IWebHostEnvironment environment, IConfigManager configManager, ILogger<SxcManager> filelogger) : IInstallable
{
    public bool Install(Tenant tenant, string version)
    {
        filelogger.Log(LogLevel.Information, Utilities.LogMessage(this, $"2sxc {EavSystemInfo.VersionString} install: Starting installation for version {version} on tenant '{tenant.Name}'"));

        // Core framework upgrade logic - executed for every tenant
        using (var scope = serviceScopeFactory.CreateScope())
        {
            // Set the current tenant context for the upgrade
            var tenantManager = scope.ServiceProvider.GetRequiredService<ITenantManager>();
            tenantManager.SetTenant(tenant.TenantId);

            // Run version-specific upgrade logic if needed
            switch (version.Replace(".", "-"))
            {
                case "20-00-00":
                    Upgrade_20_00_00(tenant, scope, version);
                    break;
            }
        }

        // Preflight check: ensure the SQL script only runs once per version by checking migration history
        var migrationId = $"ToSic.Sxc.{version}";
        var migrationSqlInHistory = sql.ExecuteNonQuery(tenant, CheckMigrationHistory(migrationId));
        if (migrationSqlInHistory > 0)
        {
            // Migration already applied: log and skip further upgrade for this version
            filelogger.Log(LogLevel.Information, Utilities.LogMessage(this,
                $"2sxc {EavSystemInfo.VersionString} install: Migration for version {version} already present in history. Skipping SQL script '{migrationId}.sql'."
            ));
            return true; // Migration already applied, nothing to do
        }

        // Execute the SQL script for the current version
        var migrationSqlExecutedWithoutError = sql.ExecuteScript(tenant, GetType().Assembly, $"{migrationId}.sql");
        if (!migrationSqlExecutedWithoutError)
        {
            filelogger.LogError(Utilities.LogMessage(this, $"2sxc {EavSystemInfo.VersionString} install ERROR: SQL script '{migrationId}.sql' failed for version {version}."));
            return false; // SQL script failed
        }
        filelogger.Log(LogLevel.Information, Utilities.LogMessage(this, $"2sxc {EavSystemInfo.VersionString} install: SQL script '{migrationId}.sql' executed successfully for version {version}."));

        // Register the migration in the migration history table
        var migrationHistoryAffected = sql.ExecuteNonQuery(tenant, RegisterInMigrationHistory(migrationId));
        if (migrationHistoryAffected <= 0)
        {
            // Migration was already recorded, log a warning
            filelogger.Log(LogLevel.Warning, Utilities.LogMessage(this,
                $"2sxc {EavSystemInfo.VersionString} install WARNING: Migration '{migrationId}' for version {version} was already recorded in migration history."
            ));
            return true;
        }
        filelogger.Log(LogLevel.Information, Utilities.LogMessage(this, $"2sxc {EavSystemInfo.VersionString} install: Migration history updated for '{migrationId}'."));

        return true; // Installation completed successfully

    }

    public bool Uninstall(Tenant tenant)
    {
        return sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc.Uninstall.sql");
    }

    private void Upgrade_20_00_00(Tenant tenant, IServiceScope scope, string version)
    {
        filelogger.Log(LogLevel.Information, Utilities.LogMessage(this, $"2sxc {EavSystemInfo.VersionString} install: {nameof(Upgrade_20_00_00)} {version}"));

        string[] assemblies = [
            "ToSic.Sxc.dll",
            "ToSic.Eav.dll",
            "ToSic.Eav.Core.dll",
            "ToSic.Lib.Core.dll"
        ];

        RemoveAssemblies(tenant, assemblies, version);
    }

    private void RemoveAssemblies(Tenant tenant, string[] assemblies, string version)
    {
        filelogger.Log(LogLevel.Information, Utilities.LogMessage(this, $"2sxc {EavSystemInfo.VersionString} install: {nameof(RemoveAssemblies)} assemblies:{assemblies.Length}, version:{version}"));

        // in a development environment assemblies cannot be removed as the debugger runs from /bin folder and locks the files
        if (tenant.Name == TenantNames.Master && !environment.IsDevelopment())
        {
            foreach (var assembly in assemblies)
            {
                try
                {
                    var binFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    var filepath = Path.Combine(binFolder, assembly);
                    filelogger.Log(LogLevel.Information, Utilities.LogMessage(this, $"2sxc {EavSystemInfo.VersionString} install: {version} Removing {assembly} - '{filepath}'"));
                    if (System.IO.File.Exists(filepath)) System.IO.File.Delete(filepath);
                }
                catch (Exception ex)
                {
                    // error deleting assembly
                    filelogger.LogError(Utilities.LogMessage(this, $"2sxc {EavSystemInfo.VersionString} install error: {version} Upgrade Error Removing {assembly} - {ex}"));
                }
            }
        }
    }

    private static string CheckMigrationHistory(string migrationId)
        => $"SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '{migrationId}'";

    // we are using SQL scripts rather than migrations, so we need to seed the migration history table
    private static string RegisterInMigrationHistory(string migrationId)
        => $"""
         IF NOT EXISTS(
             SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId = '{migrationId}'
         )
         INSERT INTO __EFMigrationsHistory(MigrationId, ProductVersion, AppliedDate, AppliedVersion)
         VALUES('{migrationId}', '{EavSystemInfo.VersionString}', SYSDATETIME(), '{Constants.Version}')
         """;
}
