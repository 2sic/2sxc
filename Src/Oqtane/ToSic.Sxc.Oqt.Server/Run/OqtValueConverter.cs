﻿using System;
using System.IO;
using System.Linq;
using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.Configuration;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Run;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Oqt.Server.Plumbing;

namespace ToSic.Sxc.Oqt.Server.Run
{
    /// <summary>
    /// The Oqtane implementation of the <see cref="IValueConverter"/> which converts "file:22" or "page:5" to the url,
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class OqtValueConverter : IValueConverter
    {
        public Lazy<IFileRepository> FileRepository { get; }
        public Lazy<IFolderRepository> FolderRepository { get; }
        public Lazy<ITenantResolver> TenantResolver { get; }
        public Lazy<IPageRepository> PageRepository { get; }
        public Lazy<IServerPaths> ServerPaths { get; }
        public Lazy<SiteStateInitializer> SiteStateInitializerLazy { get; }

        #region DI Constructor

        public OqtValueConverter(
            Lazy<IFileRepository> fileRepository,
            Lazy<IFolderRepository> folderRepository,
            Lazy<ITenantResolver> tenantResolver,
            Lazy<IPageRepository> pageRepository,
            Lazy<IServerPaths> serverPaths,
            Lazy<SiteStateInitializer> siteStateInitializerLazy
            )
        {
            FileRepository = fileRepository;
            FolderRepository = folderRepository;
            TenantResolver = tenantResolver;
            PageRepository = pageRepository;
            ServerPaths = serverPaths;
            SiteStateInitializerLazy = siteStateInitializerLazy;
        }

        protected Alias Alias
        {
            get
            {
                if (_alias != null) return _alias;
                _alias = SiteStateInitializerLazy.Value.InitializedState.Alias;
                return _alias;
            }
        }

        private Alias _alias;

        #endregion

        /// <inheritdoc />
        public string ToReference(string value) => TryToResolveOneLinkToInternalOqtCode(value);

        /// <inheritdoc />
        public string ToValue(string reference, Guid itemGuid = default) => TryToResolveOqtCodeToLink(itemGuid, reference);

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
        private string TryToResolveOneLinkToInternalOqtCode(string potentialFilePath)
        {
            // find site
            //var site = TenantResolver.Value.GetAlias();

            // Try to find the Folder
            // todo: check if it has /Content/Tenant/1/Site/1 etc.
            var pathAsFolder = potentialFilePath.Backslash();
            var folderPath = Path.GetDirectoryName(pathAsFolder);
            var folder = FolderRepository.Value.GetFolder(Alias.SiteId, folderPath);
            if (folder != null)
            {
                // Try file reference
                var fileName = Path.GetFileNameWithoutExtension(pathAsFolder);
                var files = FileRepository.Value.GetFiles(folder.FolderId);
                var fileInfo = files.FirstOrDefault(f => f.Name == fileName);
                if (fileInfo != null) return "file:" + fileInfo.FileId;
            }

            var pathAsPageLink = potentialFilePath.ForwardSlash().TrimEnd('/').TrimStart('/'); // no trailing slashes
            // Try page / tab ID
            var page = PageRepository.Value.GetPage(pathAsPageLink, Alias.SiteId);
            return page != null
                ? "page:" + page.PageId
                : potentialFilePath;
        }

        /// <summary>
        /// Will take a link like "File:17" and convert to "Faq/screenshot1.jpg"
        /// It will always deliver a relative path to the portal root
        /// </summary>
        /// <param name="itemGuid">the item we're in - important for the feature which checks if the file is in this items ADAM</param>
        /// <param name="originalValue"></param>
        /// <returns></returns>
        private string TryToResolveOqtCodeToLink(Guid itemGuid, string originalValue)
        {
            if (string.IsNullOrEmpty(originalValue)) return originalValue;
            // new
            var resultString = originalValue;
            var parts = new ValueConverterBase.LinkParts(resultString);

            //var regularExpression = Regex.Match(resultString, ValueConverterBase.RegExToDetectConvertable, RegexOptions.IgnoreCase);

            if (!parts.IsMatch) // regularExpression.Success)
                return originalValue;

            //var linkType = regularExpression.Groups[ValueConverterBase.RegExType].Value.ToLowerInvariant();
            //var linkId = int.Parse(regularExpression.Groups[ValueConverterBase.RegExId].Value);
            //var urlParams = regularExpression.Groups[ValueConverterBase.RegExParams].Value ?? "";

            //var isPageLookup = linkType == ValueConverterBase.PrefixPage;
            try
            {
                var result = (parts.IsPage // isPageLookup
                                 ? ResolvePageLink(parts.Id)
                                 : ResolveFileLink(parts.Id, itemGuid))
                             ?? originalValue;

                return result + (result == originalValue ? "" : parts.Params);
            }
            catch /*(Exception e)*/
            {
                // 2021-04-26 2dm: We can't log errors here
                // - on one hand we would flood the logs
                // - on the other hand we have issues that if this happens during json-creation of a web-api, the Logger often can't find the DB/SiteState
                //var wrappedEx = new Exception("Error when trying to lookup a friendly url of \"" + originalValue + "\"", e);
                //Logger.Value.Log(LogLevel.Error, this, LogFunction.Other, wrappedEx.Message);
                return originalValue;
            }

        }

        private string ResolveFileLink(int linkId, Guid itemGuid)
        {
            var fileInfo = FileRepository.Value.GetFile(linkId);
            if (fileInfo == null)
                return null;

            // find out if the user is allowed to resolve this link
            #region special handling of issues in case something in the background is broken
            try
            {
                // SiteStateInitializerLazy.Value.InitIfEmpty();
                //var alias = SiteStateInitializerLazy.Value.InitializedState.Alias; // SiteStateInitializerLazy.Value.SiteState.Alias;

                var pathInAdam = Path.Combine(fileInfo.Folder.Path, fileInfo.Name/*)*/).ForwardSlash();

                // get appName and filePath
                var adamFolder = "adam/";
                var prefixStart = pathInAdam.IndexOf(adamFolder, StringComparison.OrdinalIgnoreCase);
                pathInAdam = pathInAdam.Substring(prefixStart + adamFolder.Length).TrimStart('/');
                var indexOfSlash = pathInAdam.IndexOf('/');
                var appName = pathInAdam.Substring(0, indexOfSlash);
                var filePath = pathInAdam.Substring(indexOfSlash).TrimStart('/');

                var result = $"{Alias.Path}/app/{appName}/adam/{filePath}".PrefixSlash();

                // optionally do extra security checks (new in 10.02)
                if (!Features.Enabled(FeatureIds.BlockFileIdLookupIfNotInSameApp)) return result;

                // check if it's in this item. We won't check the field, just the item, so the field is ""
                return !ToSic.Sxc.Adam.Security.PathIsInItemAdam(itemGuid, "", pathInAdam)
                    ? null
                    : result;
            }
            catch
            {
                return null;
            }
            #endregion
        }

        private string ResolvePageLink(int id)
        {
            var pageResolver = PageRepository.Value;

            var page = pageResolver.GetPage(id);
            if (page == null) return null;

            return $"/{Alias.Path}/{page.Path}";

            //var psCurrent = PortalSettings.Current;
            //var psPage = psCurrent;

            //// Get full PortalSettings (with portal alias) if module sharing is active
            //if (psCurrent != null && psCurrent.PortalId != tabInfo.PortalID)
            //    psPage = new PortalSettings(tabInfo.PortalID);

            //if (psPage == null) return null;

            //if (tabInfo.CultureCode != "" && psCurrent != null && tabInfo.CultureCode != psCurrent.CultureCode)
            //{
            //    var cultureTabInfo = tabController
            //        .GetTabByCulture(tabInfo.TabID, tabInfo.PortalID,
            //            LocaleController.Instance.GetLocale(psCurrent.CultureCode));

            //    if (cultureTabInfo != null)
            //        tabInfo = cultureTabInfo;
            //}

            //// Exception in AdvancedURLProvider because ownerPortalSettings.PortalAlias is null
            //return Globals.NavigateURL(tabInfo.TabID, psPage, "", new string[] { });
        }
    }
}
