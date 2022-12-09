using System;
using System.Collections.Generic;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;

namespace ToSic.Sxc.WebApi.Adam
{
    // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

    public class AdamControllerReal<TIdentifier>: HasLog
    {
        public AdamControllerReal(
            Lazy<AdamTransUpload<TIdentifier, TIdentifier>> adamUpload,
            Lazy<AdamTransGetItems<TIdentifier, TIdentifier>> adamItems,
            Lazy<AdamTransFolder<TIdentifier, TIdentifier>> adamFolders,
            Lazy<AdamTransDelete<TIdentifier, TIdentifier>> adamDelete,
            Lazy<AdamTransRename<TIdentifier, TIdentifier>> adamRename
            ) : base("Api.Adam")
        {
            _adamUpload = adamUpload;
            _adamItems = adamItems;
            _adamFolders = adamFolders;
            _adamDelete = adamDelete;
            _adamRename = adamRename;
        }
        private readonly Lazy<AdamTransUpload<TIdentifier, TIdentifier>> _adamUpload;
        private readonly Lazy<AdamTransGetItems<TIdentifier, TIdentifier>> _adamItems;
        private readonly Lazy<AdamTransFolder<TIdentifier, TIdentifier>> _adamFolders;
        private readonly Lazy<AdamTransDelete<TIdentifier, TIdentifier>> _adamDelete;
        private readonly Lazy<AdamTransRename<TIdentifier, TIdentifier>> _adamRename;

        public AdamItemDto Upload(HttpUploadedFile uploadInfo, int appId, string contentType, Guid guid, string field, string subFolder = "", bool usePortalRoot = false)
        {
            // wrap all of it in try/catch, to reformat error in better way for js to tell the user
            try
            {
                // Check if the request contains multipart/form-data.
                if (!uploadInfo.IsMultipart())
                    return new AdamItemDto("doesn't look like a file-upload");

                if (!uploadInfo.HasFiles())
                {
                    Log.A("Error, no files");
                    return new AdamItemDto("No file was uploaded.");
                }

                var (fileName, stream) = uploadInfo.GetStream();
                var uploader = _adamUpload.Value.Init(appId, contentType, guid, field, usePortalRoot, Log);
                return uploader.UploadOne(stream, subFolder, fileName);
            }
            catch (HttpExceptionAbstraction he)
            {
                // Our abstraction places an extra message in the value, not sure if this is right, but that's how it is. 
                return new AdamItemDto(he.Message + "\n" + he.Value);
            }
            catch (Exception e)
            {
                return new AdamItemDto(e.Message + "\n" + e.Message);
            }
        }

        // Note: #AdamItemDto - as of now, we must use object because System.Io.Text.Json will otherwise not convert the object correctly :(
        public IEnumerable</*AdamItemDto*/object> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
        {
            var callLog = Log.Fn<IEnumerable<AdamItemDto>>($"adam items a:{appId}, i:{guid}, field:{field}, subfolder:{subfolder}, useRoot:{usePortalRoot}");
            var results = _adamItems.Value
                .Init(appId, contentType, guid, field, usePortalRoot, Log)
                .ItemsInField(subfolder);
            return callLog.ReturnAsOk(results);
        }

        public IEnumerable</*AdamItemDto*/object> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
            => _adamFolders.Value
                .Init(appId, contentType, guid, field, usePortalRoot, Log)
                .Folder(subfolder, newFolder);
        
        public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, TIdentifier id, bool usePortalRoot)
            => _adamDelete.Value
                .Init(appId, contentType, guid, field, usePortalRoot, Log)
                .Delete(subfolder, isFolder, id, id);

        public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, TIdentifier id, string newName, bool usePortalRoot)
            => _adamRename.Value
                .Init(appId, contentType, guid, field, usePortalRoot, Log)
                .Rename(subfolder, isFolder, id, id, newName);

    }
}
