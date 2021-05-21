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
            Version = "12.01.00",
            ServerManagerType = "ToSic.Sxc.Oqt.Server.Manager.SxcManager, ToSic.Sxc.Oqtane.Server",
            ReleaseVersions = "12.01.00",
            Dependencies = "ToSic.Sxc.Oqtane.Shared",
            Categories = "Common",
            //Runtimes = "Server",
        };
    }
}
