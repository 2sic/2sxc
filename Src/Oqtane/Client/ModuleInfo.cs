using Oqtane.Models;
using Oqtane.Modules;

namespace ToSic.Sxc
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Sxc",
            Description = "Sxc",
            Version = "1.0.0",
            ServerManagerType = "ToSic.Sxc.Manager.SxcManager, ToSic.Sxc.Server.Oqtane",
            ReleaseVersions = "1.0.0",
            Dependencies = "ToSic.Sxc.Shared.Oqtane"
        };
    }
}
