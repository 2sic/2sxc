using ToSic.Lib.Services;
using ToSic.Sxc.Render.Sys.Output;
using ToSic.Sxc.Sys.Render.PageContext;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Render.Sys.ResourceExtractor;

// ReSharper disable once UnusedMember.Global
internal class BlockResourceExtractorUnknown(WarnUseOfUnknown<BlockResourceExtractorUnknown> _, IPageServiceShared pageServiceShared) : BlockResourceExtractor(pageServiceShared)
{
    protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string renderedTemplate, ClientAssetsExtractSettings settings)
        => (renderedTemplate, false);
}