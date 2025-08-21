using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using ToSic.Sxc.Code.Sys.HotBuild;

namespace ToSic.Sxc.Oqt.Server.Code.Sys;

/// <summary>
/// Contains constants and settings for Roslyn compilation in the Oqtane context.
/// These settings define the language version and preprocessor symbols used during compilation.
/// </summary>
internal class OqtRoslynConstants
{
    /// <summary>
    /// Specifies the C# language version to use during Roslyn compilation.
    /// The "Preview" version allows the use of the latest language features.
    /// </summary>
    public static readonly LanguageVersion LanguageVersion = Enum.TryParse<LanguageVersion>(RoslynConstants.LanguageVersion, out var languageVersion) ? languageVersion : LanguageVersion.Preview;

    /// <summary>
    /// Defines the preprocessor symbols to be used during compilation.
    /// These symbols allow conditional compilation for different build configurations and platforms.
    /// Examples:
    /// - NETCOREAPP: Indicates targeting .NET Core or .NET 5+, used for Oqtane.
    /// </summary>
    public static ImmutableArray<string> PreprocessorSymbols = ["NETCOREAPP"];
}