using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;

namespace ToSic.Sxc.Dnn.Pages;

internal class ModuleWithContent
{
    public Guid ContentGroup;
    public ModuleInfo Module;
    public TabInfo Page;
}