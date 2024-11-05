using System;
using System.IO;
using Oqtane.Models;
using Oqtane.Repository;
using ToSic.Eav.Data;
using ToSic.Eav.Helpers;
using ToSic.Eav.Internal.Environment;
using ToSic.Eav.Internal.Features;
using ToSic.Lib.DI;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Server.Plumbing;
using static ToSic.Eav.Internal.Features.BuiltInFeatures;

namespace ToSic.Sxc.Oqt.Server.Data;

/// <summary>
/// The Oqtane implementation of the <see cref="IValueConverter"/> which converts "file:22" or "page:5" to the url,
/// </summary>
[PrivateApi]
internal class OqtValueConverter : ValueConverterBase
{
    private readonly LazySvc<IEavFeaturesService> _featuresLazy;
    public LazySvc<IFileRepository> FileRepository { get; }
    public LazySvc<IFolderRepository> FolderRepository { get; }
    public LazySvc<ITenantResolver> TenantResolver { get; }
    public LazySvc<IPageRepository> PageRepository { get; }
    public LazySvc<IServerPaths> ServerPaths { get; }
    public LazySvc<AliasResolver> AliasResolverLazy { get; }

    #region DI Constructor

    public OqtValueConverter(
        LazySvc<IFileRepository> fileRepository,
        LazySvc<IFolderRepository> folderRepository,
        LazySvc<ITenantResolver> tenantResolver,
        LazySvc<IPageRepository> pageRepository,
        LazySvc<IServerPaths> serverPaths,
        LazySvc<AliasResolver> aliasResolverLazy,
        LazySvc<IEavFeaturesService> featuresLazy) : base("Oqt.ValCn")
    {
        ConnectLogs([
            _featuresLazy = featuresLazy,
            FileRepository = fileRepository,
            FolderRepository = folderRepository,
            TenantResolver = tenantResolver,
            PageRepository = pageRepository,
            ServerPaths = serverPaths,
            AliasResolverLazy = aliasResolverLazy
        ]);
    }

    protected Alias Alias
    {
        get
        {
            if (_alias != null) return _alias;
            _alias = AliasResolverLazy.Value.Alias;
            return _alias;
        }
    }

    private Alias _alias;

    #endregion

    /// <inheritdoc />
    public override string ToReference(string value) => TryToResolveOneLinkToInternalOqtCode(value);

    /// <inheritdoc />
    public override string ToValue(string reference, Guid itemGuid = default) => TryToResolveCodeToLink(itemGuid, reference);

    /// <summary>
    /// Will take a link like http://... to a file or page and try to return a DNN-style info like
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
        // Try to find the Folder
        var pathAsFolder = potentialFilePath.Backslash();
        var folderPath = Path.GetDirectoryName(pathAsFolder);
        var folder = FolderRepository.Value.GetFolder(Alias.SiteId, folderPath.EnsureOqtaneFolderFormat());
        if (folder != null)
        {
            // Try file reference
            var fileName = Path.GetFileNameWithoutExtension(pathAsFolder);
            var files = FileRepository.Value.GetFiles(folder.FolderId);
            var fileInfo = files.FirstOrDefault(f => f.Name == fileName);
            if (fileInfo != null) return "file:" + fileInfo.FileId;
        }

        var pathAsPageLink = potentialFilePath.TrimLastSlash(); // no trailing slashes
        // Try page / tab ID
        var page = PageRepository.Value.GetPage(pathAsPageLink, Alias.SiteId);
        return page != null
            ? "page:" + page.PageId
            : potentialFilePath;
    }

    ///// <summary>
    ///// Will take a link like "File:17" and convert to "Faq/screenshot1.jpg"
    ///// It will always deliver a relative path to the portal root
    ///// </summary>
    ///// <param name="itemGuid">the item we're in - important for the feature which checks if the file is in this items ADAM</param>
    ///// <param name="originalValue"></param>
    ///// <returns></returns>
    //private string TryToResolveOqtCodeToLink(Guid itemGuid, string originalValue)
    //{
    //    try
    //    {
    //        return TryToResolveCodeToLink(itemGuid, originalValue);
    //    }
    //    catch /*(Exception e)*/
    //    {
    //        return originalValue;
    //    }
    //}

    /// <summary>
    /// Don't do anything here
    /// </summary>
    protected override void LogConversionExceptions(string originalValue, Exception e)
    {
        // 2021-04-26 2dm: We can't log errors here
        // - on one hand we would flood the logs
        // - on the other hand we have issues that if this happens during json-creation of a web-api, the Logger often can't find the DB/SiteState
        //var wrappedEx = new Exception("Error when trying to lookup a friendly url of \"" + originalValue + "\"", e);
        //Logger.Value.Log(LogLevel.Error, this, LogFunction.Other, wrappedEx.Message);
    }


    protected override string ResolveFileLink(int linkId, Guid itemGuid)
    {
        var fileInfo = FileRepository.Value.GetFile(linkId);
        if (fileInfo == null)
            return null;

        // find out if the user is allowed to resolve this link
        #region special handling of issues in case something in the background is broken
        try
        {
            var pathInAdam = Path.Combine(fileInfo.Folder.Path, fileInfo.Name).ForwardSlash();

            // get appName and filePath
            var adamFolder = "adam/";
            var prefixStart = pathInAdam.IndexOf(adamFolder, StringComparison.OrdinalIgnoreCase);
            pathInAdam = pathInAdam[(prefixStart + adamFolder.Length)..].TrimStart('/');
            var indexOfSlash = pathInAdam.IndexOf('/');
            var appName = pathInAdam[..indexOfSlash];
            var filePath = pathInAdam[indexOfSlash..].TrimStart('/');

            var result = $"{Alias.Path}/app/{appName}/adam/{filePath}".PrefixSlash();

            // optionally do extra security checks (new in 10.02)
            if (!_featuresLazy.Value.IsEnabled(AdamRestrictLookupToEntity.Guid)) return result;

            // check if it's in this item. We won't check the field, just the item, so the field is ""
            return !Sxc.Adam.Internal.Security.PathIsInItemAdam(itemGuid, "", pathInAdam)
                ? null
                : result;
        }
        catch
        {
            return null;
        }
        #endregion
    }

    protected override string ResolvePageLink(int id)
    {
        var pageResolver = PageRepository.Value;

        var page = pageResolver.GetPage(id);
        if (page == null) return null;

        return string.IsNullOrEmpty(Alias.Path) ? $"/{page.Path}" : $"/{Alias.Path}/{page.Path}";
    }
}