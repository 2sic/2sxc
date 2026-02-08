using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System.Reflection;
using ToSic.Sxc.Oqt.Shared;
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
    //IConfigManager configManager,
    ILogger<SxcManager> logger) : IInstallable
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
                Upgrade_20_00_00(tenant, version);
                break;
            case "21-01-01":
                Upgrade_21_01_01(tenant, version);
                break;
        }
    }

    private void Upgrade_20_00_00(Tenant tenant, string version)
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

    private void Upgrade_21_01_01(Tenant tenant, string version)
    {
        LogInfo($"2sxc {EavSystemInfo.VersionString} install: {nameof(Upgrade_21_01_01)} {version}");

        if (tenant.Name != TenantNames.Master)
        {
            LogInfo($"2sxc {EavSystemInfo.VersionString} install: {nameof(Upgrade_21_01_01)} skipped because tenant '{tenant.Name}' is not '{TenantNames.Master}'.");
            return;
        }

        try
        {
            var appRoot = Path.Combine(environment.ContentRootPath, OqtConstants.AppRoot);
            var destinationBase = Path.Combine(appRoot, OqtConstants.TenantsFolderName, tenant.TenantId.ToString(), OqtConstants.SitesFolderName);

            MoveSubfoldersToDestinationBase(appRoot, destinationBase, version);
        }
        catch (Exception ex)
        {
            LogError($"2sxc {EavSystemInfo.VersionString} install error: {version} Upgrade Error moving 2sxc folders - {ex}");
        }

        string[] assemblies =
        [
            "Microsoft.AspNetCore.Authorization.dll"
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

    private void MoveSubfoldersToDestinationBase(string sourceRoot, string destinationBase, string version)
    {
        LogInfo($"2sxc {EavSystemInfo.VersionString} install: {nameof(MoveSubfoldersToDestinationBase)} source:'{sourceRoot}', dest:'{destinationBase}', version:{version}");

        if (!Directory.Exists(sourceRoot))
        {
            LogWarn($"2sxc {EavSystemInfo.VersionString} install: {version} Source folder not found, skipping: '{sourceRoot}'");
            return;
        }

        Directory.CreateDirectory(destinationBase);

        foreach (var directory in Directory.GetDirectories(sourceRoot))
        {
            var folderName = Path.GetFileName(directory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            
            if (string.IsNullOrWhiteSpace(folderName))
                continue;

            // Don't try to move the new structure folder back into itself.
            if (folderName.Equals(OqtConstants.TenantsFolderName, StringComparison.OrdinalIgnoreCase))
                continue;

            var target = Path.Combine(destinationBase, folderName);
            try
            {
                var success = MoveDirectoryWithRetry(source: directory, target: target, version: version);
                if (!success)
                    LogError($"2sxc {EavSystemInfo.VersionString} install error: {version} Could not move folder '{directory}' -> '{target}'. Manual intervention may be required.");
            }
            catch (Exception ex)
            {
                LogError($"2sxc {EavSystemInfo.VersionString} install error: {version} Unexpected error moving folder '{directory}' -> '{target}' - {ex}");
            }
        }
    }

    private bool MoveDirectoryWithRetry(string source, string target, string version)
    {
        const int maxAttempts = 10;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                MoveDirectoryMerged(source, target, version);
                return true;
            }
            catch (IOException ex) when (attempt < maxAttempts)
            {
                LogWarn($"2sxc {EavSystemInfo.VersionString} install: {version} Move attempt {attempt}/{maxAttempts} IO error, retrying '{source}' -> '{target}': {ex.Message}");
                Thread.Sleep(150 * attempt);
            }
            catch (UnauthorizedAccessException ex) when (attempt < maxAttempts)
            {
                LogWarn($"2sxc {EavSystemInfo.VersionString} install: {version} Move attempt {attempt}/{maxAttempts} access error, retrying '{source}' -> '{target}': {ex.Message}");
                Thread.Sleep(150 * attempt);
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                LogWarn($"2sxc {EavSystemInfo.VersionString} install: {version} Move attempt {attempt}/{maxAttempts} error, retrying '{source}' -> '{target}': {ex.Message}");
                Thread.Sleep(150 * attempt);
            }
        }

        // Final attempt failed - don't throw, so we can continue with other folders.
        try
        {
            MoveDirectoryMerged(source, target, version);
            return true;
        }
        catch (Exception ex)
        {
            LogError($"2sxc {EavSystemInfo.VersionString} install error: {version} Failed moving folder '{source}' -> '{target}' after {maxAttempts} attempts - {ex.Message}");
            return false;
        }
    }

    private void MoveDirectoryMerged(string source, string target, string version)
    {
        if (!Directory.Exists(source))
            return;

        if (!Directory.Exists(target))
        {
            try
            {
                LogInfo($"2sxc {EavSystemInfo.VersionString} install: {version} Moving folder '{source}' -> '{target}'");
                Directory.Move(source, target);
                return;
            }
            catch (IOException ex)
            {
                LogWarn($"2sxc {EavSystemInfo.VersionString} install: {version} Could not move folder directly, will merge instead '{source}' -> '{target}': {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                LogWarn($"2sxc {EavSystemInfo.VersionString} install: {version} Could not move folder directly due to access, will merge instead '{source}' -> '{target}': {ex.Message}");
            }

            Directory.CreateDirectory(target);
        }

        LogInfo($"2sxc {EavSystemInfo.VersionString} install: {version} Merging folder '{source}' -> '{target}'");

        foreach (var subDir in Directory.GetDirectories(source))
        {
            var name = Path.GetFileName(subDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (string.IsNullOrWhiteSpace(name)) continue;

            var success = MoveDirectoryWithRetry(subDir, Path.Combine(target, name), version);
            if (!success)
                LogError($"2sxc {EavSystemInfo.VersionString} install error: {version} Could not move subfolder '{subDir}' into '{target}'.");
        }

        foreach (var file in Directory.GetFiles(source))
        {
            var name = Path.GetFileName(file);
            if (string.IsNullOrWhiteSpace(name)) continue;

            var destinationFile = Path.Combine(target, name);
            if (File.Exists(destinationFile))
            {
                LogWarn($"2sxc {EavSystemInfo.VersionString} install: {version} Skipping file because destination already exists: '{destinationFile}'");
                continue;
            }

            MoveFileWithRetry(file, destinationFile, version);
        }

        // Try clean up the now-empty folder. Ignore errors in case something is still locked.
        try
        {
            if (Directory.GetFileSystemEntries(source).Length == 0)
            {
                Directory.Delete(source, recursive: false);
                LogInfo($"2sxc {EavSystemInfo.VersionString} install: {version} Removed empty folder '{source}'");
            }
        }
        catch (Exception ex)
        {
            LogWarn($"2sxc {EavSystemInfo.VersionString} install: {version} Could not delete folder '{source}': {ex.Message}");
        }
    }

    private void MoveFileWithRetry(string sourceFile, string targetFile, string version)
    {
        const int maxAttempts = 10;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                LogInfo($"2sxc {EavSystemInfo.VersionString} install: {version} Moving file '{sourceFile}' -> '{targetFile}'");
                File.Move(sourceFile, targetFile);
                return;
            }
            catch (IOException ex) when (attempt < maxAttempts)
            {
                LogWarn($"2sxc {EavSystemInfo.VersionString} install: {version} File move attempt {attempt}/{maxAttempts} IO error, retrying '{sourceFile}' -> '{targetFile}': {ex.Message}");
                Thread.Sleep(150 * attempt);
            }
            catch (UnauthorizedAccessException ex) when (attempt < maxAttempts)
            {
                LogWarn($"2sxc {EavSystemInfo.VersionString} install: {version} File move attempt {attempt}/{maxAttempts} access error, retrying '{sourceFile}' -> '{targetFile}': {ex.Message}");
                Thread.Sleep(150 * attempt);
            }
        }

        LogError($"2sxc {EavSystemInfo.VersionString} install error: {version} Failed moving file '{sourceFile}' -> '{targetFile}' after {maxAttempts} attempts.");
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
    private void LogInfo(string message) => logger.Log(LogLevel.Information, Utilities.LogMessage(this, message));
    private void LogWarn(string message) => logger.Log(LogLevel.Warning, Utilities.LogMessage(this, message));
    private void LogError(string message) => logger.LogError(Utilities.LogMessage(this, message)); 
    #endregion
}
