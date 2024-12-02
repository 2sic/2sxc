using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Caching;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Helpers;
using ToSic.Eav.Integration.Environment;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Dnn.Web;
using ToSic.Sxc.Web.Internal.EditUi;
using ToSic.Sxc.Web.Internal.JsContext;
using static System.StringComparison;

namespace ToSic.Sxc.Dnn.dist;

public class CachedPageBase : CDefault // HACK: inherits dnn default.aspx to preserve correct language cookie
{
    private const int UnknownSiteId = -1;
    private const int UnknownPageId = -1;

    #region GetService and Service Provider

    /// <summary>
    /// Get the service provider only once - ideally in Dnn9.4 we will get it from Dnn
    /// If we would get it multiple times, there are edge cases where it could be different each time! #2614
    /// </summary>
    private IServiceProvider ServiceProvider => _serviceProvider.Get(Log, DnnStaticDi.CreateModuleScopedServiceProvider);
    private readonly GetOnce<IServiceProvider> _serviceProvider = new();
    private TService GetService<TService>() => ServiceProvider.Build<TService>(Log);

    #endregion

    #region Logging

    private ILog Log { get; } = new Log("Sxc.Dnn.CachedPageBase");
    private IEnvironmentLogger EnvLogger => _envLogger.Get(Log, GetService<IEnvironmentLogger>);
    private readonly GetOnce<IEnvironmentLogger> _envLogger = new();
    #endregion

    protected string PageOutputCached(string virtualPath, EditUiResourceSettings settings)
    {
        // add to insights-history for analytic
        GetService<ILogStore>().Add("edit-dialog", Log);

        var l = Log.Fn<string>($"{nameof(virtualPath)}: {virtualPath}");

        var html = GetPageHtml(virtualPath);
        l.A($"html: {html.Length} chars");

        var siteId = GetSiteId();
        var addOn = $"&{DnnJsApiService.PortalIdParamName}={siteId}";

        var pageId = GetPageId();
        var siteRoot = GetSiteRoot(pageId, siteId);

        var sp = HttpContext.Current.GetScope().ServiceProvider;
        var editUiResources = sp.GetService<EditUiResources>();
        var assets = editUiResources.GetResources(true, siteId, settings);
        l.A($"customHeaders: {assets.HtmlHead}");

        var dnnJsApi = sp.GetService<IJsApiService>();
        var content = dnnJsApi.GetJsApiJson(pageId: pageId, siteRoot: siteRoot, rvt: null, withPublicKey: WithPublicKey());
        l.A($"JsApiJson: {content}");

        return l.ReturnAsOk(HtmlDialog.UpdatePlaceholders(html, content, pageId, addOn, assets.HtmlHead, ""));
    }

    private string GetPageHtml(string virtualPath)
    {
        var l = Log.Fn<string>($"{nameof(virtualPath)}: {virtualPath}");

        var key = CacheKey(virtualPath);
        if (Cache.Get(key) is string html) return l.Return(html,"ok, from cache");

        l.A($"not in cache (key:{key})");

        var path = GetPath(virtualPath);
        l.A($"path to file: {path}");

        html = File.ReadAllText(path);
        l.A($"read file: {html.Length} chars");

        html = HtmlDialog.CleanImport(html);
        l.A($"html adjusted: {html.Length} chars");

        Cache.Insert(key, html, new CacheDependency(path));
        return l.Return(html, "ok, added to cache with cache dependency on file");
    }

    private static string CacheKey(string virtualPath) => $"2sxc-edit-ui-page-{virtualPath}";

    private string GetPath(string virtualPath)
    {
        var l = Log.Fn<string>($"{nameof(virtualPath)}: {virtualPath}");
        var path = Server.MapPath(virtualPath);
        if (!File.Exists(path)) throw l.Ex(new Exception("File not found: " + path));
        return l.ReturnAsOk(path);
    }

    private int GetSiteId()
    {
        var l = Log.Fn<int>();

        // portalId should be provided in query string (because of DNN special handling of aspx pages in DesktopModules)
        var portalIdString = Request.QueryString[DnnJsApiService.PortalIdParamName];
        l.A($"{DnnJsApiService.PortalIdParamName} from query string: {portalIdString}");

        var siteId = portalIdString.HasValue() ? Convert.ToInt32(portalIdString) : UnknownSiteId;
        l.A($"{(siteId == UnknownSiteId ? "unknown" : "")} siteId: {siteId}");

        return l.ReturnAsOk(siteId);
    }

    private int GetPageId()
    {
        var l = Log.Fn<int>();

        // pageId should be provided in query string
        var pageIdString = Request.QueryString[HtmlDialog.PageIdInUrl];
        l.A($"{HtmlDialog.PageIdInUrl} from query string: {pageIdString}");

        var pageId = pageIdString.HasValue() ? Convert.ToInt32(pageIdString) : UnknownPageId;
        l.A($"{(pageId == UnknownPageId ? "unknown" : "")} pageId: {pageId}");

        return l.ReturnAsOk(pageId);
    }

    private bool WithPublicKey()
    {
        var l = Log.Fn<bool>();

        // 'wpk' should be provided in query string
        var withPublicKeyString = Request.QueryString[HtmlDialog.WithPublicKey];
        l.A($"{HtmlDialog.WithPublicKey} from query string: {withPublicKeyString}");

        var withPublicKey = withPublicKeyString.HasValue() && Convert.ToBoolean(withPublicKeyString);

        return l.ReturnAsOk(withPublicKey);
    }

