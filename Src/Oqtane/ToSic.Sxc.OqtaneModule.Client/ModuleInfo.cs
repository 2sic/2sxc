using Oqtane.Models;
using Oqtane.Modules;

namespace ToSic.Sxc.OqtaneModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Sxc",
            Description = "2sxc Oqtane Module",
            Version = "0.0.1",
            ServerManagerType = "ToSic.Sxc.OqtaneModule.Manager.SxcManager, ToSic.Sxc.OqtaneModule.Server",
            ReleaseVersions = "0.0.1",
            Dependencies = "ToSic.Sxc.OqtaneModule.Shared",
            Categories = "Develop",
            Runtimes = "Server",
        };
    }
}
