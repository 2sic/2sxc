using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.WebApi.Adam
{
    internal partial class AdamTransactionBase<T>
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
            var currentAdam = State.ContainerContext.Folder(subFolderName, false);
            var fs = State.AdamAppContext.EnvironmentFs;
            var currentFolder = fs.GetFolder(currentAdam.Id);

            // ensure that it's super user, or the folder is really part of this item
            if (!State.Security.SuperUserOrAccessingItemFolder(currentFolder.Path, out var exp))
            {
                Log.Add("user is not super-user and folder doesn't seem to be an ADAM folder of this item - will throw");
                throw exp;
            }

            var subfolders = currentFolder.Folders.ToList();
            var files = currentFolder.Files.ToList();

            var dtoMaker = Factory.Resolve<AdamItemDtoMaker>();
            var allDtos = new List<AdamItemDto>();

            var currentFolderDto = dtoMaker.Create(currentFolder, State);
            currentFolderDto.Name = ".";
            currentFolderDto.MetadataId = currentFolder.Metadata.EntityId;
            allDtos.Insert(0, currentFolderDto);

            var adamFolders = subfolders.Where(s => s.Id != currentFolder.Id)
                .Select(f =>
                {
                    var dto = dtoMaker.Create(f, State);
                    dto.MetadataId = Metadata.GetMetadataId(State.AdamAppContext.AppRuntime, f.Id, true);
                    return dto;
                })
                .ToList();
            allDtos.AddRange(adamFolders);

            var adamFiles = files
                .Select(f =>
                {
                    var dto = dtoMaker.Create(f, State);
                    dto.MetadataId = Metadata.GetMetadataId(State.AdamAppContext.AppRuntime, f.Id, false);
                    dto.Type = Classification.TypeName(f.Extension);
                    return dto;
                })
                .ToList();
            allDtos.AddRange(adamFiles);

            return wrapLog($"ok - fld⋮{adamFolders.Count}, files⋮{adamFiles.Count} tot⋮{allDtos.Count}", allDtos.ToList());
        }

    }
}
