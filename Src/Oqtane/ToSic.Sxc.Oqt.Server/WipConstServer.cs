using System.ComponentModel;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;
using ToSic.Sxc.Run.Context;

namespace ToSic.Sxc.Oqt.Server
{
    class WipConstServer
    {
        public static IPageInternal NullPage = new PageNull();
        public static IModuleInternal NullContainer = new ModuleNull();
    }
}
