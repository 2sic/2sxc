using Oqtane.Models;
using Oqtane.Modules;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Oqt.App;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ModuleInfo : IModule
{
    public ModuleDefinition ModuleDefinition => Content.ModuleInfo.BuildModuleDefinition("App", "2sxc Apps are rich, self-contained extensions");
}