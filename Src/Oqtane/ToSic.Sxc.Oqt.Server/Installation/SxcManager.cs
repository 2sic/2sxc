using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using System.Reflection;
using Microsoft.Extensions.Hosting;
using Oqtane.Shared;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace ToSic.Sxc.Oqt.Server.Installation;

/// <summary>
/// This is probably some kind of installer-class.
/// </summary>
/// <remarks>
/// WARNING: Careful when renaming / moving, the name is listed in the ModuleInfo.cs in the Client.
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcManager(ISqlRepository sql, IServiceScopeFactory serviceScopeFactory, IWebHostEnvironment environment, IConfigManager configManager, ILogger<UpgradeManager> filelogger) : IInstallable
{
    public bool Install(Tenant tenant, string version)
    {
        filelogger.Log(LogLevel.Information, Utilities.LogMessage(this, $"2sxc install: {nameof(Install)} {version}"));

        // core framework upgrade logic - executed for every tenant
        using (var scope = serviceScopeFactory.CreateScope())
        {
            // set tenant
            var tenantManager = scope.ServiceProvider.GetRequiredService<ITenantManager>();
            tenantManager.SetTenant(tenant.TenantId);

            switch (version.Replace(".", "-"))
            {
                case "20-00-00":
                    Upgrade_20_00_00(tenant, scope, version);
                    break;

            }
        }

        return sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc." + version + ".sql");
    }

    public bool Uninstall(Tenant tenant)
    {
        return sql.ExecuteScript(tenant, GetType().Assembly, "ToSic.Sxc.Uninstall.sql");
    }

    private void Upgrade_20_00_00(Tenant tenant, IServiceScope scope, string version)
    {
        filelogger.Log(LogLevel.Information, Utilities.LogMessage(this, $"2sxc install: {nameof(Upgrade_20_00_00)} {version}"));

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
        filelogger.Log(LogLevel.Information, Utilities.LogMessage(this, $"2sxc install: {nameof(RemoveAssemblies)} assemblies:{assemblies.Length}, version:{version}"));

        // in a development environment assemblies cannot be removed as the debugger runs from /bin folder and locks the files
        if (tenant.Name == TenantNames.Master && !environment.IsDevelopment())
        {
            foreach (var assembly in assemblies)
            {
                try
                {
                    var binFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    var filepath = Path.Combine(binFolder, assembly);
                    filelogger.Log(LogLevel.Information, Utilities.LogMessage(this, $"2sxc install: {version} Removing {assembly} - '{filepath}'"));
                    if (System.IO.File.Exists(filepath)) System.IO.File.Delete(filepath);
                }
                catch (Exception ex)
                {
                    // error deleting assembly
                    filelogger.LogError(Utilities.LogMessage(this, $"2sxc install Error: {version} Upgrade Error Removing {assembly} - {ex}"));
                }
            }
        }
    }
}