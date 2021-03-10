using Oqtane.Models;
using Oqtane.Modules;

namespace ToSic.Sxc.Oqt.Content
{
    public class ModuleInfo : IModule
    {
        public ModuleDefinition ModuleDefinition => new ModuleDefinition
        {
            Name = "Content",
            Description = "2sxc helps create designed, cross-platform content",
            Version = "0.0.59",
            ServerManagerType = "ToSic.Sxc.Oqt.Server.Manager.SxcManager, ToSic.Sxc.Oqtane.Server",
            ReleaseVersions = "0.0.57,0.0.59",
            Dependencies = "ToSic.Sxc.Oqtane.Shared",
            Categories = "Common",
            Runtimes = "Server",
        };
    }
}
