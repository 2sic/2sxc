namespace ToSic.Sxc.ImportExport.Package.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record PackageInstallAbout
{
    public required string Title { get; init; }

    public required string Description { get; init; }
}