    /// <summary>
    /// portalId and pageId context is lost on DesktopModules/ToSIC_SexyContent/dist/...aspx
    /// and DNN Framework can not resolve site root, so we need to handle it by ourselves
    /// </summary>
    /// <param name="pageId"></param>
    /// <param name="portalId"></param>
    /// <returns></returns>
    private string GetSiteRoot(int pageId, int portalId)
    {
        var l = Log.Fn<string>($"{nameof(pageId)}: {pageId}, {nameof(portalId)}: {portalId}");
        
        try
        {
            // this is fallback
            if (pageId == UnknownPageId) 
                return l.ReturnAsError(ServicesFramework.GetServiceFrameworkRoot(), $"fallback, because of unknown {nameof(pageId)}");
            
            if (portalId == UnknownSiteId)
            {
                l.A($"fallback, unknown portalId, trying to get it from {nameof(pageId)}");
                portalId = PortalController.GetPortalDictionary()[pageId];
                l.A($"{nameof(portalId)}: {portalId}");
            }

            var primaryPortalAlias = GetPrimaryPortalAliasBasedOnRequestUrlAndCulture(portalId);
            l.A($"primaryPortalAlias: {primaryPortalAlias?.HTTPAlias}");

            string siteRoot;
            if (primaryPortalAlias != null)
            {
                siteRoot = CleanLeadingPartSiteRoot(primaryPortalAlias.HTTPAlias);
            }
            else
            {
                siteRoot = ServicesFramework.GetServiceFrameworkRoot();
                l.A("portalAlias is null, falling back to ServicesFramework.GetServiceFrameworkRoot()");
            }
            l.A($"siteRoot: {siteRoot}");

            if (string.IsNullOrEmpty(siteRoot))
            {
                siteRoot = "/";
                l.A("siteRoot is empty, falling back to /");
            }
            return l.ReturnAsOk(siteRoot);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            EnvLogger.LogException(ex);
            // if all breaks, falling back to a default value
            return l.ReturnAsError(ServicesFramework.GetServiceFrameworkRoot(), "error, falling back to a default value");
        }
    }

    private PortalAliasInfo GetPrimaryPortalAliasBasedOnRequestUrlAndCulture(int portalId)
    {
        var l = Log.Fn<PortalAliasInfo>($"{nameof(portalId)}: {portalId}");

        //var cultureCode = LocaleController.Instance.GetCurrentLocale(portalId).Code;
        var cultureCode = Thread.CurrentThread.CurrentCulture.ToString();
        l.A($"cultureCode: {cultureCode}");

        // Get all aliases for the portal
        var aliases = PortalAliasController.Instance
            .GetPortalAliasesByPortalId(portalId)
            .ToList();
        l.A($"aliases: {aliases.Count}");

        // Figure out the correct alias based on the current URL and culture
        // Should also fall back to correct primary if the current one is not found
        var currentUrl = HttpContext.Current.Request.Url.ToString();
        l.A($"figure out the correct alias based on the current URL:{currentUrl} and culture:{cultureCode}.");

        // try to filter aliases on the current url
        var aliases2 = aliases.Where(a => currentUrl.IndexOf(a.HTTPAlias, OrdinalIgnoreCase) >= 0).ToList();
        if (!aliases2.Any())
        {
            l.A("list of aliases filtered by current url is empty, falling back to filter without current URL");
            aliases2 = aliases; // falling back to filter without current URL
        }
        aliases2.ForEach(a => l.A($"alias: {a.HTTPAlias}, isPrimary: {a.IsPrimary}, culture: {a.CultureCode}"));

        var primaryPortalAlias = aliases2
            .Where(a => string.Compare(a.CultureCode, cultureCode, OrdinalIgnoreCase) == 0 || string.IsNullOrEmpty(a.CultureCode))
            .OrderByDescending(a => a.IsPrimary)
            .ThenByDescending(a => a.CultureCode)
            .FirstOrDefault();
        l.A($"primaryPortalAlias: {primaryPortalAlias?.HTTPAlias} based on culture:{cultureCode}");

        if (primaryPortalAlias == null)
        {
            l.A("primaryPortalAlias based on culture is null, falling back to the first primary alias");
            // fallback to the first primary first
            primaryPortalAlias = aliases.FirstOrDefault(a => a.IsPrimary);
            l.A($"primaryPortalAlias: {primaryPortalAlias?.HTTPAlias}");
        }

        if (primaryPortalAlias == null)
        {
            l.A("primaryPortalAlias is null, falling back to the first alias");
            // and only if this doesn't exist for random reasons, fallback to first alias
            primaryPortalAlias = aliases.FirstOrDefault();
            l.A($"portalAlias: {primaryPortalAlias?.HTTPAlias}");
        }

        return l.ReturnAsOk(primaryPortalAlias);
    }

    private string CleanLeadingPartSiteRoot(string path)
    {
        var l = Log.Fn<string>($"{nameof(path)}:{path}");
        var index = path.IndexOf('/');
        l.A($"position of /: {index}");
        return l.ReturnAsOk(index <= 0 ? "/" : path.Substring(index).SuffixSlash());
    }
}