namespace ToSic.Sxc.ImportExport.Package.Sys;

public record PackageInstallAbout
{
    public required string Title { get; init; }

    public required string Description { get; init; }
}