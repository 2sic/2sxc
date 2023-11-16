using ToSic.Eav.Plumbing;
using ToSic.Eav.Run.Capabilities;
using static ToSic.Eav.Run.Capabilities.SystemCapabilityListForImplementation;

namespace ToSic.Sxc.Dnn.Run.Capabilities
{
    public class SysFeatureDetectorCSharp6 : SysFeatureDetector
    {
        public SysFeatureDetectorCSharp6() : base(CSharp06.Clone(name: " optional in Dnn 9.6.1+")) { }

        public override bool IsEnabled => _isEnabledCache ?? (_isEnabledCache = DetectIfCs6IsInstalled()).Value;
        private static bool? _isEnabledCache;

        // DETECT based on installed stuff (DLLs, available APIs?)
        // Goal is that it can tell if the newer CodeDom library has been installed or not
        // I'll then use it to build a config in the App, so the app can warn if a feature is missing
        private static bool DetectIfCs6IsInstalled() => AssemblyHandling.HasType("Microsoft.CodeDom.Providers.DotNetCompilerPlatform") && RoslynCompilerCapability.CheckCsharpLangVersion("6");
    }

    public class SysFeatureDetectorCSharp7 : SysFeatureDetector
    {
        public SysFeatureDetectorCSharp7() : base(CSharp07.Clone(name: CSharp07.Name + " optional in Dnn 9.? (todo)")) { }

        public override bool IsEnabled => _isEnabledCache ?? (_isEnabledCache = DetectIfCs73IsInstalled()).Value;
        private static bool? _isEnabledCache;

        // DETECT based on installed stuff (DLLs, available APIs?)
        // Goal is that it can tell if the newer CodeDom library has been installed or not
        // I'll then use it to build a config in the App, so the app can warn if a feature is missing
        private static bool DetectIfCs73IsInstalled() => AssemblyHandling.HasType("Microsoft.CodeDom.Providers.DotNetCompilerPlatform") && RoslynCompilerCapability.CheckCsharpLangVersion("7.3");
    }

    public class SysFeatureDetectorCSharp8 : SysFeatureDetector
    {
        public SysFeatureDetectorCSharp8() : base(CSharp08.Clone(name: CSharp08.Name + " optional in Dnn 9.13+ (ca. todo)"), false) { }
    }

    public class SysFeatureDetectorCSharp9 : SysFeatureDetector
    {
        public SysFeatureDetectorCSharp9() : base(CSharp09.Clone(name: CSharp09.Name + " (not available in Dnn)"), false) { }
    }

    public class SysFeatureDetectorCSharp10 : SysFeatureDetector
    {
        public SysFeatureDetectorCSharp10() : base(CSharp10.Clone(name: CSharp10.Name + " (not available in Dnn)"), false) { }
    }

    public class SysFeatureDetectorCSharp11 : SysFeatureDetector
    {
        public SysFeatureDetectorCSharp11() : base(CSharp11.Clone(name: CSharp11.Name + " (not available in Dnn)"), false) { }
    }

    public class SysFeatureDetectorCSharp12 : SysFeatureDetector
    {
        public SysFeatureDetectorCSharp12() : base(CSharp12.Clone(name: CSharp12.Name + " (not available in Dnn)"), false) { }
    }

}
