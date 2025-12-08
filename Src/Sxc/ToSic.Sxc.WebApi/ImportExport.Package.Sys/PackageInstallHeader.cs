namespace ToSic.Sxc.ImportExport.Package.Sys;

/// <summary>
/// Header / compatibility information about the package.
/// </summary>
public record PackageInstallHeader
{
    private const string CurrentPackageFormatVersion = "00.00.01";

    public string PackageVersion { get; init; } = CurrentPackageFormatVersion;

    public PackageTypes PackageType { get; init; } = PackageTypes.App;


}
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