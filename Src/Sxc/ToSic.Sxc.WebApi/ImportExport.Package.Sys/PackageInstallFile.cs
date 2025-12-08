using System.Text.Json.Serialization;

namespace ToSic.Sxc.ImportExport.Package.Sys;

/// <summary>
/// This is a package definition structure.
/// This file should be placed in the root of any ZIP exported in future,
/// so that importing systems can quickly determine if it's compatible and other aspects.
/// </summary>
/// <remarks>
/// It should be extended with additional information about version compatibility, platforms, etc.
/// </remarks>
public record PackageInstallFile
{
    /// <summary>
    /// The name in the root of the package ZIP file where this definition is stored.
    /// Note that upon import, this file should never be imported.
    /// </summary>
    public const string FileName = "package-install.json";

    /// <inheritdoc cref="PackageInstallHeader"/>
    public PackageInstallHeader Header { get; init; } = new();

    public required PackageInstallAbout About { get; init; }

    /// <summary>
    /// List of extensions in this package.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<PackageInstallExtension>? Extensions { get; init; }
}