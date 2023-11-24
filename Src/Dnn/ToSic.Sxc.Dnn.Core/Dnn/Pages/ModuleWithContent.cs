using System;
using System.ComponentModel;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;

namespace ToSic.Sxc.Dnn.Pages
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ModuleWithContent
    {
        public Guid ContentGroup;
        public ModuleInfo Module;
        public TabInfo Page;
    }
}
