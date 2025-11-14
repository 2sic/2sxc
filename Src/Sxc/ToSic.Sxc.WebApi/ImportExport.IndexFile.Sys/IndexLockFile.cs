namespace ToSic.Sxc.ImportExport.IndexFile.Sys;

/// <summary>
/// This is for a file which contains the index of a set of files, and also the lock/hashes,
/// </summary>
public record IndexLockFile
{
    public required string Version { get; init; }

    public required List<IndexLockFileEntry> Files { get; init; }
}

public record IndexLockFileEntry
{
    public required string File { get; init; }

    public required string Hash { get; init; }
}

