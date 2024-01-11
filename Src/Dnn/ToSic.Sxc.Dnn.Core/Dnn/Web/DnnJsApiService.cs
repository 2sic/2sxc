using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;
using ToSic.Sxc.Web.Internal.JsContext;

namespace ToSic.Sxc.Dnn.Web;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DnnJsApiService : ServiceBase, IJsApiService
{
    public const string PortalIdParamName = "portalId";

    public DnnJsApiService(JsApiCacheService jsApiCache) : base("DnnJsAPi")
    {
        ConnectServices(_jsApiCache = jsApiCache);
    }

    private readonly JsApiCacheService _jsApiCache;

    public string GetJsApiJson(int? pageId = null, string siteRoot = null, string rvt = null) 
        => JsApi.JsApiJson(GetJsApi(pageId, siteRoot, rvt));

    public JsApi GetJsApi(int? pageId = null, string siteRoot = null, string rvt = null)
    {
        // pageId and siteRoot are normally null when called from razor, api, custom cs
        // pageId and siteRoot are provided only in very special case for EditUI in /DesktopModules/.../...aspx

        string SiteRootFn() => siteRoot ?? ServicesFramework.GetServiceFrameworkRoot();

        return _jsApiCache.JsApiJson(
            platform: PlatformType.Dnn.ToString(),
            pageId: pageId ?? PortalSettings.Current.ActiveTab.TabID,
            siteRoot: SiteRootFn,
            apiRoot: () => GetApiRoots(SiteRootFn()).SiteApiRoot,
            appApiRoot: () => GetApiRoots(SiteRootFn()).AppApiRoot,
            uiRoot: () => VirtualPathUtility.ToAbsolute(DnnConstants.SysFolderRootVirtual),
            rvtHeader: DnnConstants.AntiForgeryTokenHeaderName,
            rvt: AntiForgeryToken,
            dialogQuery: $"{PortalIdParamName}={PortalSettings.Current.PortalId}"
        );
    }

    internal static (string SiteApiRoot, string AppApiRoot) GetApiRoots(string siteRoot = null)
    {
        siteRoot = siteRoot ?? ServicesFramework.GetServiceFrameworkRoot();
        var apiRoot = siteRoot + $"api/{JsApi.ExtensionPlaceholder}/";
            
        // appApiRoot is the same as apiRoot - the UI will add "app" to it later on 
        // but app-api root shouldn't contain generic modules-name, as it's always 2sxc
        var appApiRoot = apiRoot;
        appApiRoot = appApiRoot.Replace(JsApi.ExtensionPlaceholder, "2sxc");

        return (apiRoot, appApiRoot);
    }

    private static string AntiForgeryToken()
    {
        var tag = AntiForgery.GetHtml().ToString();
        return GetAttribute(tag, "value");
    }

    private static string GetAttribute(string tag, string attribute)
    {
        return new Regex(@"(?<=\b" + attribute + @"="")[^""]*")
            .Match(tag).Value;
    }
}