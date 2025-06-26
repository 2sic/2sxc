using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Adam.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class AdamWorkUpload(AdamWorkBase.MyServices services)
    : AdamWorkBase(services, "Adm.TrnUpl")
{

    public IFile UploadOneNew(Stream stream, string subFolder, string fileName)
    {
        var file = UploadOne(stream, fileName, subFolder, false);
        return file;
    }

    public IFile UploadOne(Stream stream, string originalFileName, string subFolder, bool skipFieldAndContentTypePermissionCheck)
    {
        Log.A($"upload one subfold:{subFolder}, file: {originalFileName}");

        // make sure the file name we'll use doesn't contain injected path-traversal
        originalFileName = Path.GetFileName(originalFileName);

        if (!skipFieldAndContentTypePermissionCheck)
        {
            if (AdamContext.Security.UserNotPermittedOnField(GrantSets.WriteSomething, out var exp))
                throw exp;

            // check that if the user should only see drafts, he doesn't see items of published data
            if (AdamContext.Security.UserIsRestrictedOrItemIsNotDraft(AdamContext.ItemGuid, out var permissionException))
                throw permissionException;
        }

        // Access parent to be sure it's created
        var folder = AdamContext.AdamRoot.RootFolder(autoCreate: true);

        if (!string.IsNullOrEmpty(subFolder))
            folder = AdamContext.AdamRoot.Folder(subFolder, true);

        // start with a security check...
        var fs = AdamContext.AdamManager.AdamFs;
        var parentFolder = folder; // 2025-05-20 2dm: this was before, seems to just get itself: fs.GetFolder(folder.SysId);

        // validate that dnn user have write permissions for folder in case dnn file system is used (usePortalRoot)
        if (AdamContext.UseSiteRoot && !AdamContext.Security.CanEditFolder(parentFolder))
            throw HttpException.PermissionDenied("can't upload - permission denied");

        // we only upload into valid adam if that's the scenario
        if (AdamContext.Security.UserIsRestrictedAndAccessingItemOutsideOfFolder(parentFolder?.Path, out var expNotAllowed))
            throw expNotAllowed;

        #region check content-type extensions...

        // Check file size and extension
        var fileName = originalFileName;
        if (AdamContext.Security.ExtensionIsNotOk(fileName, out var exceptionAbstraction))
            throw exceptionAbstraction;

        // check metadata of the FieldDef to see if still allowed extension
        // note 2018-04-20 2dm: can't do this for wysiwyg, as it doesn't have a setting for allowed file-uploads
        var additionalFilter = AdamContext.Attribute!.Metadata.Get<string>("FileFilter");
        if (!string.IsNullOrWhiteSpace(additionalFilter) && !CustomFileFilterOk(additionalFilter!, fileName))
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