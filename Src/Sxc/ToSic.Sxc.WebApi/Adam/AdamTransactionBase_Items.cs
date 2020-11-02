using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.WebApi.Adam
{
    public partial class AdamTransactionBase<T, TFolderId, TFileId>
    {
        internal IList<AdamItemDto> ItemsInField(string subFolderName)
        {
            var wrapLog = Log.Call<IList<AdamItemDto>>($"Subfolder: {subFolderName}");

            Log.Add("starting permissions checks");
            if (State.Security.UserIsRestricted && !State.Security.FieldPermissionOk(GrantSets.ReadSomething))
                return wrapLog("user is restricted, and doesn't have permissions on field - return null", null);

            // check that if the user should only see drafts, he doesn't see items of published data
            if (!State.Security.UserIsNotRestrictedOrItemIsDraft(State.ItemGuid, out _))
                return wrapLog("user is restricted (no read-published rights) and item is published - return null", null);

            Log.Add("first permission checks passed");


            // get root and at the same time auto-create the core folder in case it's missing (important)
            State.ContainerContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            var currentFolder = State.ContainerContext.Folder(subFolderName, false);

            // ensure that it's super user, or the folder is really part of this item
            if (!State.Security.SuperUserOrAccessingItemFolder(currentFolder.Path, out var exp))
            {
                Log.Add("user is not super-user and folder doesn't seem to be an ADAM folder of this item - will throw");
                throw exp;
            }

            var subfolders = currentFolder.Folders.ToList();
            var files = currentFolder.Files.ToList();

            var dtoMaker = Factory.Resolve<AdamItemDtoMaker<TFolderId, TFileId>>();
            var allDtos = new List<AdamItemDto>();

            var currentFolderDto = dtoMaker.Create(currentFolder, State);
            currentFolderDto.Name = ".";
            currentFolderDto.MetadataId = currentFolder.Metadata.EntityId;
            allDtos.Insert(0, currentFolderDto);

            var adamFolders = subfolders
                .Cast<Folder<TFolderId, TFileId>>()
                .Where(s => !EqualityComparer<TFolderId>.Default.Equals(s.SysId, currentFolder.SysId))
                .Select(f =>
                {
                    var dto = dtoMaker.Create(f, State);
                    dto.MetadataId = (int)f.Metadata.EntityId;
                    return dto;
                })
                .ToList();
            allDtos.AddRange(adamFolders);

            var adamFiles = files
                .Cast<File<TFolderId, TFileId>>()
                .Select(f =>
                {
                    var dto = dtoMaker.Create(f, State);
                    dto.MetadataId = (int)f.Metadata.EntityId;
                    dto.Type = Classification.TypeName(f.Extension);
                    return dto;
                })
                .ToList();
            allDtos.AddRange(adamFiles);

            return wrapLog($"ok - fld⋮{adamFolders.Count}, files⋮{adamFiles.Count} tot⋮{allDtos.Count}", allDtos.ToList());
        }

    }
}
