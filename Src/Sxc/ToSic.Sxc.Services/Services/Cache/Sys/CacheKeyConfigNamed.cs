namespace ToSic.Sxc.Services.Cache.Sys;

/// <summary>
/// Describes a cache configuration for named parameters.
/// </summary>
public record CacheKeyConfigNamed
{
    public required string? Names { get; init; }
    public required bool CaseSensitive { get; init; }
}