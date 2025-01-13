namespace ToSic.Sxc.Oqt.Shared.Models
{
    public record RenderParameters
    {
        public int AliasId { get; init; }
        public int PageId { get; init; }
        public int ModuleId { get; init; }
        public string Culture { get; init; }
        public bool PreRender { get; init; }
        public string OriginalParameters { get; init; }
    }
}
