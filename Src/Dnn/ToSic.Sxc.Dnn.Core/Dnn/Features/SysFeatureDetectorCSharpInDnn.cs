using ToSic.Sys.Capabilities.SysFeatures;
using static ToSic.Sys.Capabilities.SysFeatures.SysFeatureSuggestions;

namespace ToSic.Sxc.Dnn.Features;

// Important: `RoslynCompilerCapability`
// has been commented out for now
// since it's not used anymore.
// If DNN starts to implement newer roslyn compilers, we may need to re-enable this, but for now it's not needed.


#region C# Features which are always enabled

internal class SysFeatureDetectorCSharp6() : SysFeatureDetector(CSharp06 with { Name = " required for 2sxc 17" }, true);

internal class SysFeatureDetectorCSharp7() : SysFeatureDetector(CSharp07 with { Name = $"{CSharp07.Name} required for 2sxc 17" }, true);

/// <summary>
/// C# 8 is default in DNN 9 and 10; since 2sxc v22 is min DNN 10 it's always true.
/// </summary>
internal class SysFeatureDetectorCSharp8() : SysFeatureDetector(CSharp08 with { Name = $"{CSharp08.Name} required for 2sxc 17" }, true);

#endregion


#region C# Features which are currently never enabled in DNN

internal class SysFeatureDetectorCSharp9() : SysFeatureDetector(CSharp09 with { Name = $"{CSharp09.Name} (not available in Dnn)" });

internal class SysFeatureDetectorCSharp10() : SysFeatureDetector(CSharp10 with { Name = $"{CSharp10.Name} (not available in Dnn)" });

internal class SysFeatureDetectorCSharp11() : SysFeatureDetector(CSharp11 with { Name = $"{CSharp11.Name} (not available in Dnn)" });

internal class SysFeatureDetectorCSharp12() : SysFeatureDetector(CSharp12 with { Name = $"{CSharp12.Name} (not available in Dnn)" });

internal class SysFeatureDetectorCSharp13() : SysFeatureDetector(CSharp13 with { Name = $"{CSharp13.Name} (not available in Dnn)" });

internal class SysFeatureDetectorCSharp14() : SysFeatureDetector(CSharp14 with { Name = $"{CSharp14.Name} (not available in Dnn)" });

internal class SysFeatureDetectorCSharp15() : SysFeatureDetector(CSharp15 with { Name = $"{CSharp15.Name} (not available in Dnn)" });

internal class SysFeatureDetectorCSharp16() : SysFeatureDetector(CSharp16 with { Name = $"{CSharp16.Name} (not available in Dnn)" });

#endregion
