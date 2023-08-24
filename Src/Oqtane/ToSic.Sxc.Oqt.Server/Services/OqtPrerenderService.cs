using Microsoft.AspNetCore.Http;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Services
{
    public class OqtPrerenderService : ServiceBase, IOqtPrerenderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IThemeRepository _themes;

        public OqtPrerenderService(IHttpContextAccessor httpContextAccessor, IThemeRepository themes) : base($"{Constants.SxcLogName}.OqtPrerndSrv")
        {
            _httpContextAccessor = httpContextAccessor;
            _themes = themes;
        }

        public string GetPrerenderHtml(bool isPrerendered, OqtViewResultsDto viewResults, SiteState siteState, string themeType)
        {
            try
            {
                if (!isPrerendered || !UsePrerender) return string.Empty;

                var prerenderHtmlFragment = string.Empty;
                prerenderHtmlFragment = ManageStyleSheets(prerenderHtmlFragment, viewResults, siteState.Alias, themeType);
                prerenderHtmlFragment = ManageScripts(prerenderHtmlFragment, viewResults, siteState.Alias);
                prerenderHtmlFragment = SystemHtml(prerenderHtmlFragment);
                Html += prerenderHtmlFragment;
                return prerenderHtmlFragment;
            }
            catch (Exception e)
            {
                Log.Ex(e);
                return string.Empty;
            }
        }

        #region Validation
        public bool UsePrerender => _usePrerender ??= (HasUserAgentSignature() || CheckForKeyInQueryString("prerender"));
        private bool? _usePrerender;

        private bool HasUserAgentSignature()
        {
            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"];
            return userAgent.HasValue && _userAgentSignatures.Exists(x => userAgent.Value.ToString().Contains(x, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool CheckForKeyInQueryString(string key)
        {
            var queryCollection = _httpContextAccessor.HttpContext?.Request.Query;
            return queryCollection != null && queryCollection.ContainsKey(key);
        }

        // based on https://raw.githubusercontent.com/monperrus/crawler-user-agents/master/crawler-user-agents.json
        private readonly List<string> _userAgentSignatures = new() // used for production
        {
            // ReSharper disable StringLiteralTypo
            @"Googlebot",
            @"AdsBot-Google",
            @"Feedfetcher-Google",
            @"Mediapartners",
            @"APIs-Google",
            @"bingbot",
            @"Slurp",
            @"wget",
            @"LinkedInBot",
            @"Python-urllib",
            @"python-requests",
            @"aiohttp",
            @"httpx",
            @"libwww-perl",
            @"httpunit",
            @"nutch",
            @"Go-http-client",
            @"phpcrawl",
            @"msnbot",
            @"jyxobot",
            @"FAST-WebCrawler",
            @"FAST Enterprise Crawler",
            @"BIGLOTRON",
            @"Teoma",
            @"convera",
            @"seekbot",
            @"Gigabot",
            @"Gigablast",
            @"exabot",
            @"ia_archiver",
            @"GingerCrawler",
            @"webmon",
            @"HTTrack",
            @"grub.org",
            @"UsineNouvelleCrawler",
            @"antibot",
            @"netresearchserver",
            @"speedy",
            @"fluffy",
            @"findlink",
            @"msrbot",
            @"panscient",
            @"yacybot",
            @"AISearchBot",
            @"ips-agent",
            @"tagoobot",
            @"MJ12bot",
            @"woriobot",
            @"yanga",
            @"buzzbot",
            @"mlbot",
            @"YandexBot",
            @"YandexImages",
            @"YandexAccessibilityBot",
            @"YandexMobileBot",
            @"YandexMetrika",
            @"YandexTurbo",
            @"YandexImageResizer",
            @"YandexVideo",
            @"YandexAdNet",
            @"YandexBlogs",
            @"YandexCalendar",
            @"YandexDirect",
            @"YandexFavicons",
            @"YaDirectFetcher",
            @"YandexForDomain",
            @"YandexMarket",
            @"YandexMedia",
            @"YandexMobileScreenShotBot",
            @"YandexNews",
            @"YandexOntoDB",
            @"YandexPagechecker",
            @"YandexPartner",
            @"YandexRCA",
            @"YandexSearchShop",
            @"YandexSitelinks",
            @"YandexSpravBot",
            @"YandexTracker",
            @"YandexVertis",
            @"YandexVerticals",
            @"YandexWebmaster",
            @"YandexScreenshotBot",
            @"purebot",
            @"Linguee Bot",
            @"CyberPatrol",
            @"voilabot",
            @"Baiduspider",
            @"citeseerxbot",
            @"spbot",
            @"twengabot",
            @"postrank",
            @"TurnitinBot",
            @"scribdbot",
            @"page2rss",
            @"sitebot",
            @"linkdex",
            @"Adidxbot",
            @"ezooms",
            @"dotbot",
            @"Mail.RU_Bot",
            @"discobot",
            @"heritrix",
            @"findthatfile",
            @"europarchive.org",
            @"NerdByNature.Bot",
            @"sistrix crawler",
            @"Ahrefs",
            @"fuelbot",
            @"CrunchBot",
            @"IndeedBot",
            @"mappydata",
            @"woobot",
            @"ZoominfoBot",
            @"PrivacyAwareBot",
            @"Multiviewbot",
            @"SWIMGBot",
            @"Grobbot",
            @"eright",
            @"Apercite",
            @"semanticbot",
            @"Aboundex",
            @"domaincrawler",
            @"wbsearchbot",
            @"summify",
            @"CCBot",
            @"edisterbot",
            @"seznambot",
            @"ec2linkfinder",
            @"gslfbot",
            @"aiHitBot",
            @"intelium_bot",
            @"facebookexternalhit",
            @"Yeti",
            @"RetrevoPageAnalyzer",
            @"lb-spider",
            @"Sogou",
            @"lssbot",
            @"careerbot",
            @"wotbox",
            @"wocbot",
            @"ichiro",
            @"DuckDuckBot",
            @"lssrocketcrawler",
            @"drupact",
            @"webcompanycrawler",
            @"acoonbot",
            @"openindexspider",
            @"gnam gnam spider",
            @"web-archive-net.com.bot",
            @"backlinkcrawler",
            @"coccoc",
            @"integromedb",
            @"content crawler spider",
            @"toplistbot",
            @"it2media-domain-crawler",
            @"ip-web-crawler.com",
            @"siteexplorer.info",
            @"elisabot",
            @"proximic",
            @"changedetection",
            @"arabot",
            @"WeSEE:Search",
            @"niki-bot",
            @"CrystalSemanticsBot",
            @"rogerbot",
            @"360Spider",
            @"psbot",
            @"InterfaxScanBot",
            @"CC Metadata Scaper",
            @"g00g1e.net",
            @"GrapeshotCrawler",
            @"urlappendbot",
            @"brainobot",
            @"fr-crawler",
            @"binlar",
            @"SimpleCrawler",
            @"Twitterbot",
            @"cXensebot",
            @"smtbot",
            @"bnf.fr_bot",
            @"A6-Indexer",
            @"ADmantX",
            @"Facebot",
            @"OrangeBot",
            @"memorybot",
            @"AdvBot",
            @"MegaIndex",
            @"SemanticScholarBot",
            @"ltx71",
            @"nerdybot",
            @"xovibot",
            @"BUbiNG",
            @"Qwantify",
            @"archive.org_bot",
            @"Applebot",
            @"TweetmemeBot",
            @"crawler4j",
            @"findxbot",
            @"SemrushBot",
            @"yoozBot",
            @"lipperhey",
            @"Y!J",
            @"Domain Re-Animator Bot",
            @"AddThis",
            @"Screaming Frog SEO Spider",
            @"MetaURI",
            @"Scrapy",
            @"Livelapbot",
            @"OpenHoseBot",
            @"CapsuleChecker",
            @"collection@infegy.com",
            @"IstellaBot",
            @"DeuSu",
            @"betaBot",
            @"Cliqzbot",
            @"MojeekBot",
            @"netEstate NE Crawler",
            @"SafeSearch microdata crawler",
            @"Gluten Free Crawler",
            @"Sonic",
            @"Sysomos",
            @"Trove",
            @"deadlinkchecker",
            @"Slack-ImgProxy",
            @"Embedly",
            @"RankActiveLinkBot",
            @"iskanie",
            @"SafeDNSBot",
            @"SkypeUriPreview",
            @"Veoozbot",
            @"Slackbot",
            @"redditbot",
            @"datagnionbot",
            @"Google-Adwords-Instant",
            @"adbeat_bot",
            @"WhatsApp",
            @"contxbot",
            @"pinterest.com.bot",
            @"electricmonk",
            @"GarlikCrawler",
            @"BingPreview",
            @"vebidoobot",
            @"FemtosearchBot",
            @"Yahoo Link Preview",
            @"MetaJobBot",
            @"DomainStatsBot",
            @"mindUpBot",
            @"Daum",
            @"Jugendschutzprogramm-Crawler",
            @"Xenu Link Sleuth",
            @"Pcore-HTTP",
            @"moatbot",
            @"KosmioBot",
            @"pingdom",
            @"AppInsights",
            @"PhantomJS",
            @"Gowikibot",
            @"PiplBot",
            @"Discordbot",
            @"TelegramBot",
            @"Jetslide",
            @"newsharecounts",
            @"James BOT",
            @"Barkrowler",
            @"TinEye",
            @"SocialRankIOBot",
            @"trendictionbot",
            @"Ocarinabot",
            @"epicbot",
            @"Primalbot",
            @"DuckDuckGo-Favicons-Bot",
            @"GnowitNewsbot",
            @"Leikibot",
            @"LinkArchiver",
            @"YaK",
            @"PaperLiBot",
            @"Digg Deeper",
            @"dcrawl",
            @"Snacktory",
            @"AndersPinkBot",
            @"Fyrebot",
            @"EveryoneSocialBot",
            @"Mediatoolkitbot",
            @"Luminator-robots",
            @"ExtLinksBot",
            @"SurveyBot",
            @"NING",
            @"okhttp",
            @"Nuzzel",
            @"omgili",
            @"PocketParser",
            @"YisouSpider",
            @"um-LN",
            @"ToutiaoSpider",
            @"MuckRack",
            @"Jamie's Spider",
            @"AHC",
            @"NetcraftSurveyAgent",
            @"Laserlikebot",
            @"Apache-HttpClient",
            @"AppEngine-Google",
            @"Jetty",
            @"Upflow",
            @"Thinklab",
            @"Traackr.com",
            @"Twurly",
            @"Mastodon",
            @"http_get",
            @"DnyzBot",
            @"botify",
            @"007ac9 Crawler",
            @"BehloolBot",
            @"BrandVerity",
            @"check_http",
            @"BDCbot",
            @"ZumBot",
            @"EZID",
            @"ICC-Crawler",
            @"ArchiveBot",
            @"LCC",
            @"filterdb.iss.netcrawler",
            @"BLP_bbot",
            @"BomboraBot",
            @"Buck",
            @"Companybook-Crawler",
            @"Genieo",
            @"magpie-crawler",
            @"MeltwaterNews",
            @"Moreover",
            @"newspaper",
            @"ScoutJet",
            @"sentry",
            @"StorygizeBot",
            @"UptimeRobot",
            @"OutclicksBot",
            @"seoscanners",
            @"Hatena",
            @"Google Web Preview",
            @"MauiBot",
            @"AlphaBot",
            @"SBL-BOT",
            @"IAS crawler",
            @"adscanner",
            @"Netvibes",
            @"acapbot",
            @"Baidu-YunGuanCe",
            @"bitlybot",
            @"blogmuraBot",
            @"Bot.AraTurka.com",
            @"bot-pge.chlooe.com",
            @"BoxcarBot",
            @"BTWebClient",
            @"ContextAd Bot",
            @"Digincore bot",
            @"Disqus",
            @"Feedly",
            @"Fetch",
            @"Fever",
            @"Flamingo_SearchEngine",
            @"FlipboardProxy",
            @"g2reader-bot",
            @"G2 Web Services",
            @"imrbot",
            @"K7MLWCBot",
            @"Kemvibot",
            @"Landau-Media-Spider",
            @"linkapediabot",
            @"vkShare",
            @"Siteimprove.com",
            @"BLEXBot",
            @"DareBoost",
            @"ZuperlistBot",
            @"Miniflux",
            @"Feedspot",
            @"Diffbot",
            @"SEOkicks",
            @"tracemyfile",
            @"Nimbostratus-Bot",
            @"zgrab",
            @"PR-CY.RU",
            @"AdsTxtCrawler",
            @"Datafeedwatch",
            @"Zabbix",
            @"TangibleeBot",
            @"google-xrawler",
            @"axios",
            @"Amazon CloudFront",
            @"Pulsepoint",
            @"CloudFlare-AlwaysOnline",
            @"Google-Structured-Data-Testing-Tool",
            @"WordupInfoSearch",
            @"WebDataStats",
            @"HttpUrlConnection",
            @"Seekport Crawler",
            @"ZoomBot",
            @"VelenPublicWebCrawler",
            @"MoodleBot",
            @"jpg-newsbot",
            @"outbrain",
            @"W3C_Validator",
            @"Validator.nu",
            @"W3C-checklink",
            @"W3C-mobileOK",
            @"W3C_I18n-Checker",
            @"FeedValidator",
            @"W3C_CSS_Validator",
            @"W3C_Unicorn",
            @"Google-PhysicalWeb",
            @"Blackboard",
            @"ICBot",
            @"BazQux",
            @"Twingly",
            @"Rivva",
            @"Experibot",
            @"awesomecrawler",
            @"Dataprovider.com",
            @"GroupHigh",
            @"theoldreader.com",
            @"AnyEvent",
            @"Uptimebot.org",
            @"Nmap Scripting Engine",
            @"2ip.ru",
            @"Clickagy",
            @"Caliperbot",
            @"MBCrawler",
            @"online-webceo-bot",
            @"B2B Bot",
            @"AddSearchBot",
            @"Google Favicon",
            @"HubSpot",
            @"Chrome-Lighthouse",
            @"HeadlessChrome",
            @"CheckMarkNetwork",
            @"www.uptime.com",
            @"Streamline3Bot",
            @"serpstatbot",
            @"MixnodeCache",
            @"curl",
            @"SimpleScraper",
            @"RSSingBot",
            @"Jooblebot",
            @"fedoraplanet",
            @"Friendica",
            @"NextCloud",
            @"Tiny Tiny RSS",
            @"RegionStuttgartBot",
            @"Bytespider",
            @"Datanyze",
            @"Google-Site-Verification",
            @"TrendsmapResolver",
            @"tweetedtimes",
            @"NTENTbot",
            @"Gwene",
            @"SimplePie",
            @"SearchAtlas",
            @"Superfeedr",
            @"feedbot",
            @"UT-Dorkbot",
            @"Amazonbot",
            @"SerendeputyBot",
            @"Eyeotabot",
            @"officestorebot",
            @"Neticle Crawler",
            @"SurdotlyBot",
            @"LinkisBot",
            @"AwarioSmartBot",
            @"AwarioRssBot",
            @"RyteBot",
            @"FreeWebMonitoring SiteChecker",
            @"AspiegelBot",
            @"NAVER Blog Rssbot",
            @"zenback bot",
            @"SentiBot",
            @"Domains Project",
            @"Pandalytics",
            @"VKRobot",
            @"bidswitchbot",
            @"tigerbot",
            @"NIXStatsbot",
            @"Atom Feed Robot",
            @"Curebot",
            @"PagePeeker",
            @"Vigil",
            @"rssbot",
            @"startmebot",
            @"JobboerseBot",
            @"seewithkids",
            @"NINJA bot",
            @"Cutbot",
            @"BublupBot",
            @"BrandONbot",
            @"RidderBot",
            @"Taboolabot",
            @"Dubbotbot",
            @"FindITAnswersbot",
            @"infoobot",
            @"Refindbot",
            @"BlogTraffic",
            @"SeobilityBot",
            @"Cincraw",
            @"Dragonbot",
            @"VoluumDSP-content-bot",
            @"FreshRSS",
            @"BitBot",
            @"PHP-Curl-Class",
            @"Google-Certificates-Bridge",
            @"centurybot",
            @"Viber",
            @"e.ventures Investment Crawler",
            @"evc-batch",
            @"PetalBot",
            @"virustotal",
            @"PTST",
            @"minicrawler",
            // ReSharper restore StringLiteralTypo
        }; 
        #endregion

        #region StyleSheets
        private string ManageStyleSheets(string html, OqtViewResultsDto viewResults, Alias alias, string themeType)
        {
            if (viewResults == null) return html;

            var external = viewResults.TemplateResources
                .Where(r => r.IsExternal && r.ResourceType == ResourceType.Stylesheet)
                .Select(r => r.Url)
                .ToList();

            var count = 0;
            foreach (var styleSheet in viewResults.SxcStyles.Union(external))
            {
                var url = styleSheet;

                if (url.StartsWith("~"))
                    url = url.Replace("~", "/Themes/" + Theme(themeType).Name + "/").Replace("//", "/");

                if (!url.Contains("://") && alias.BaseUrl != "" && !url.StartsWith(alias.BaseUrl))
                    url = alias.BaseUrl + url;

                if (!html.Contains(url, StringComparison.OrdinalIgnoreCase) && !Html.Contains(url, StringComparison.OrdinalIgnoreCase))
                {
                    count++;
                    var id = "id=\"app-stylesheet-" + ResourceLevel.Page.ToString().ToLower() + "-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff") + "-" + count.ToString("00") + "\" ";
                    html += "<link " + id + "rel=\"stylesheet\" href=\"" + url + "\" type=\"text/css\"/>" + Environment.NewLine;
                }
            }

            return html;
        }

        private Theme Theme(string themeType) => _theme ??= _themes.GetThemes().FirstOrDefault(item => item.Themes.Any(item => item.TypeName == themeType));
        private Theme _theme;
        #endregion

        #region Scripts
        private string ManageScripts(string html, OqtViewResultsDto viewResults, Alias alias)
        {
            if (viewResults == null) return html;
            if (string.IsNullOrEmpty(html)) html = string.Empty;

            var externalScripts = viewResults.TemplateResources
                .Where(r => r.IsExternal && r.ResourceType == ResourceType.Script)
                .Select(r => r.Url)
                .ToList();

            foreach (var external in viewResults.SxcScripts.Union(externalScripts))
                html = AddExternalScript(html, external, alias);

            foreach (var inline in viewResults.TemplateResources.Where(r => !r.IsExternal))
                html = AddInlineScript(html, inline.Content);

            return html;
        }

        private string AddExternalScript(string html, string src, Alias alias)
        {
            if (string.IsNullOrEmpty(src)) return html;
            var script = "<script src=\"" + ((src.Contains("://")) ? src : alias.BaseUrl + src) + "\"" + "></script>";
            if (!html.Contains(script, StringComparison.OrdinalIgnoreCase) && !Html.Contains(script, StringComparison.OrdinalIgnoreCase)) html += script + Environment.NewLine;
            return html;
        }

        private string AddInlineScript(string html, string content)
        {
            if (string.IsNullOrEmpty(content)) return html;
            var script = $"<script>{content}</script>";
            if (!html.Contains(script, StringComparison.OrdinalIgnoreCase) && !Html.Contains(script, StringComparison.OrdinalIgnoreCase)) html += script + Environment.NewLine;
            return html;
        }

        #endregion

        #region SystemHtml
        private string SystemHtml(string html)
        {
            if (Executed) return html;
            var systemHtml =
                $"<style> {(OqtaneVersion >= new Version(3, 0, 2) ? "body" : "app")} > div:first-of-type {{ display: block !important }} </style>";
            if (!html.Contains(systemHtml, StringComparison.OrdinalIgnoreCase) && !Html.Contains(systemHtml, StringComparison.OrdinalIgnoreCase))
                html += systemHtml + Environment.NewLine;
            Executed = true;
            return html;
        }

        private Version OqtaneVersion => _oqtaneVersion ??= GetOqtaneVersion();
        private Version _oqtaneVersion;

        private static Version GetOqtaneVersion()
            => Version.TryParse(Oqtane.Shared.Constants.Version, out var ver) ? ver : new Version(1, 0);

        private bool Executed // for execution once per request
        {
            get => (_httpContextAccessor?.HttpContext?.Items[ExecutedKey] as bool?) ?? false;
            set
            {
                if (_httpContextAccessor?.HttpContext != null)
                    _httpContextAccessor.HttpContext.Items[ExecutedKey] = value;
            }
        }
        private const string ExecutedKey = "PrerenderServiceExecuted";
        #endregion

        #region Html
        private string Html // for execution once per request
        {
            get => (_httpContextAccessor?.HttpContext?.Items[PrerenderHtmlFragmentKey] as string) ?? string.Empty;
            set
            {
                if (_httpContextAccessor?.HttpContext != null)
                    _httpContextAccessor.HttpContext.Items[PrerenderHtmlFragmentKey] = value;
            }
        }
        private const string PrerenderHtmlFragmentKey = "PrerenderHtmlFragment"; 
        #endregion
    }
}
