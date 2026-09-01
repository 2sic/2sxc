using ToSic.Sxc.Sys.Render.PageContext;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Render.StaticAssets.Sys;

// ReSharper disable once UnusedMember.Global
internal class AssetsExtractorUnknown(WarnUseOfUnknown<AssetsExtractorUnknown> _, IPageServiceShared pageServiceShared) : AssetsExtractor(pageServiceShared)
{
    protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string renderedTemplate, ClientAssetsExtractSettings settings)
        => (renderedTemplate, false);
}