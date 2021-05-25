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
            ReleaseVersions = "0.0.1,12.00.00", // list versions with sql scripts in \ToSic.Sxc.Oqt.Server\Scripts\
            Dependencies = "ToSic.Sxc.Oqtane.Shared",
            Categories = "Common",
            //Runtimes = "Server",
        };
    }
}
