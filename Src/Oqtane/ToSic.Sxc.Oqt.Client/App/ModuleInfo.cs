using Oqtane.Models;
using Oqtane.Modules;

namespace ToSic.Sxc.Oqt.App
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "App",
            Description = "2sxc helps create designed, cross-platform content",
            Version = "12.01.00",
            ServerManagerType = "ToSic.Sxc.Oqt.Server.Manager.SxcManager, ToSic.Sxc.Oqtane.Server",
            // This must contain all versions with a SQL script and current/latest version
            // list versions with sql scripts in \ToSic.Sxc.Oqt.Server\Scripts\
            // Always keep this in sync wth the App and Content ModuleInfo.cs
            ReleaseVersions = "0.0.1,12.00.00,12.01.00", 
            Dependencies = "ToSic.Sxc.Oqtane.Shared",
            Categories = "Common",
            //Runtimes = "Server",
        };
    }
}
