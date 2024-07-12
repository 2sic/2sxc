namespace ToSic.Sxc.Oqt.Shared.Models
{
    public class RenderParameters
    {
        public int AliasId { get; init; }
        public int PageId { get; init; }
        public int ModuleId { get; init; }
        public string Culture { get; init; }
        public bool PreRender { get; init; }
        public string OriginalParameters { get; init; }

        public RenderParameters Clone()
        {
            return new RenderParameters()
            {
                AliasId = AliasId,
                PageId = PageId,
                ModuleId = ModuleId,
                Culture = Culture,
                PreRender = PreRender,
                OriginalParameters = OriginalParameters
            };
        }

        public bool Equals(RenderParameters other)
        {
            return AliasId == other.AliasId &&
                   PageId == other.PageId &&
                   ModuleId == other.ModuleId &&
                   Culture == other.Culture &&
                   PreRender == other.PreRender &&
                   OriginalParameters == other.OriginalParameters;
        }
    }
}
