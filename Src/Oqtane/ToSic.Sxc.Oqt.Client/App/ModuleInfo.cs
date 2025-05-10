using Oqtane.Models;
using Oqtane.Modules;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ModuleInfo : IModule
{
    public ModuleDefinition ModuleDefinition => Content.ModuleInfo.BuildModuleDefinition("App", "2sxc Apps are rich, self-contained extensions");
}