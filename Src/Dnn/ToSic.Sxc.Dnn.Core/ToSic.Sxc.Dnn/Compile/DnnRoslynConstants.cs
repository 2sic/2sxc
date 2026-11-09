using ToSic.Sxc.Code.Sys.HotBuild;

namespace ToSic.Sxc.Dnn.Compile;

/// <summary>
/// Contains constants used for Roslyn compilation in the DNN context.
/// These constants define default compiler options, language versions, and other settings.
/// </summary>
internal class DnnRoslynConstants
{
    /// <summary>
    /// Compiler options used during Roslyn compilation.
    /// - /optimize-: Disables compiler optimizations to make debugging easier.
    /// - /warnaserror-: Treats warnings as warnings (not errors), allowing the build to succeed even with warnings.
    /// - {CompilerOptionLanguageVersion}: Specifies the C# language version to use (e.g., "preview").
    /// - {DefaultDisableWarnings}: Suppresses specific warnings defined in the DNN web.config.
    /// - {CompilerOptionDefine}: Defines preprocessor symbols (e.g., DEBUG, NETFRAMEWORK).
    /// </summary>
    public const string CompilerOptions = $"/optimize- /warnaserror- {DefaultDisableWarnings} {CompilerOptionLanguageVersion} {CompilerOptionDefine}";

    /// <summary>
    /// Compiler warnings to suppress, as specified in the default DNN web.config file.
    /// These warnings are typically related to obsolete or deprecated APIs.
    /// </summary>
    private const string DefaultDisableWarnings = "/nowarn:1659;1699;1701;612;618";

    /// <summary>
    /// Compiler option to specify the language version.
    /// This is constructed using the <see cref="RoslynConstants.LanguageVersion"/>.
    /// </summary>
    private const string CompilerOptionLanguageVersion = $"/langversion:{RoslynConstants.LanguageVersion}"; // "8" till 2025-08-20;

    /// <summary>
    /// Defines the preprocessor symbols to be used during compilation.
    /// These symbols allow conditional compilation for different build configurations and platforms.
    /// Examples:
    /// - NETFRAMEWORK: Indicates targeting the .NET Framework (used for DNN).
    /// </summary>
    private const string DefineConstants = "NETFRAMEWORK";

    /// <summary>
    /// Compiler option to define preprocessor symbols.
    /// This is constructed using the <see cref="DefineConstants"/>.
    /// </summary>
    private const string CompilerOptionDefine = $"/define:{DefineConstants}";
}