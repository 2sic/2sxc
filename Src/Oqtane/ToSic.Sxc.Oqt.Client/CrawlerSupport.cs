using Microsoft.AspNetCore.Http;
using Oqtane.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Oqtane.Modules;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Client
{
    public class CrawlerSupport
    {
        //private const string CssToEnableCrawlerSupport = "<style> app > div { display: block !important } </style>";
        //private const string CssToEnableCrawlerSupport = "<style> body div:first-of-type { display: block !important } </style>";
        private const string CssToEnableCrawlerSupport = "<style> div[style*=\"display: none;\"] { display: block !important } </style>";

        private const string CrawlerQueryStringKey = "crawler"; // used for testing, just add to page url in query string ("?crawler")

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public readonly List<string> CrawlerUserAgentSignatures = new() // used in production to detect crawler bots with unique part of user agent string in http header
        {
            "googlebot","bingbot","yandexbot","ahrefsbot","msnbot","linkedinbot","exabot","compspybot",
            "yesupbot","paperlibot","tweetmemebot","semrushbot","gigabot","voilabot","adsbot-google",
            "botlink","alkalinebot","araybot","undrip bot","borg-bot","boxseabot","yodaobot","admedia bot",
            "ezooms.bot","confuzzledbot","coolbot","internet cruiser robot","yolinkbot","diibot","musobot",
            "dragonbot","elfinbot","wikiobot","twitterbot","contextad bot","hambot","iajabot","news bot",
            "irobot","socialradarbot","ko_yappo_robot","skimbot","psbot","rixbot","seznambot","careerbot",
            "simbot","solbot","mail.ru_bot","spiderbot","blekkobot","bitlybot","techbot","void-bot",
            "vwbot_k","diffbot","friendfeedbot","archive.org_bot","woriobot","crystalsemanticsbot","wepbot",
            "spbot","tweetedtimes bot","mj12bot","who.is bot","psbot","robot","jbot","bbot","bot",
            "facebookexternalhit", "facebot", "lighthouse"
        };

        private const string CrawlerSupportExecutedKey = "2sxcCrawlerSupportExecuted"; // crawler support execution is needed ounce per request

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PageState _pageState;
        private readonly ModuleBase _module;
        private readonly OqtViewResultsDto _viewResults;

        public CrawlerSupport(ModuleBase module, IHttpContextAccessor httpContextAccessor, PageState pageState, OqtViewResultsDto viewResults)
        {
            _httpContextAccessor = httpContextAccessor;
            _pageState = pageState;
            _module = module;
            _viewResults = viewResults;
        }

        public void Execute()
        {
            try
            {
                if (CrawlerSupportExecuted) return;
                if (!HasServerSidePreRendering()) return;
                if (!IsCrawlerBot() && !HasCrawlerQueryString()) return;
                _viewResults.Html = $"{CssToEnableCrawlerSupport}{Environment.NewLine}{_viewResults.Html}";
                CrawlerSupportExecuted = true;
            }
            catch (Exception e)
            {
                _module.AddModuleMessage($"Exception in crawler support: {e.Message}", MessageType.Warning);
            }
        }

        public bool HasServerSidePreRendering() => _pageState.Runtime == Oqtane.Shared.Runtime.Server /* for Oqt 2+ */;
        // TODO: STV replace after update references to Oqt 3+
        //public bool HasServerSidePreRendering() => _site.RenderMode == "ServerPrerendered"; // The render mode for the site (ie. Server, ServerPrerendered, WebAssembly, WebAssemblyPrerendered ).

        public bool CrawlerSupportExecuted
        {
            get => (_httpContextAccessor.HttpContext.Items[CrawlerSupportExecutedKey] as bool?) ?? false;
            set => _httpContextAccessor.HttpContext.Items[CrawlerSupportExecutedKey] = value;
        }

        public bool IsCrawlerBot()
        {
            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"];
            return userAgent.HasValue && CrawlerUserAgentSignatures.Exists(x => userAgent.Value.ToString().Contains(x, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool HasCrawlerQueryString() => _pageState.QueryString.ContainsKey(CrawlerQueryStringKey);
    }
}
