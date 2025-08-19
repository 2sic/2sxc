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
using File = System.IO.File;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ToSic.Sxc.Oqt.Server.Installation;

/// <summary>
/// Handles install/upgrade steps for 2sxc on Oqtane.
/// </summary>
/// <remarks>
/// WARNING: Careful when renaming / moving, the name is listed in the ModuleInfo.cs in the Client.
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcManager(
    ISqlRepository sql,
    IServiceScopeFactory serviceScopeFactory,
    IWebHostEnvironment environment,
    IConfigManager configManager,
    ILogger<SxcManager> filelogger) : IInstallable
{
    private const string CleanInstallMigrationId = "ToSic.Sxc.Install";
    private const string MigrationPrefix = "ToSic.Sxc.";
    private const string MigrationTable = "__EFMigrationsHistory";

    public bool Install(Tenant tenant, string version)
    {
        LogInfo($"2sxc {EavSystemInfo.VersionString} install: Starting installation for version {version} on tenant '{tenant.Name}'");

        // 1) Clean install
        if (IsCleanInstall(tenant))
        {
            LogInfo($"2sxc {EavSystemInfo.VersionString} install: Clean install detected for tenant '{tenant.Name}'. Running '{CleanInstallMigrationId}.sql'.");
            if (!RunCleanInstall(tenant))
            {
                LogError($"2sxc {EavSystemInfo.VersionString} install ERROR: Clean install script '{CleanInstallMigrationId}.sql' failed for tenant '{tenant.Name}'.");
                return false;
            }

            LogInfo($"2sxc {EavSystemInfo.VersionString} install: Clean install completed successfully for tenant '{tenant.Name}'.");
            return true;
        }

        // 2) Version-specific upgrade hooks (non-SQL changes)
        ApplyVersionUpgrade(tenant, version);

        // 3) SQL migration per version (.sql embedded resource)
        var migrationId = BuildMigrationId(version);
        return ApplyMigrationIfNeeded(tenant, migrationId);
    }

    public bool Uninstall(Tenant tenant)
        => sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc.Uninstall.sql");

    private void ApplyVersionUpgrade(Tenant tenant, string version)
    {
        using var scope = serviceScopeFactory.CreateScope();

        // Set the current tenant context for the upgrade
        scope.ServiceProvider.GetRequiredService<ITenantManager>()
            .SetTenant(tenant.TenantId);

        switch (version.Replace(".", "-"))
        {
            case "20-00-00":
                Upgrade_20_00_00(tenant, scope, version);
                break;
        }
    }

    private void Upgrade_20_00_00(Tenant tenant, IServiceScope scope, string version)
    {
        LogInfo($"2sxc {EavSystemInfo.VersionString} install: {nameof(Upgrade_20_00_00)} {version}");

        string[] assemblies =
        [
            "ToSic.Sxc.dll",
            "ToSic.Eav.dll",
            "ToSic.Eav.Core.dll",
            "ToSic.Lib.Core.dll"
        ];

        RemoveAssemblies(tenant, assemblies, version);
    }

    private void RemoveAssemblies(Tenant tenant, string[] assemblies, string version)
    {
        LogInfo($"2sxc {EavSystemInfo.VersionString} install: {nameof(RemoveAssemblies)} assemblies:{assemblies.Length}, version:{version}");

        // In a development environment assemblies cannot be removed as the debugger runs from /bin and locks the files
        if (tenant.Name == TenantNames.Master && !environment.IsDevelopment())
        {
            foreach (var assembly in assemblies)
            {
                try
                {
                    var binFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    var filepath = Path.Combine(binFolder!, assembly);
                    LogInfo($"2sxc {EavSystemInfo.VersionString} install: {version} Removing {assembly} - '{filepath}'");
                    if (File.Exists(filepath)) File.Delete(filepath);
                }
                catch (Exception ex)
                {
                    LogError($"2sxc {EavSystemInfo.VersionString} install error: {version} Upgrade Error Removing {assembly} - {ex}");
                }
            }
        }
    }


    #region Migration helpers
    private static string BuildMigrationId(string version) => $"{MigrationPrefix}{version}";

    private static string CheckMigrationHistory(string migrationId)
        => $"SELECT 1 FROM {MigrationTable} WHERE MigrationId = '{migrationId}'";

    // We are using SQL scripts rather than EF migrations, so we seed the migration history table
    private static string RegisterInMigrationHistory(string migrationId)
        => $"""
            IF NOT EXISTS({CheckMigrationHistory(migrationId)})
            INSERT INTO __EFMigrationsHistory(MigrationId, ProductVersion, AppliedDate, AppliedVersion)
            VALUES('{migrationId}', '{EavSystemInfo.VersionString}', SYSDATETIME(), '{Constants.Version}')
            """;

    private bool ApplyMigrationIfNeeded(Tenant tenant, string migrationId)
    {
        var fileName = $"{migrationId}.sql";
        var script = sql.GetScriptFromAssembly(GetType().Assembly, fileName);

        if (string.IsNullOrEmpty(script))
        {
            LogWarn($"2sxc {EavSystemInfo.VersionString} install: SQL script '{migrationId}.sql' do not exists as embedded resource in assembly. Skipping.");
            return true;
        }
        
        if (Exists(sql, tenant, CheckMigrationHistory(migrationId)))
        {
            LogWarn($"2sxc {EavSystemInfo.VersionString} install: Migration for version '{migrationId}' already present in history. Skipping SQL script '{migrationId}.sql'.");
            return true;
        }

        try
        {
            sql.ExecuteScript(tenant, script);
            LogInfo($"2sxc {EavSystemInfo.VersionString} install: SQL script '{migrationId}.sql' executed successfully.");
        }
        catch
        {
            LogError($"2sxc {EavSystemInfo.VersionString} install ERROR: SQL script '{migrationId}.sql' failed.");
            return false;
        }

        var affected = sql.ExecuteNonQuery(tenant, RegisterInMigrationHistory(migrationId));
        if (affected <= 0)
        {
            LogWarn($"2sxc {EavSystemInfo.VersionString} install WARNING: Migration '{migrationId}' was already recorded in migration history.");
            return true;
        }

        LogInfo($"2sxc {EavSystemInfo.VersionString} install: Migration history updated for '{migrationId}'.");
        return true;
    }
    #endregion


    #region Existence checks
    // DB-agnostic existence check using a SELECT (never use ExecuteNonQuery for SELECTs - it returns -1)
    private static string HasSxcDb
        => """
            SELECT 1
            WHERE
                (
                    EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE 'TsDynData%' OR TABLE_NAME LIKE 'ToSIC_EAV_%')
                )
                OR
                (
                    EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory')
                    AND EXISTS (SELECT 1 FROM __EFMigrationsHistory WHERE MigrationId LIKE 'ToSic.Sxc.%')
                )
            """;

    private bool IsCleanInstall(Tenant tenant)
    {
        var exists = Exists(sql, tenant, HasSxcDb);
        LogInfo($"2sxc {EavSystemInfo.VersionString} install: Clean install status: {!exists}");
        return !exists;
    }

    private bool RunCleanInstall(Tenant tenant)
        => sql.ExecuteScript(tenant, GetType().Assembly, $"{CleanInstallMigrationId}.sql");

    private static bool Exists(ISqlRepository sql, Tenant tenant, string select)
    {
        using var reader = sql.ExecuteReader(tenant, select);
        return reader.Read();
    }
    #endregion


    #region Logging helpers
    private void LogInfo(string message) => filelogger.Log(LogLevel.Information, Utilities.LogMessage(this, message));
    private void LogWarn(string message) => filelogger.Log(LogLevel.Warning, Utilities.LogMessage(this, message));
    private void LogError(string message) => filelogger.LogError(Utilities.LogMessage(this, message)); 
    #endregion
}
