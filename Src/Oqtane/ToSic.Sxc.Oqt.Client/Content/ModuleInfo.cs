using Oqtane.Models;
using Oqtane.Modules;
using System.Linq;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Models;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.Content;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ModuleInfo : IModule
{
    /*
     * History
     * These version numbers shouldn't be exactly like the normal ones, so we use a "-" instead of "." so it won't be replaced on search/replace
     *
     * List of 2sxc versions with special relevance to Oqtane
     * 00-00-01 - SQL - Oqtane db fix for https://github.com/oqtane/oqtane.framework/issues/1269 (probably we will remove later).
     * 12-00-00 - SQL - 2sxc oqtane module first release.
     * 12-01-00 -  -  - 2sxc oqtane module minor release.
     * 12-02-00 -  -  - Compatible with Oqtane 2.0.2
     * 12-02-01 - SQL - Oqtane 2.1.0
     * 12-04-00 -  -  - Compatible with Oqtane 2.2.0
     * 12-05-00 - SQL - Oqtane db fix for folders
     * 13-00-00 - SQL - update TargetTypes Metadata
     * 13-01-00 - SQL - add SysSettings column to Apps table + add TargetTypes Metadata
     * 15-00-00 - SQL - updates for DataTimeline table
     * ...
     * 16-07-01 - SQL - add Json for Attribute and ContentType configuration + Guid for Attribute
     * 18-02-01 - SQL - remove AttributeGroups SQL table and related
     */

    /// <summary>
    /// The SQL versions must use a "-" to avoid being replaced on search/replace when releasing a new version.
    /// When SQL script is added in new version, include new version explicitly in this array.
    /// </summary>
    internal static string[] SqlScriptVersions = ["0-0-1", "12-00-00", "12-02-01", "12-05-00", "13-00-00", "13-01-00", "15-00-00", "16-07-01", "18-03-00"];

    /// <summary>
    /// Merge versions for use in Oqtane version list
    /// </summary>
    /// <returns></returns>
    internal static string GetSqlAndLatestVersions()
    {
        var versionsWithDot = SqlScriptVersions
            .Select(v => v.Replace('-', '.'))
            .ToList();
        versionsWithDot.Add(EavSystemInfo.VersionString);
        // remove duplicates in case the current version also has SQL scripts
        var versions = versionsWithDot.Distinct();
        return string.Join(',', versions);
    }

    internal static ModuleDefinition BuildModuleDefinition(string name, string description) => new()
    {
        Name = name,
        Description = description,
        Categories = "Common",
        Version = EavSystemInfo.VersionString, // Must be duplicated here, so Oqtane Client doesn't depend on server DLLs
        Owner = "2sic Internet Solutions",
        Url = "https://2sxc.org",
        Contact = "@iJungleboy",
        License = "MIT",
        Dependencies = "ToSic.Sxc.Oqtane.Shared,ToSic.Lib.Core",
        // PermissionNames = "",
        ServerManagerType = "ToSic.Sxc.Oqt.Server.Installation.SxcManager, ToSic.Sxc.Oqtane.Server",
        // ControlTypeRoutes = "",
        // This must contain all versions with a SQL script and current/latest version
        // list versions with sql scripts in \ToSic.Sxc.Oqt.Server\Scripts\
        ReleaseVersions = GetSqlAndLatestVersions(),
        // DefaultAction = "",
        // SettingsType = "",
        PackageName = OqtConstants.PackageName, // "ToSic.Sxc.Oqtane"
        Runtimes = "", // string.Empty enables all runtimes
        Template = "", // "External" (not "internal") "Default Module Template"

    };

    public ModuleDefinition ModuleDefinition => BuildModuleDefinition("Content", "Text/Image layouts using structured content");
}