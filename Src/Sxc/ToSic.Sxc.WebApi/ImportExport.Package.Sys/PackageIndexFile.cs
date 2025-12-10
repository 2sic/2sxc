using System.Text.Json.Serialization;

namespace ToSic.Sxc.ImportExport.Package.Sys;

/// <summary>
/// This is for a file which contains the index of a set of files, and also the lock/hashes,
/// </summary>
public record PackageIndexFile
{
    /// <summary>
    /// File name for an extension index/lock file inside an Extension's App_Data folder.
    /// </summary>
    /// <remarks>
    /// Should have a generic name which will also be usable for non-extension packages in the future.
    /// But we can't use `package.lock.json` because it looks too similar to `package-lock.json` used by npm.
    /// </remarks>
    public const string LockFileName = "package-index.json";

    [JsonPropertyOrder(1)]
#pragma warning disable CA1822
    public string Comments => "This file contains the list of files in a package with their hashes to check if files were changed.";
#pragma warning restore CA1822

    [JsonPropertyOrder(2)]
    public required string Version { get; init; }

    [JsonPropertyOrder(3)]
    public required List<PackageIndexFileEntry> Files { get; init; }
}