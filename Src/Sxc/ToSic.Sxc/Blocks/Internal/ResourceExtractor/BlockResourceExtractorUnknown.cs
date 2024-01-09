using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Web.Internal;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.PageService;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Blocks.Internal;

// ReSharper disable once UnusedMember.Global
internal class BlockResourceExtractorUnknown(WarnUseOfUnknown<BlockResourceExtractorUnknown> _, PageServiceShared pageServiceShared) : BlockResourceExtractor(pageServiceShared)
{
    protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string renderedTemplate, ClientAssetsExtractSettings settings)
        => (renderedTemplate, false);
}