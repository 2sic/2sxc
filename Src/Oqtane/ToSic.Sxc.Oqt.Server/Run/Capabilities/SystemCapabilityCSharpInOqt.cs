using ToSic.Eav.Run.Capabilities;
using static ToSic.Eav.Run.Capabilities.SystemCapabilityListForImplementation;

namespace ToSic.Sxc.Oqt.Server.Run.Capabilities
{
    public class SystemCapabilityCSharp7: SystemCapability
    {
        public SystemCapabilityCSharp7() : base(CSharp07, true) { }
    }
    public class SystemCapabilityCSharp8: SystemCapability
    {
        public SystemCapabilityCSharp8() : base(CSharp08, true) { }
    }
    public class SystemCapabilityCSharp9: SystemCapability
    {
        public SystemCapabilityCSharp9() : base(CSharp09, true) { }
    }
    public class SystemCapabilityCSharp10: SystemCapability
    {
        public SystemCapabilityCSharp10() : base(CSharp10, true) { }
    }
    public class SystemCapabilityCSharp11: SystemCapability
    {
        public SystemCapabilityCSharp11() : base(CSharp11, true) { }
    }
    public class SystemCapabilityCSharp12: SystemCapability
    {
        public SystemCapabilityCSharp12() : base(CSharp12, false) { }
    }
}
