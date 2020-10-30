using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Oqtane.Repository;
using ToSic.Eav.Configuration;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Run;

// todo: nothing here is tested yet!

namespace ToSic.Sxc.Oqt.Server.Run
{
    /// <summary>
    /// The DNN implementation of the <see cref="IValueConverter"/> which converts "file:22" or "page:5" to the url,
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("this is just fyi")]
    public class OqtValueConverter : IValueConverter
    {
        private readonly Lazy<ILinkPaths> _linkPaths;
        private ILinkPaths LinkPaths => _linkPaths.Value;

        public Lazy<IFileRepository> FileRepository { get; }
        public Lazy<IFolderRepository> FolderRepository { get; }
        public Lazy<ITenantResolver> TenantResolver { get; }
        public Lazy<IPageRepository> PageRepository { get; }
        public Lazy<IServerPaths> ServerPaths { get; }

        #region DI Constructor

        public OqtValueConverter(
            Lazy<IFileRepository> fileRepository, 
            Lazy<IFolderRepository> folderRepository, 
            Lazy<ITenantResolver> tenantResolver,
            Lazy<IPageRepository> pageRepository,
            Lazy<IServerPaths> serverPaths,
            Lazy<ILinkPaths> linkPaths)
        {
            _linkPaths = linkPaths;
            FileRepository = fileRepository;
            FolderRepository = folderRepository;
            TenantResolver = tenantResolver;
            PageRepository = pageRepository;
            ServerPaths = serverPaths;
        }


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
            var site = TenantResolver.Value.GetAlias();

            // Try to find the Folder
            // todo: check if it has /Content/Tenant/1/Site/1 etc.
            var pathAsFolder = potentialFilePath.Backslash();
            var folderPath = Path.GetDirectoryName(pathAsFolder);
            var folder = FolderRepository.Value.GetFolder(site.SiteId, folderPath);
            if (folder != null)
            {
                // Try file reference
                var fileName = Path.GetFileNameWithoutExtension(pathAsFolder);
                var files = FileRepository.Value.GetFiles(folder.FolderId); //, potentialFilePath);
                var fileInfo = files.FirstOrDefault(f => f.Name == fileName);
                if (fileInfo != null) return "file:" + fileInfo.FileId;
            }

            var pathAsPageLink = potentialFilePath.Forwardslash().TrimEnd('/').TrimStart('/'); // no trailing slashes
            // Try page / tab ID
            var page = PageRepository.Value.GetPage(pathAsPageLink, site.SiteId);
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
            // new
            var resultString = originalValue;
            var regularExpression = Regex.Match(resultString, @"^(?<type>(file|page)):(?<id>[0-9]+)(?<params>(\?|\#).*)?$", RegexOptions.IgnoreCase);

            if (!regularExpression.Success)
                return originalValue;

            var linkType = regularExpression.Groups["type"].Value.ToLower();
            var linkId = int.Parse(regularExpression.Groups["id"].Value);
            var urlParams = regularExpression.Groups["params"].Value ?? "";

            var isPageLookup = linkType == "page";
            try
            {
                var result = (isPageLookup
                                 ? ResolvePageLink(linkId)
                                 : ResolveFileLink(linkId, itemGuid))
                             ?? originalValue;

                return result + (result == originalValue ? "" : urlParams);
            }
            catch (Exception e)
            {
                var wrappedEx = new Exception("Error when trying to lookup a friendly url of \"" + originalValue + "\"", e);
                // TODO SPM: find out if we can log to Oqtane, otherwise add a WIP reference here for exceptions
                //Exceptions.LogException(wrappedEx);
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
                var filePath = LinkPaths.ToAbsolute(Path.Combine(fileInfo.Folder.Path, fileInfo.Name));
                // = Path.Combine(new PortalSettings(fileInfo.PortalId)?.HomeDirectory ?? "", fileInfo?.RelativePath ?? "");

                var siteId = TenantResolver.Value.GetAlias().SiteId;

                // return linkclick url for secure and other not standard folder locations
                //var result = (fileInfo.StorageLocation == 0) ? filePath : FileLinkClickController.Instance.GetFileLinkClick(fileInfo);
                var result = $"/{siteId}/api/sxc/{filePath}".Forwardslash();

                // optionally do extra security checks (new in 10.02)
                if (!Features.Enabled(FeatureIds.BlockFileIdLookupIfNotInSameApp)) return result;

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

        private string ResolvePageLink(int id)
        {
            var pageResolver = PageRepository.Value;

            var page = pageResolver.GetPage(id);
            if (page == null) return null;

            return "/" + page.Path;

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