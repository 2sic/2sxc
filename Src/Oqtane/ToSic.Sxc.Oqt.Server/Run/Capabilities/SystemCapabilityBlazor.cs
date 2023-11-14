using ToSic.Eav.Run.Capabilities;
using static ToSic.Eav.Run.Capabilities.SystemCapabilityListForImplementation;

namespace ToSic.Sxc.Oqt.Server.Run.Capabilities
{
    internal class SystemCapabilityBlazor: SystemCapability
    {
        public SystemCapabilityBlazor() : base(Blazor, true)
        {
        }
    }
}
