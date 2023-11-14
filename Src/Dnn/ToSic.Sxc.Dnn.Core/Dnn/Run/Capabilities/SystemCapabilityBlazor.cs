using ToSic.Eav.Run.Capabilities;
using static ToSic.Eav.Run.Capabilities.SystemCapabilityListForImplementation;

namespace ToSic.Sxc.Dnn.Run.Capabilities
{
    internal class SystemCapabilityBlazor: SystemCapability
    {
        public SystemCapabilityBlazor() : base(Blazor.Clone(name: Blazor.Name + " not available in Dnn."), false)
        {
        }
    }
}
