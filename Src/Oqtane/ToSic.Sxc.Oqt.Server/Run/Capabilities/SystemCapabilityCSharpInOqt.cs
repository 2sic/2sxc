using ToSic.Eav.Run.Capabilities;
using static ToSic.Eav.Run.Capabilities.SystemCapabilityListForImplementation;

namespace ToSic.Sxc.Oqt.Server.Run.Capabilities
{
    public class SystemCapabilityCSharp7: SystemCapabilityBase
    {
        public SystemCapabilityCSharp7() : base(CSharp7, true) { }
    }
    public class SystemCapabilityCSharp8: SystemCapabilityBase
    {
        public SystemCapabilityCSharp8() : base(CSharp8, true) { }
    }
    public class SystemCapabilityCSharp9: SystemCapabilityBase
    {
        public SystemCapabilityCSharp9() : base(CSharp9, true) { }
    }
    public class SystemCapabilityCSharp10: SystemCapabilityBase
    {
        public SystemCapabilityCSharp10() : base(CSharp10, true) { }
    }
    public class SystemCapabilityCSharp11: SystemCapabilityBase
    {
        public SystemCapabilityCSharp11() : base(CSharp11, true) { }
    }
    public class SystemCapabilityCSharp12: SystemCapabilityBase
    {
        public SystemCapabilityCSharp12() : base(CSharp12, false) { }
    }
}
