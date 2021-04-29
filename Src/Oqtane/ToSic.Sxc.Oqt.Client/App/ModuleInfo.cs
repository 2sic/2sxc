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
            Version = "11.90.0",
            ServerManagerType = "ToSic.Sxc.Oqt.Server.Manager.SxcManager, ToSic.Sxc.Oqtane.Server",
            ReleaseVersions = "0.0.1,11.90.0",
            Dependencies = "ToSic.Sxc.Oqtane.Shared",
            Categories = "Common",
            //Runtimes = "Server",
        };
    }
    /*
     * 0.0.1 - Oqtane db fix for https://github.com/oqtane/oqtane.framework/issues/1269 (probably we will remove latter).
     * 11.90.0 - 2sxc oqtane module pre-release.
     */
}
