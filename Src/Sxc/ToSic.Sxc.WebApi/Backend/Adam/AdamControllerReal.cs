using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Adam.Sys;
using ToSic.Sxc.Adam.Work.Internal;

namespace ToSic.Sxc.Backend.Adam;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamControllerReal<TIdentifier>(
    Generator<AdamWorkUpload, AdamWorkOptions> adamUpload,
    Generator<AdamWorkGet, AdamWorkOptions> adamWorkGet,
    Generator<AdamWorkFolderCreate, AdamWorkOptions> adamFolders,
    Generator<AdamWorkDelete, AdamWorkOptions> adamDelete,
    Generator<AdamWorkRename, AdamWorkOptions> adamRename,
    Generator<IAdamItemDtoMaker, AdamItemDtoMakerOptions> dtoMaker)
    : ServiceBase("Api.Adam", connect: [adamUpload, adamWorkGet, adamFolders, adamDelete, adamRename])
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
            if (stream == null!)
                return l.ReturnAsError(new("File Stream is empty"));

            var uploader = adamUpload.New(new()
            {
                AppId = appId,
                ContentType = contentType,
                ItemGuid = guid,
                Field = field,
                UsePortalRoot = usePortalRoot,
            });//.Value.Setup(appId, contentType, guid, field, usePortalRoot);
            var file = uploader.UploadOneNew(stream, subFolder, fileName);

            var dtoMake = dtoMaker.New(new() { AdamContext = uploader.AdamContext, });
            return dtoMake.Create(file);
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
        var adamGet = adamWorkGet.New(new()
        {
            AppId = appId,
            ContentType = contentType,
            ItemGuid = guid,
            Field = field,
            UsePortalRoot = usePortalRoot,
        });
        var results = adamGet.ItemsInField(subfolder);

        if (results == null)
            return l.ReturnAsError([], "got empty object, user probably restricted");

        var dto = dtoMaker
            .New(new() { AdamContext = adamGet.AdamContext })
            .Convert(results);

        return l.ReturnAsOk(dto);
    }

    public IEnumerable</*AdamItemDto*/object> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
    {
        var l = Log.Fn<IEnumerable<object>>();
        adamFolders.New(new()
            {
                AppId = appId,
                ContentType = contentType,
                ItemGuid = guid,
                Field = field,
                UsePortalRoot = usePortalRoot,
            })
            .Create(subfolder, newFolder);

        var adamGet = adamWorkGet.New(new()
        {
            AppId = appId,
            ContentType = contentType,
            ItemGuid = guid,
            Field = field,
            UsePortalRoot = usePortalRoot,
        });
        var folder = adamGet.ItemsInField(subfolder);

        if (folder == null)
            return l.ReturnAsError([], "got empty object, user probably restricted");

        var dto = dtoMaker
            .New(new() { AdamContext = adamGet.AdamContext })
            .Convert(folder);
        return l.ReturnAsOk(dto);
    }

    public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, TIdentifier id, bool usePortalRoot)
        => adamDelete.New(new()
        {
            AppId = appId,
            ContentType = contentType,
            ItemGuid = guid,
            Field = field,
            UsePortalRoot = usePortalRoot,
        })
            .Delete(subfolder, isFolder, AdamAssetIdentifier.Create(id),  AdamAssetIdentifier.Create(id));

    public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, TIdentifier id, string newName, bool usePortalRoot)
        => adamRename.New(new()
            {
                AppId = appId,
                ContentType = contentType,
                ItemGuid = guid,
                Field = field,
                UsePortalRoot = usePortalRoot,
            })
            .Rename(subfolder, isFolder, AdamAssetIdentifier.Create(id), AdamAssetIdentifier.Create(id), newName);

}