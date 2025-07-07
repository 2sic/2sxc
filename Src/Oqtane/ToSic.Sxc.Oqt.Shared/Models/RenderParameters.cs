namespace ToSic.Sxc.Oqt.Shared.Models;

public record RenderParameters
{
    public required int AliasId { get; init; }
    public required int PageId { get; init; }
    public required int ModuleId { get; init; }
    public required string Culture { get; init; }
    public required bool PreRender { get; init; }
    public required string OriginalParameters { get; init; }
}