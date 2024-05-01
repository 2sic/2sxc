using ToSic.Eav.Internal.Features;
using static ToSic.Eav.Internal.Features.SysFeatureSuggestions;
using static ToSic.Sxc.Dnn.Compile.RoslynCompilerCapability;

namespace ToSic.Sxc.Dnn.Features;

internal class SysFeatureDetectorCSharp6() : SysFeatureDetector(CSharp06.Clone(name: " required for 2sxc 17"))
{
    public override bool IsEnabled => _isEnabledCache ??= CheckCsharpLangVersion(6); // C# 6
    private static bool? _isEnabledCache;
}

internal class SysFeatureDetectorCSharp7() : SysFeatureDetector(CSharp07.Clone(name: $"{CSharp07.Name} required for 2sxc 17"))
{
    public override bool IsEnabled => _isEnabledCache ??= CheckCsharpLangVersion(703); // C# 7.3;
    private static bool? _isEnabledCache;
}

internal class SysFeatureDetectorCSharp8() : SysFeatureDetector(CSharp08.Clone(name: $"{CSharp08.Name} required for 2sxc 17"))
{
    public override bool IsEnabled => _isEnabledCache ??= CheckCsharpLangVersion(800); // C# 8; - code is 800
    private static bool? _isEnabledCache;
}

internal class SysFeatureDetectorCSharp9() : SysFeatureDetector(CSharp09.Clone(name: $"{CSharp09.Name} (not available in Dnn)"));

internal class SysFeatureDetectorCSharp10() : SysFeatureDetector(CSharp10.Clone(name: $"{CSharp10.Name} (not available in Dnn)"));

internal class SysFeatureDetectorCSharp11() : SysFeatureDetector(CSharp11.Clone(name: $"{CSharp11.Name} (not available in Dnn)"));

internal class SysFeatureDetectorCSharp12() : SysFeatureDetector(CSharp12.Clone(name: $"{CSharp12.Name} (not available in Dnn)"));