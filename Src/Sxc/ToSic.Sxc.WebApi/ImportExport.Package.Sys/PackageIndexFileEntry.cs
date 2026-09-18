namespace ToSic.Sxc.ImportExport.Package.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record PackageIndexFileEntry
{
    public required string File { get; init; }

    public required string Hash { get; init; }
}