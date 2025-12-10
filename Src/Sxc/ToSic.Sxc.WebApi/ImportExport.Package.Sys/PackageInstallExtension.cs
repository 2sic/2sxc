namespace ToSic.Sxc.ImportExport.Package.Sys;

/// <summary>
/// Definition of a single extension within the package.
/// </summary>
/// <param name="Name">Extension name - usually a lower-case folder name</param>
/// <param name="DefinitionFile">Path to the extension definition file</param>
/// <param name="IndexFile">Path to the index file</param>
/// <param name="IndexFileHash">Hash of the index file for verification</param>
public record PackageInstallExtension(
    string Name,
    string DefinitionFile,
    string IndexFile,
    string IndexFileHash
);