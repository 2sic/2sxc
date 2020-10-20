using Oqtane.Models;
using Oqtane.Modules;

namespace ToSic.Sxc.OqtaneModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Sxc",
            Description = "Sxc",
            Version = "1.0.0",
            ServerManagerType = "ToSic.Sxc.OqtaneModule.Manager.SxcManager, ToSic.Sxc.OqtaneModule.Server",
            ReleaseVersions = "1.0.0",
            Dependencies = "ToSic.Sxc.OqtaneModule.Shared"
        };
    }
}
