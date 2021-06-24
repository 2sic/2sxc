using Oqtane.Models;
using Oqtane.Modules;

namespace ToSic.Sxc.Oqt.Content
{
    public class ModuleInfo : IModule
    {
        /*
         * History
         * These version numbers shouldn't be exactly like the normal ones, so we use a "-" instead of "." so it won't be replaced on search/replace
         * 00-00-01 - SQL - Oqtane db fix for https://github.com/oqtane/oqtane.framework/issues/1269 (probably we will remove latter).
         * 12-00-00 - SQL - 2sxc oqtane module first release.
         * 12-01-00 -  -  - 2sxc oqtane module minor release.
         * 12-02-00 -  -  - Nothing yet
         */

        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Content",
            Description = "2sxc helps create designed, cross-platform content",
            Version = "12.03.00",
            ServerManagerType = "ToSic.Sxc.Oqt.Server.Manager.SxcManager, ToSic.Sxc.Oqtane.Server",
            // This must contain all versions with a SQL script and current/latest version
            // list versions with sql scripts in \ToSic.Sxc.Oqt.Server\Scripts\
            // Always keep this in sync wth the App and Content ModuleInfo.cs
            ReleaseVersions = "0.0.1,12.00.00,12.02.00,12.02.01,12.03.00",
            Dependencies = "ToSic.Sxc.Oqtane.Shared",
            Categories = "Common",
            //Runtimes = "Server",
        };
    }
}
