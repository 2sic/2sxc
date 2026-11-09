using ToSic.Sxc.Adam.Sys;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Adam.Sys.Work;

namespace ToSic.Sxc.Backend.Adam;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamControllerReal<TIdentifier>(
    Generator<AdamContext, AdamContext.Options> adamContextGen,
    Generator<AdamWorkUpload, AdamContext> adamUpload,
    Generator<AdamWorkGet, AdamContext> adamWorkGet,
    Generator<AdamWorkFolderCreate, AdamContext> adamFolders,
    Generator<AdamWorkDelete, AdamContext> adamDelete,
    Generator<AdamWorkRename, AdamContext> adamRename,
    Generator<IAdamItemDtoMaker, AdamItemDtoMakerOptions> dtoMaker,
    ISxcCurrentContextService ctxService)
    : ServiceBase("Api.Adam", connect: [adamContextGen, adamUpload, adamWorkGet, adamFolders, adamDelete, adamRename])
{
    // TODO: @2dm change to not changing the state
    private IContextOfApp ShareAppStateOptimized(int appId)
    {
        return ctxService.ShareAppStateOptimized(appId);
    }
    
    public AdamItemDto Upload(HttpUploadedFile uploadInfo, int appId, string contentType, Guid guid, string field, string subFolder = "", bool usePortalRoot = false)
    {
        var l = Log.Fn<AdamItemDto>();
        // wrap all of it in try/catch, to reformat error in better way for js to tell the user
        try
        {
            // 1. Set the global context
            var context = ShareAppStateOptimized(appId);
            
            // Check if the request contains multipart/form-data.
            if (!uploadInfo.IsMultipart())
                return l.Return(new("doesn't look like a file-upload"), "no file multipart");

            if (!uploadInfo.HasFiles())
                return l.Return(new("No file was uploaded."), "Error, no files");

            var (fileName, stream) = uploadInfo.GetStream();
            if (stream == null!)
                return l.ReturnAsError(new("File Stream is empty"));

            var adamContext = adamContextGen.New(new(
                Context: context,
                ContentType: contentType,
                FieldName: field,
                EntityGuid: guid,
                UsePortalRoot: usePortalRoot));

            var file = adamUpload.New(adamContext).UploadOneNew(stream, subFolder, fileName);

            var dtoMake = dtoMaker.New(new() { AdamContext = adamContext, });
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

        // 1. Set the global context
        var context = ShareAppStateOptimized(appId);

        var adamContext = adamContextGen.New(new(
            Context: context,
            ContentType: contentType,
            FieldName: field,
            EntityGuid: guid,
            UsePortalRoot: usePortalRoot));

        var results = adamWorkGet.New(adamContext).ItemsInField(subfolder);

        if (results == null)
            return l.ReturnAsError([], "got empty object, user probably restricted");

        var dto = dtoMaker
            .New(new() { AdamContext = adamContext })
            .Convert(results);

        return l.ReturnAsOk(dto);
    }

    public IEnumerable</*AdamItemDto*/object> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
    {
        var l = Log.Fn<IEnumerable<object>>();

        // 1. Set the global context
        var context = ShareAppStateOptimized(appId);

        var adamContext = adamContextGen.New(new(
            Context: context,
            ContentType: contentType,
            FieldName: field,
            EntityGuid: guid,
            UsePortalRoot: usePortalRoot));

        adamFolders.New(adamContext).Create(subfolder, newFolder);

        var folder = adamWorkGet.New(adamContext).ItemsInField(subfolder);

        if (folder == null)
            return l.ReturnAsError([], "got empty object, user probably restricted");

        var dto = dtoMaker
            .New(new() { AdamContext = adamContext })
            .Convert(folder);
        return l.ReturnAsOk(dto);
    }

    public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, TIdentifier id, bool usePortalRoot)
    {
        // 1. Set the global context
        var context = ShareAppStateOptimized(appId);

        var adamContext = adamContextGen.New(new(
            Context: context,
            ContentType: contentType,
            FieldName: field,
            EntityGuid: guid,
            UsePortalRoot: usePortalRoot));
        return adamDelete.New(adamContext)
            .Delete(subfolder, isFolder, AdamAssetIdentifier.Create(id), AdamAssetIdentifier.Create(id));
    }

    public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder,
        TIdentifier id, string newName, bool usePortalRoot)
    {
        // 1. Set the global context
        var context = ShareAppStateOptimized(appId);

        var adamContext = adamContextGen.New(new(
            Context: context,
            ContentType: contentType,
            FieldName: field,
            EntityGuid: guid,
            UsePortalRoot: usePortalRoot));
        return adamRename.New(adamContext)
            .Rename(subfolder, isFolder, AdamAssetIdentifier.Create(id), AdamAssetIdentifier.Create(id), newName);
    }
}
