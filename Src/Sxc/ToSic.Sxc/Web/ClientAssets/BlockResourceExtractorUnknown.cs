using ToSic.Eav.Internal.Unknown;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Web.ClientAssets;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Web
{
    // ReSharper disable once UnusedMember.Global
    internal class BlockResourceExtractorUnknown: BlockResourceExtractor
    {
        public BlockResourceExtractorUnknown(WarnUseOfUnknown<BlockResourceExtractorUnknown> _, PageServiceShared pageServiceShared): base(pageServiceShared)
        { }

        protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string renderedTemplate, ClientAssetsExtractSettings settings)
            => (renderedTemplate, false);
    }
}
