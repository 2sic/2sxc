using ToSic.Lib.Helpers;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Web.ClientAssets;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Oqt.Server.Blocks.Output
{
    public class OqtBlockResourceExtractor: BlockResourceExtractor
    {
        public OqtBlockResourceExtractor(PageServiceShared pageServiceShared): base(pageServiceShared) { }

        protected override ClientAssetsExtractSettings Settings => _settings.Get(() => new(extractAll: true));
        private readonly GetOnce<ClientAssetsExtractSettings> _settings = new();

        protected override (string Template, bool Include2sxcJs) ExtractFromHtml(string html, ClientAssetsExtractSettings settings)
        {
            var include2SxcJs = false;

            // Handle Client Dependency injection
            html = ExtractExternalScripts(html, ref include2SxcJs, settings);

            // Handle inline JS
            html = ExtractInlineScripts(html);

            // Handle Styles
            html = ExtractStyles(html, settings);

            Assets.ForEach(a => a.PosInPage = OqtPositionName(a.PosInPage));

            return (html, include2SxcJs);
        }



        private string OqtPositionName(string position)
        {
            position = position.ToLowerInvariant();

            return position switch
            {
                "body" => position,
                "head" => position,
                _ => "body"
            };
        }

    }

}
