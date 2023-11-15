using ToSic.Eav.Plumbing;
using ToSic.Eav.Run.Capabilities;
using static ToSic.Eav.Run.Capabilities.SystemCapabilityListForImplementation;

namespace ToSic.Sxc.Dnn.Run.Capabilities
{
    // todo: @stv
    public class SystemCapabilityCSharp6 : SystemCapability
    {
        public SystemCapabilityCSharp6() : base(CSharp06) { }

        public override bool IsEnabled => _isEnabledCache ?? (_isEnabledCache = DetectIfCs6IsInstalled()).Value;
        private static bool? _isEnabledCache;

        // DETECT based on installed stuff (DLLs, available APIs?)
        // Goal is that it can tell if the newer CodeDom library has been installed or not
        // I'll then use it to build a config in the App, so the app can warn if a feature is missing
        private static bool DetectIfCs6IsInstalled() => AssemblyHandling.HasType("Microsoft.CodeDom.Providers.DotNetCompilerPlatform");
    }

    public class SystemCapabilityCSharp7: SystemCapability
    {
        public SystemCapabilityCSharp7() : base(CSharp07) { }

        public override bool IsEnabled => _isEnabledCache ?? (_isEnabledCache = DetectIfCs73IsInstalled()).Value;
        private static bool? _isEnabledCache;

        // DETECT based on installed stuff (DLLs, available APIs?)
        // Goal is that it can tell if the newer CodeDom library has been installed or not
        // I'll then use it to build a config in the App, so the app can warn if a feature is missing
        private static bool DetectIfCs73IsInstalled() => AssemblyHandling.HasType("Microsoft.CodeDom.Providers.DotNetCompilerPlatform");
    }

    public class SystemCapabilityCSharp8: SystemCapability
    {
        public SystemCapabilityCSharp8() : base(CSharp08.Clone(name: CSharp08.Name + " - not available in Dnn."), false) { }
    }

    public class SystemCapabilityCSharp9: SystemCapability
    {
        public SystemCapabilityCSharp9() : base(CSharp09.Clone(name: CSharp09.Name + " - not available in Dnn."), false) { }
    }

    public class SystemCapabilityCSharp10: SystemCapability
    {
        public SystemCapabilityCSharp10() : base(CSharp10.Clone(name: CSharp10.Name + " - not available in Dnn."), false) { }
    }

    public class SystemCapabilityCSharp11: SystemCapability
    {
        public SystemCapabilityCSharp11() : base(CSharp11.Clone(name: CSharp11.Name + " - not available in Dnn."), false) { }
    }

    public class SystemCapabilityCSharp12: SystemCapability
    {
        public SystemCapabilityCSharp12() : base(CSharp12.Clone(name: CSharp12.Name + " - not available in Dnn."), false) { }
    }

}
