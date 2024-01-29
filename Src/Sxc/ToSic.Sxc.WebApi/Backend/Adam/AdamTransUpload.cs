using System.IO;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Adam.Internal;

namespace ToSic.Sxc.Backend.Adam;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public partial class AdamTransUpload<TFolderId, TFileId>: AdamTransactionBase<AdamTransUpload<TFolderId, TFileId>, TFolderId, TFileId>
{
    public AdamItemDtoMaker<TFolderId, TFileId> DtoMaker { get; }

    public AdamTransUpload(MyServices services) : base(services, "Adm.TrnUpl")
    {
        DtoMaker = Services.AdamDtoMaker.New().Init(AdamContext);
    }

    public AdamItemDto UploadOne(Stream stream, string subFolder, string fileName)
    {
        var file = UploadOne(stream, fileName, subFolder, false);
        return DtoMaker.Create(file);
    }

    public File<TFolderId, TFileId> UploadOne(Stream stream, string originalFileName, string subFolder, bool skipFieldAndContentTypePermissionCheck)
    {
        Log.A($"upload one subfold:{subFolder}, file: {originalFileName}");

        // make sure the file name we'll use doesn't contain injected path-traversal
        originalFileName = Path.GetFileName(originalFileName);

        HttpExceptionAbstraction exp;
        if (!skipFieldAndContentTypePermissionCheck)
        {
            if (!AdamContext.Security.UserIsPermittedOnField(GrantSets.WriteSomething, out exp))
                throw exp;

            // check that if the user should only see drafts, he doesn't see items of published data
            if (!AdamContext.Security.UserIsNotRestrictedOrItemIsDraft(AdamContext.ItemGuid, out var permissionException))
                throw permissionException;
        }

        var folder = AdamContext.AdamRoot.Folder(autoCreate: true);

        if (!string.IsNullOrEmpty(subFolder))
            folder = AdamContext.AdamRoot.Folder(subFolder, true);

        // start with a security check...
        var fs = AdamContext.AdamManager.AdamFs;
        var parentFolder = fs.GetFolder(folder.SysId);

        // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
        if (AdamContext.UseSiteRoot && !AdamContext.Security.CanEditFolder(parentFolder))
            throw HttpException.PermissionDenied("can't upload - permission denied");

        // we only upload into valid adam if that's the scenario
        if (!AdamContext.Security.SuperUserOrAccessingItemFolder(parentFolder.Path, out exp))
            throw exp;

        #region check content-type extensions...

        // Check file size and extension
        var fileName = originalFileName;
        if (!AdamContext.Security.ExtensionIsOk(fileName, out var exceptionAbstraction))
            throw exceptionAbstraction;

        // check metadata of the FieldDef to see if still allowed extension
        // note 2018-04-20 2dm: can't do this for wysiwyg, as it doesn't have a setting for allowed file-uploads
        var additionalFilter = AdamContext.Attribute.Metadata.GetBestValue<string>("FileFilter");
        if (!string.IsNullOrWhiteSpace(additionalFilter)
            && !CustomFileFilterOk(additionalFilter, fileName))
            throw HttpException.NotAllowedFileType(fileName, "field has custom file-filter, which doesn't match");

        #endregion

        var maxSize = (long)fs.MaxUploadKb() * 1024; // convert to bytes (without overflow that happens with int)
        var fileSize = stream.Length;
        Log.A($"file size: {fileSize} (max size is {maxSize})");
        if (fileSize > maxSize)
            throw new($"file too large, {fileSize} is more than {maxSize}");

        // remove forbidden / troubling file name characters
        fileName = fileName
            .Replace("+", "plus")
            .Replace("%", "per")
            .Replace("#", "hash");
            
        if (fileName != originalFileName)
            Log.A($"cleaned file name from'{originalFileName}' to '{fileName}'");

        var eavFile = fs.Add(parentFolder, stream, fileName, true);

        return eavFile;
    }
}