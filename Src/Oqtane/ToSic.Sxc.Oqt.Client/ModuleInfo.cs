using Oqtane.Models;
using Oqtane.Modules;

namespace ToSic.Sxc.Oqt.Client
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "2sxc",
            Description = "2sxc helps create designed, cross-platform content",
            Version = "0.0.55",
            ServerManagerType = "ToSic.Sxc.Oqt.Server.Manager.SxcManager, ToSic.Sxc.Oqtane.Server",
            ReleaseVersions = "0.0.55",
            Dependencies = "ToSic.Sxc.Oqtane.Shared",
            Categories = "Common",
            Runtimes = "Server",
        };
    }
}
