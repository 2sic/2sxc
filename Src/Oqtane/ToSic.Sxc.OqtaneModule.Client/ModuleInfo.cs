using Oqtane.Models;
using Oqtane.Modules;

namespace ToSic.Sxc.OqtaneModule
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "2sxc",
            Description = "2sxc is a Oqtane module to create attractive and designed content. It solves the common problem, allowing the web designer to create designed templates for different content elements, so that the user must only fill in fields and receive a perfectly designed and animated output.",
            Version = "0.0.1",
            ServerManagerType = "ToSic.Sxc.OqtaneModule.Manager.SxcManager, ToSic.Sxc.OqtaneModule.Server",
            ReleaseVersions = "0.0.1",
            Dependencies = "ToSic.Sxc.OqtaneModule.Shared",
            Categories = "Developer",
            Runtimes = "Server",
        };
    }
}
