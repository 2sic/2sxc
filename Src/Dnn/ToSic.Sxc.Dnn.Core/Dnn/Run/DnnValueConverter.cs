using System;
using System.IO;
using System.Linq;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Dnn.Run
{
    /// <summary>
    /// The DNN implementation of the <see cref="IValueConverter"/> which converts "file:22" or "page:5" to the url,
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class DnnValueConverter : IValueConverter
    {
        #region DI Constructor

        public DnnValueConverter(ISite site, Lazy<IFeaturesService> featuresLazy )
        {
            _site = site;
            _featuresLazy = featuresLazy;
        }

        private readonly ISite _site;
        private readonly Lazy<IFeaturesService> _featuresLazy;

        #endregion

        /// <inheritdoc />
        public string ToReference(string value) => TryToResolveOneLinkToInternalDnnCode(value);

        /// <inheritdoc />
        public string ToValue(string reference, Guid itemGuid = default) => TryToResolveDnnCodeToLink(itemGuid, reference);

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
            var fileInfo = FileManager.Instance.GetFile(_site.Id, potentialFilePath);
            if (fileInfo != null) return ValueConverterBase.PrefixFile + ValueConverterBase.Separator + fileInfo.FileId;

            // Try page / tab ID
            var tabController = new TabController();
            var tabCollection = tabController.GetTabsByPortal(_site.Id);
            var tabInfo = tabCollection.Select(tab => tab.Value)
                                       .FirstOrDefault(tab => tab.TabPath == potentialFilePath);

            return tabInfo != null 
                ? ValueConverterBase.PrefixPage + ValueConverterBase.Separator + tabInfo.TabID 
                : potentialFilePath;
        }

        /// <summary>
        /// Will take a link like "File:17" and convert to "Faq/screenshot1.jpg"
        /// It will always deliver a relative path to the portal root
        /// </summary>
        /// <param name="itemGuid">the item we're in - important for the feature which checks if the file is in this items ADAM</param>
        /// <param name="originalValue"></param>
        /// <returns></returns>
        private string TryToResolveDnnCodeToLink(Guid itemGuid, string originalValue)
        {
            try
            {
                return ValueConverterBase.TryToResolveCodeToLink(itemGuid, originalValue, ResolvePageLink, ResolveFileLink);
            }
            catch (Exception e)
            {
                var wrappedEx = new Exception("Error when trying to lookup a friendly url of \"" + originalValue + "\"", e);
                Exceptions.LogException(wrappedEx);
                return originalValue;
            }

        }

        private string ResolveFileLink(int linkId, Guid itemGuid)
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
                if (!_featuresLazy.Value.Enabled(FeaturesCatalog.BlockFileResolveOutsideOfEntityAdam.Guid)) return result;

                // check if it's in this item. We won't check the field, just the item, so the field is ""
                return !Sxc.Adam.Security.PathIsInItemAdam(itemGuid, "", filePath)
                    ? null
                    : result;
            }
            catch
            {
                return null;
            }
            #endregion
        }

        private static string ResolvePageLink(int id)
        {
            var tabController = new TabController();

            var tabInfo = tabController.GetTab(id, 0);
            if (tabInfo == null) return null;

            var psCurrent = PortalSettings.Current;
            var psPage = psCurrent;

            // Get full PortalSettings (with portal alias) if module sharing is active
            if (psCurrent != null && psCurrent.PortalId != tabInfo.PortalID)
                psPage = new PortalSettings(tabInfo.PortalID);

            if (psPage == null) return null;

            if (tabInfo.CultureCode != "" && psCurrent != null && tabInfo.CultureCode != psCurrent.CultureCode)
            {
                var cultureTabInfo = tabController
                    .GetTabByCulture(tabInfo.TabID, tabInfo.PortalID,
                        LocaleController.Instance.GetLocale(psCurrent.CultureCode));

                if (cultureTabInfo != null)
                    tabInfo = cultureTabInfo;
            }

            // Exception in AdvancedURLProvider because ownerPortalSettings.PortalAlias is null
            return Globals.NavigateURL(tabInfo.TabID, psPage, "", new string[] { });
        }
    }
}