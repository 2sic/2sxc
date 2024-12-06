using DotNetNuke.Entities.Portals;
using DotNetNuke.Framework;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using ToSic.Eav.Security.Encryption;
using ToSic.Lib.Services;
using ToSic.Sxc.Context;
using ToSic.Sxc.Web.Internal.JsContext;

namespace ToSic.Sxc.Dnn.Web;

internal class DnnJsApiService(JsApiCacheService jsApiCache, RsaCryptographyService rsaCryptographyService)
    : ServiceBase("DnnJsAPi", connect: [jsApiCache, rsaCryptographyService]), IJsApiService
{
    public const string PortalIdParamName = "portalId";

    public string GetJsApiJson(int? pageId = null, string siteRoot = null, string rvt = null, bool withPublicKey = false) 
        => JsApi.JsApiJson(GetJsApi(pageId, siteRoot, rvt, withPublicKey: withPublicKey));

    public JsApi GetJsApi(int? pageId, string siteRoot, string rvt, bool withPublicKey)
    {
        // pageId and siteRoot are normally null when called from razor, api, custom cs
        // pageId and siteRoot are provided only in very special case for EditUI in /DesktopModules/.../...aspx

        return jsApiCache.JsApiJson(
            platform: PlatformType.Dnn.ToString(),
            pageId: pageId ?? PortalSettings.Current.ActiveTab.TabID,
            siteRoot: SiteRootFn,
            apiRoot: () => GetApiRoots(SiteRootFn()).SiteApiRoot,
            appApiRoot: () => GetApiRoots(SiteRootFn()).AppApiRoot,
            uiRoot: () => VirtualPathUtility.ToAbsolute(DnnConstants.SysFolderRootVirtual),
            rvtHeader: DnnConstants.AntiForgeryTokenHeaderName,
            rvt: AntiForgeryToken,
            withPublicKey: withPublicKey,
            secureEndpointPublicKey: () => rsaCryptographyService.PublicKey,
            dialogQuery: $"{PortalIdParamName}={PortalSettings.Current.PortalId}"
        );

        string SiteRootFn() => siteRoot ?? ServicesFramework.GetServiceFrameworkRoot();
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