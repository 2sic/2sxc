using Oqtane.Modules;
using Oqtane.UI;
using System;
using Oqtane.Services;
using System.Net.Http;
using Oqtane.Shared;

namespace ToSic.Sxc.Oqt.Client.Services
{
    public class OqtPrerenderService : IOqtPrerenderService, IService
    {
        private readonly IOqtPrerenderSupportService _oqtPrerenderSupportService;

        public OqtPrerenderService(IOqtPrerenderSupportService oqtPrerenderSupportService)
        {
            _oqtPrerenderSupportService = oqtPrerenderSupportService;
        }

        private PageState _pageState;
        private ModuleBase.Logger _logger;

        public IOqtPrerenderService Init(PageState pageState, ModuleBase.Logger logger)
        {
            _pageState = pageState;
            _logger = logger;
            return this;
        }

        public string GetSystemHtml()
        {
            try
            {
                if (_oqtPrerenderSupportService.Executed) return string.Empty;
                if (!PrerenderingEnabled()) return string.Empty;
                if (!_oqtPrerenderSupportService.HasUserAgentSignature() && !HasQueryString()) return string.Empty;
                _oqtPrerenderSupportService.Executed = true;
                return SystemHtml();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "PrerenderService");
                return string.Empty;
            }
        }

        private string SystemHtml() => $"<style> {(OqtaneVersion >= new Version(3, 0, 2) ? "body" : "app")} > div:first-of-type {{ display: block !important }} </style>";

        private Version OqtaneVersion => _oqtaneVersion ??= GetOqtaneVersion();
        private Version _oqtaneVersion;

        private static Version GetOqtaneVersion()
        {
            return Version.TryParse(Oqtane.Shared.Constants.Version, out var ver) ? ver : new Version(1, 0);
        }
        public bool PrerenderingEnabled() => _pageState.Site.RenderMode is "ServerPrerendered" or "WebAssemblyPrerendered"; // The render mode for the site.; // The render mode for the site.

        // used for testing, just add to page url in query string ("?prerender")
        private bool HasQueryString() => _pageState.QueryString.ContainsKey(QueryStringKey);
        private const string QueryStringKey = "prerender";
    }
}
