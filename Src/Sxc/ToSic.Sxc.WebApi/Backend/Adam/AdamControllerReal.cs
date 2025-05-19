using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Adam.Work.Internal;

namespace ToSic.Sxc.Backend.Adam;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamControllerReal<TIdentifier>(
    LazySvc<AdamWorkUpload<TIdentifier, TIdentifier>> adamUpload,
    LazySvc<AdamPrefetchHelper<TIdentifier, TIdentifier>> adamItems,
    LazySvc<AdamWorkFolderGet<TIdentifier, TIdentifier>> adamFolders,
    LazySvc<AdamWorkDelete<TIdentifier, TIdentifier>> adamDelete,
    LazySvc<AdamWorkRename<TIdentifier, TIdentifier>> adamRename,
    AdamItemDtoMaker<TIdentifier, TIdentifier> dtoMaker)
    : ServiceBase("Api.Adam", connect: [adamUpload, adamItems, adamFolders, adamDelete, adamRename])
{
    public AdamItemDto Upload(HttpUploadedFile uploadInfo, int appId, string contentType, Guid guid, string field, string subFolder = "", bool usePortalRoot = false)
    {
        var l = Log.Fn<AdamItemDto>();
        // wrap all of it in try/catch, to reformat error in better way for js to tell the user
        try
        {
            // Check if the request contains multipart/form-data.
            if (!uploadInfo.IsMultipart())
                return l.Return(new("doesn't look like a file-upload"), "no file multipart");

            if (!uploadInfo.HasFiles())
                return l.Return(new("No file was uploaded."), "Error, no files");

            var (fileName, stream) = uploadInfo.GetStream();
            var uploader = adamUpload.Value.Setup(appId, contentType, guid, field, usePortalRoot);
            var file = uploader.UploadOneNew(stream, subFolder, fileName);
            //dtoMaker.Init(uploader.AdamContext);
            var dtoMake = dtoMaker.SpawnNew(new() { AdamContext = uploader.AdamContext, });
            return dtoMake.Create(file); // uploader.UploadOne(stream, subFolder, fileName);
        }
        catch (HttpExceptionAbstraction he)
        {
            // Our abstraction places an extra message in the value, not sure if this is right, but that's how it is. 
            return l.ReturnAsError(new(he.Message + "\n" + he.Value));
        }
        catch (Exception e)
        {
            return l.ReturnAsError(new(e.Message + "\n" + e.Message));
        }
    }

    // Note: #AdamItemDto - as of now, we must use object because System.Io.Text.Json will otherwise not convert the object correctly :(
    public IEnumerable</*AdamItemDto*/object> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
    {
        var l = Log.Fn<IEnumerable<AdamItemDto>>($"adam items a:{appId}, i:{guid}, field:{field}, subfolder:{subfolder}, useRoot:{usePortalRoot}");
        var results = adamItems.Value
            .Setup(appId, contentType, guid, field, usePortalRoot)
            .ItemsInFieldNew(subfolder);

        dtoMaker.Init(adamItems.Value.AdamContext);
        var dto = dtoMaker.Convert(results);

        return l.ReturnAsOk(dto);
    }

    public IEnumerable</*AdamItemDto*/object> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
    {
        var folder = adamFolders.Value
            .Setup(appId, contentType, guid, field, usePortalRoot)
            .Folder(subfolder, newFolder);

        dtoMaker.Init(adamItems.Value.AdamContext);
        var dto = dtoMaker.Convert(folder);
        return dto;
    }

    public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, TIdentifier id, bool usePortalRoot)
        => adamDelete.Value
            .Setup(appId, contentType, guid, field, usePortalRoot)
            .Delete(subfolder, isFolder, id, id);

    public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, TIdentifier id, string newName, bool usePortalRoot)
        => adamRename.Value
            .Setup(appId, contentType, guid, field, usePortalRoot)
            .Rename(subfolder, isFolder, id, id, newName);

}