using ToSic.Eav.Run.Capabilities;
using static ToSic.Eav.Run.Capabilities.SystemCapabilityListForImplementation;

namespace ToSic.Sxc.Dnn.Run.Capabilities
{
    public class SystemCapabilityCSharp7: SystemCapabilityBase
    {
        public SystemCapabilityCSharp7() : base(CSharp7) { }

        // TODO: DETECT based on installed stuff
        public override bool IsEnabled => false;
    }

    public class SystemCapabilityCSharp8: SystemCapabilityBase
    {
        public SystemCapabilityCSharp8() : base(CSharp8.Clone(name: CSharp8.Name + " - not available in Dnn."), false) { }
    }

    public class SystemCapabilityCSharp9: SystemCapabilityBase
    {
        public SystemCapabilityCSharp9() : base(CSharp9.Clone(name: CSharp9.Name + " - not available in Dnn."), false) { }
    }

    public class SystemCapabilityCSharp10: SystemCapabilityBase
    {
        public SystemCapabilityCSharp10() : base(CSharp10.Clone(name: CSharp10.Name + " - not available in Dnn."), false) { }
    }

    public class SystemCapabilityCSharp11: SystemCapabilityBase
    {
        public SystemCapabilityCSharp11() : base(CSharp11.Clone(name: CSharp11.Name + " - not available in Dnn."), false) { }
    }

    public class SystemCapabilityCSharp12: SystemCapabilityBase
    {
        public SystemCapabilityCSharp12() : base(CSharp12.Clone(name: CSharp12.Name + " - not available in Dnn."), false) { }
    }

}
