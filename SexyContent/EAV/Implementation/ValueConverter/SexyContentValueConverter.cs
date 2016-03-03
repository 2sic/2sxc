using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DotNetNuke.Common;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using ToSic.Eav;
using ToSic.Eav.Implementations.ValueConverter;

namespace ToSic.SexyContent.EAV.Implementation.ValueConverter
{
    public class SexyContentValueConverter : IEavValueConverter
    {
        private string hlnkType = "Hyperlink";

        public string Convert(ConversionScenario scenario, string type, string originalValue/*, PortalSettings portalInfo*/)
        {
            switch (scenario)
            {
                case ConversionScenario.GetFriendlyValue:
                    if (type == hlnkType)
                        return TryToResolveDnnCodeToLink(originalValue);
                    break;
                case ConversionScenario.ConvertFriendlyToData:
                    if (type == hlnkType)
                        return TryToResolveOneLinkToInternalDnnCode(originalValue);
                    break;
            }

            return originalValue;
        }

        /// <summary>
        /// Will take a link like http:\\... to a file or page and try to return a DNN-style info like
        /// Page:35 or File:43003
        /// </summary>
        /// <param name="potentialFilePath"></param>
        /// <returns></returns>
        private string TryToResolveOneLinkToInternalDnnCode(string potentialFilePath)
        {
            // note: this can always use the current context, because this should happen
            // when saving etc. - which is always expected to happen in the owning portal
	        var portalInfo = PortalSettings.Current; //PortalController.Instance.GetCurrentPortalSettings();

            // Try file reference
            var fileInfo = FileManager.Instance.GetFile(portalInfo.PortalId, potentialFilePath);
            if (fileInfo != null)
                return "File:" + fileInfo.FileId;

            // Try page / tab ID
            var tabController = new TabController();
            var tabCollection = tabController.GetTabsByPortal(portalInfo.PortalId);
            var tabInfo = tabCollection.Select(tab => tab.Value)
                                       .Where(tab => tab.TabPath == potentialFilePath)
                                       .FirstOrDefault();

            if (tabInfo != null)
                return "Page:" + tabInfo.TabID;

            return potentialFilePath;
        }

        /// <summary>
        /// Will take a link like "File:17" and convert to "Faq/screenshot1.jpg"
        /// It will always deliver a relative path to the portal root
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string TryToResolveDnnCodeToLink(string value)
        {
            // new
            var resultString = value;
            var regularExpression = Regex.Match(resultString, @"^(?<type>(file|page)):(?<id>[0-9]+)(?<params>(\?|\#).*)?$", RegexOptions.IgnoreCase);

            if (!regularExpression.Success)
                return value;

            //var fileManager = FileManager.Instance;
            //var tabController = new TabController();
            var linkType = regularExpression.Groups["type"].Value.ToLower();
            var linkId = int.Parse(regularExpression.Groups["id"].Value);
            var urlParams = regularExpression.Groups["params"].Value ?? "";

            // old

            //var match = Regex.Match(value, @"(?<type>.+)\:(?<id>\d+)");
            //if (!match.Success)
            //    return value;

            //var linkId = int.Parse(match.Groups["id"].Value);
            //var linkType = match.Groups["type"].Value;

            var result = linkType== "page"
                ? ResolvePageLink(linkId, value)
                : ResolveFileLink(linkId, value);

            return result + ((result == value) ? "" : urlParams);
        }

        private string ResolveFileLink(int linkId, string defaultValue)
        {
            var fileInfo = FileManager.Instance.GetFile(linkId);
            if (fileInfo == null)
                return defaultValue;

            #region special handling of issues in case something in the background is broken
            // there are cases where the PortalSettings will be null or something, and in these cases the serializer would break down
            // so this is to just ensure that if it can't be converted, it'll just fall back to default
            try
            {
                return Path.Combine(new PortalSettings(fileInfo.PortalId)?.HomeDirectory ?? "", fileInfo?.RelativePath ?? "");
            }
            catch
            {
                return defaultValue;
            }
            #endregion
        }

        private string ResolvePageLink(int id, string defaultValue)
        {
            var tabController = new TabController();
            // 2016-03-03 before issue #710
            //var tabInfo = tabController.GetTab(linkId);
            //if (tabInfo == null)
            //    return defaultValue;

            //return tabInfo.TabPath;
            //ownerPortalSettings = SxcInstance.
            var tabInfo = tabController.GetTab(id);
            if (tabInfo == null) return defaultValue;

            var portalSettings = PortalSettings.Current;

            // Get full PortalSettings (with portal alias) if module sharing is active
            if (PortalSettings.Current != null && PortalSettings.Current.PortalId != tabInfo.PortalID)
            //{
                portalSettings = new PortalSettings(tabInfo.PortalID); 
                //var portalAlias = ownerPortalSettings.PrimaryAlias ?? TestablePortalAliasController.Instance.GetPortalAliasesByPortalId(tabInfo.PortalID).First();
                //portalSettings = new PortalSettings(id, portalAlias);
            //}
            if(portalSettings == null) return defaultValue;

            // var tabInfo = tabController.GetTab(id, portalSettings.PortalId, false);
            // if (tabInfo == null) return defaultValue;
            if (tabInfo.CultureCode != "" && tabInfo.CultureCode != PortalSettings.Current.CultureCode)
            {
                var cultureTabInfo = tabController.GetTabByCulture(tabInfo.TabID, tabInfo.PortalID, LocaleController.Instance.GetLocale(PortalSettings.Current.CultureCode));

                if (cultureTabInfo != null)
                    tabInfo = cultureTabInfo;
            }

            // Exception in AdvancedURLProvider because ownerPortalSettings.PortalAlias is null
            return Globals.NavigateURL(tabInfo.TabID, portalSettings, "", new string[] {});// + urlParams;
        }
    }




}