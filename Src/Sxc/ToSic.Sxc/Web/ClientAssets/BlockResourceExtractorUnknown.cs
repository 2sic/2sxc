using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Web
{
    // ReSharper disable once UnusedMember.Global
    public class BlockResourceExtractorUnknown: BlockResourceExtractor
    {
        public BlockResourceExtractorUnknown(WarnUseOfUnknown<BlockResourceExtractorUnknown> _, PageServiceShared pageServiceShared): base(pageServiceShared)
        { }

        protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string renderedTemplate) => (renderedTemplate, false);
    }
}
