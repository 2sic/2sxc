using System.Linq;
using Oqtane.Models;
using Oqtane.Modules;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.Content
{
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
         */

        /// <summary>
        /// The SQL versions must use a "-" to avoid being replaced on search/replace when releasing a new version
        /// </summary>
        internal static string[] SqlScriptVersions = { "0-0-1", "12-00-00", "12-02-01", "12-05-00" };

        /// <summary>
        /// Merge versions for use in Oqtane version list
        /// </summary>
        /// <returns></returns>
        internal static string GetVersionList()
        {
            var versionsWithDot = SqlScriptVersions
                .Select(v => v.Replace('-', '.'))
                .ToList();
            versionsWithDot.Add(Sxc.Settings.Installation.CurrentReleaseVersion);
            // remove duplicates in case the current version also has SQL scripts
            var versions = versionsWithDot.Distinct();
            return string.Join(',', versions);
        }

        internal static ModuleDefinition BuildModuleDefinition(string name, string description) => new()
        {
            Name = name,
            Description = description,
            Categories = "Common",
            Version = "12.11.00",
            Owner = "2sic Internet Solutions",
            Url = "https://2sxc.org",
            Contact = "@iJungleboy",
            License = "MIT",
            Dependencies = "ToSic.Sxc.Oqtane.Shared",
            // PermissionNames = "",
            ServerManagerType = "ToSic.Sxc.Oqt.Server.Manager.SxcManager, ToSic.Sxc.Oqtane.Server",
            // ControlTypeRoutes = "",
            // This must contain all versions with a SQL script and current/latest version
            // list versions with sql scripts in \ToSic.Sxc.Oqt.Server\Scripts\
            ReleaseVersions = GetVersionList(),
            // DefaultAction = "",
            // SettingsType = "",
            PackageName = "ToSic.Sxc",
            //Runtimes = "Server",
            Template = "", // "External" (not "internal") "Default Module Template"

        };

        public ModuleDefinition ModuleDefinition => BuildModuleDefinition("Content", "Text/Image layouts using structured content");
    }
}
