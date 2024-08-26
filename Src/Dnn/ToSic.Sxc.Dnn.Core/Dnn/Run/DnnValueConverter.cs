using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using System.IO;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Internal.Features;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Internal.Plumbing;

namespace ToSic.Sxc.Dnn.Run;

/// <summary>
/// The DNN implementation of the <see cref="IValueConverter"/> which converts "file:22" or "page:5" to the url,
/// </summary>
[PrivateApi("Hide implementation - not useful for external documentation")]
internal class DnnValueConverter : ValueConverterBase
{
    public const string CurrentLanguage = "current";

    #region DI Constructor

    public DnnValueConverter(ISite site, LazySvc<IEavFeaturesService> featuresLazy, LazySvc<PageScopedService<ISite>> siteFromPageLazy, LazySvc<INavigationManager> navigationManager) : base(
        $"{DnnConstants.LogName}.ValCnv")
    {
        ConnectLogs([
            _site = site,
            _featuresLazy = featuresLazy,
            _siteFromPageLazy = siteFromPageLazy,
            _navigationManager = navigationManager
        ]);
    }

    private readonly ISite _site;
    private readonly LazySvc<IEavFeaturesService> _featuresLazy;
    private readonly LazySvc<PageScopedService<ISite>> _siteFromPageLazy;
    private readonly LazySvc<INavigationManager> _navigationManager;
    private int PageSiteId => _siteFromPageLazy.Value.Value.Id; // PortalId from page di scope

    #endregion

    /// <inheritdoc />
    public override string ToReference(string value) => TryToResolveOneLinkToInternalDnnCode(value);

    /// <inheritdoc />
    public override string ToValue(string reference, Guid itemGuid = default) => TryToResolveCodeToLink(itemGuid, reference);

    /// <summary>
    /// Will take a link like http:\\... to a file or page and try to return a DNN-style info like
    /// Page:35 or File:43003
    /// </summary>
    /// <param name="potentialFilePath"></param>
    /// <remarks>
    /// note: this can always use the current context, because this should happen
    /// when saving etc. - which is always expected to happen in the owning portal
    /// </remarks>
    /// <returns></returns>
    private string TryToResolveOneLinkToInternalDnnCode(string potentialFilePath)
    {
        // Try file reference
        var fileInfo = FileManager.Instance.GetFile(_site.Id, potentialFilePath) // PortalId from module di scope
                       ?? FileManager.Instance.GetFile(PageSiteId, potentialFilePath); // PortalId from page di scope (module sharing on different site)
        if (fileInfo != null) return PrefixFile + Separator + fileInfo.FileId;

        // Try page / tab ID
        var tabController = new TabController();
        var tabInfo = tabController.GetTabsByPortal(_site.Id).Select(tab => tab.Value)
                          .FirstOrDefault(tab => tab.TabPath == potentialFilePath)// PortalId from module di scope
                      ?? tabController.GetTabsByPortal(PageSiteId).Select(tab => tab.Value)
                          .FirstOrDefault(tab => tab.TabPath == potentialFilePath);// PortalId from page di scope (module sharing on different site)

        return tabInfo != null
            ? PrefixPage + Separator + tabInfo.TabID
            : potentialFilePath;
    }
        

    protected override void LogConversionExceptions(string originalValue, Exception e)
    {
        var wrappedEx = new Exception("Error when trying to lookup a friendly url of \"" + originalValue + "\"", e);
        Exceptions.LogException(wrappedEx);
    }

    protected override string ResolveFileLink(int linkId, Guid itemGuid)
    {
        var fileInfo = FileManager.Instance.GetFile(linkId);
        if (fileInfo == null)
            return null;

        #region special handling of issues in case something in the background is broken
        // there are cases where the PortalSettings will be null or something, and in these cases the serializer would break down
        // so this is to just ensure that if it can't be converted, it'll just fall back to default
        try
        {
            var filePath = Path.Combine(new PortalSettings(fileInfo.PortalId).HomeDirectory ?? "", fileInfo.RelativePath ?? "");

            // return linkclick url for secure and other not standard folder locations
            var result = fileInfo.StorageLocation == 0 ? filePath : FileLinkClickController.Instance.GetFileLinkClick(fileInfo);

            // optionally do extra security checks (new in 10.02)
            if (!_featuresLazy.Value.IsEnabled(BuiltInFeatures.AdamRestrictLookupToEntity.Guid)) return result;

            // check if it's in this item. We won't check the field, just the item, so the field is ""
            return !Security.PathIsInItemAdam(itemGuid, "", filePath)
                ? null
                : result;
        }
        catch
        {
            return null;
        }
        #endregion
    }

    protected override string ResolvePageLink(int id) => ResolvePageLink(id, CurrentLanguage, []);

    /// <summary>
    /// Resolve URL to Page with TabId, but handles more situations than DNN framework:
    /// - supports module sharing scenarios, when module is on different portal
    /// - return localized page TabId, instead of requested TabId
    /// </summary>
    /// <param name="id">TabId to page</param>
    /// <param name="language">"current" is required to ensure expected behavior (to try to find current language localized TabId)</param>
    /// <param name="additionalParameters">query string parameters</param>
    /// <returns>return string url to page</returns>
    internal string ResolvePageLink(int id, string language = null, params string[] additionalParameters)
    {
        var tabController = new TabController();

        var tabInfo = tabController.GetTab(id, _site.Id) // PortalId from module di scope
                      ?? tabController.GetTab(id, PageSiteId); // PortalId from page di scope (module sharing on different site)
        if (tabInfo == null) return null;

        var psCurrent = PortalSettings.Current;
        var psPage = psCurrent;

        // Get full PortalSettings (with portal alias) if module sharing is active
        if (psCurrent != null && psCurrent.PortalId != tabInfo.PortalID)
            psPage = PortalSettingsForNavigateUrl(tabInfo.PortalID);

        if (psPage == null) return null;

        // try to change TabID to localized version when language == CurrentLanguage
        if (language == CurrentLanguage && tabInfo.CultureCode != "" && psCurrent != null && tabInfo.CultureCode != psCurrent.CultureCode)
        {
            var cultureTabInfo = tabController
                .GetTabByCulture(tabInfo.TabID, tabInfo.PortalID,
                    LocaleController.Instance.GetLocale(psCurrent.CultureCode));

            if (cultureTabInfo != null)
                tabInfo = cultureTabInfo;
        }

        // Exception in AdvancedURLProvider because ownerPortalSettings.PortalAlias is null
        return _navigationManager.Value.NavigateURL(tabInfo.TabID, psPage, "", additionalParameters);

    }

    private static PortalSettings PortalSettingsForNavigateUrl(int portalId)
    {
        var psPage = new PortalSettings(portalId);

        // PortalAlias is required for NavigateURL in AdvancedURLProvider,
        // but sometimes is missing (module sharing)
        if (psPage.PortalAlias == null)
            psPage.PortalAlias = PortalAliasForNavigateUrl(portalId);

        return psPage;
    }

    private static PortalAliasInfo PortalAliasForNavigateUrl(int portalId) =>
        PortalAliasController.Instance.GetPortalAliasesByPortalId(portalId)
            .FirstOrDefault(alias => alias.IsPrimary); // get primary alias
}