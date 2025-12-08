using System.Text.Json.Serialization;
// ReSharper disable UnusedMember.Global

namespace ToSic.Sxc.ImportExport.InstallPackage.Sys;

/// <summary>
/// This is a package definition structure.
/// This file should be placed in the root of any ZIP exported in future,
/// so that importing systems can quickly determine if it's compatible and other aspects.
/// </summary>
/// <remarks>
/// It should be extended with additional information about version compatibility, platforms, etc.
/// </remarks>
public record InstallPackage
{
    /// <summary>
    /// The name in the root of the package ZIP file where this definition is stored.
    /// Note that upon import, this file should never be imported.
    /// </summary>
    public const string FileName = "package-install.json";

    /// <inheritdoc cref="InstallPackageHeader"/>
    public InstallPackageHeader Header { get; init; } = new();

    public required InstallPackageAbout About { get; init; }

    /// <summary>
    /// List of extensions in this package.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<InstallPackageExtension>? Extensions { get; init; }
}

/// <summary>
/// Header / compatibility information about the package.
/// </summary>
public record InstallPackageHeader
{
    private const string CurrentPackageFormatVersion = "00.00.01";

    public string PackageVersion { get; init; } = CurrentPackageFormatVersion;

    public PackageTypes PackageType { get; init; } = PackageTypes.App;

    public enum PackageTypes
    {
        /// <summary>
        /// An App package - contains an application with optional extensions.
        /// </summary>
        App,

        /// <summary>
        /// An AppExtension package - things to be installed into an existing app.
        /// </summary>
        AppExtension,
    }
}

public record InstallPackageAbout
{
    public required string Title { get; init; }

    public required string Description { get; init; }
}

/// <summary>
/// Definition of a single extension within the package.
/// </summary>
/// <param name="Name">Extension name - usually a lower-case folder name</param>
/// <param name="DefinitionFile">Path to the extension definition file</param>
/// <param name="IndexFile">Path to the index file</param>
/// <param name="IndexFileHash">Hash of the index file for verification</param>
public record InstallPackageExtension(string Name, string DefinitionFile, string IndexFile, string IndexFileHash);