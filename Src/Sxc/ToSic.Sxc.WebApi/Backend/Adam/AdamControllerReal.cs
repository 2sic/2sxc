using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.Backend.Adam;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AdamControllerReal<TIdentifier>(
    LazySvc<AdamTransUpload<TIdentifier, TIdentifier>> adamUpload,
    LazySvc<AdamTransGetItems<TIdentifier, TIdentifier>> adamItems,
    LazySvc<AdamTransFolder<TIdentifier, TIdentifier>> adamFolders,
    LazySvc<AdamTransDelete<TIdentifier, TIdentifier>> adamDelete,
    LazySvc<AdamTransRename<TIdentifier, TIdentifier>> adamRename)
    : ServiceBase("Api.Adam", connect: [adamUpload, adamItems, adamFolders, adamDelete, adamRename])
{
    public AdamItemDto Upload(HttpUploadedFile uploadInfo, int appId, string contentType, Guid guid, string field, string subFolder = "", bool usePortalRoot = false)
    {
        // wrap all of it in try/catch, to reformat error in better way for js to tell the user
        try
        {
            // Check if the request contains multipart/form-data.
            if (!uploadInfo.IsMultipart())
                return new("doesn't look like a file-upload");

            if (!uploadInfo.HasFiles())
            {
                Log.A("Error, no files");
                return new("No file was uploaded.");
            }

            var (fileName, stream) = uploadInfo.GetStream();
            var uploader = adamUpload.Value.Init(appId, contentType, guid, field, usePortalRoot);
            return uploader.UploadOne(stream, subFolder, fileName);
        }
        catch (HttpExceptionAbstraction he)
        {
            // Our abstraction places an extra message in the value, not sure if this is right, but that's how it is. 
            return new(he.Message + "\n" + he.Value);
        }
        catch (Exception e)
        {
            return new(e.Message + "\n" + e.Message);
        }
    }

    // Note: #AdamItemDto - as of now, we must use object because System.Io.Text.Json will otherwise not convert the object correctly :(
    public IEnumerable</*AdamItemDto*/object> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
    {
        var callLog = Log.Fn<IEnumerable<AdamItemDto>>($"adam items a:{appId}, i:{guid}, field:{field}, subfolder:{subfolder}, useRoot:{usePortalRoot}");
        var results = adamItems.Value
            .Init(appId, contentType, guid, field, usePortalRoot)
            .ItemsInField(subfolder);
        return callLog.ReturnAsOk(results);
    }

    public IEnumerable</*AdamItemDto*/object> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
        => adamFolders.Value
            .Init(appId, contentType, guid, field, usePortalRoot)
            .Folder(subfolder, newFolder);
        
    public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, TIdentifier id, bool usePortalRoot)
        => adamDelete.Value
            .Init(appId, contentType, guid, field, usePortalRoot)
            .Delete(subfolder, isFolder, id, id);

    public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, TIdentifier id, string newName, bool usePortalRoot)
        => adamRename.Value
            .Init(appId, contentType, guid, field, usePortalRoot)
            .Rename(subfolder, isFolder, id, id, newName);

}