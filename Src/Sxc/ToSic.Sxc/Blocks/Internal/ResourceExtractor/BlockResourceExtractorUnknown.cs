using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Web.Internal.ClientAssets;
using ToSic.Sxc.Web.Internal.PageService;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Blocks.Internal;

// ReSharper disable once UnusedMember.Global
internal class BlockResourceExtractorUnknown(WarnUseOfUnknown<BlockResourceExtractorUnknown> _, PageServiceShared pageServiceShared) : BlockResourceExtractor(pageServiceShared)
{
    protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string renderedTemplate, ClientAssetsExtractSettings settings)
        => (renderedTemplate, false);
}