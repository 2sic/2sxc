using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;

namespace ToSic.Sxc.Dnn.Pages;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ModuleWithContent
{
    public Guid ContentGroup;
    public ModuleInfo Module;
    public TabInfo Page;
}