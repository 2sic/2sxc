namespace ToSic.Sxc.Code.Sys.HotBuild;

/// <summary>
/// Contains constants and settings for Roslyn compilation.
/// These settings define the language version and preprocessor symbols used during compilation.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
public class RoslynConstants
{
    /// <summary>
    /// Specifies the C# language version to use during Roslyn compilation.
    /// The "Preview" version allows the use of the latest language features.
    /// </summary>
    public const string LanguageVersion = "latest";
}