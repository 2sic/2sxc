using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Adam;

namespace ToSic.Sxc.WebApi.Adam
{
    internal partial class AdamBackend
    {
        internal IList<AdamItemDto> ItemsInField(AdamState state, Guid entityGuid, string fieldSubfolder, bool usePortalRoot)
        {
            var wrapLog = Log.Call<IList<AdamItemDto>>();

            Log.Add("starting permissions checks");
            if (state.Security.UserIsRestricted && !state.Security.FieldPermissionOk(GrantSets.ReadSomething))
                return wrapLog("user is restricted, and doesn't have permissions on field - return null", null);

            // check that if the user should only see drafts, he doesn't see items of published data
            if (!state.Security.UserIsNotRestrictedOrItemIsDraft(entityGuid, out _))
                return wrapLog("user is restricted (no read-published rights) and item is published - return null", null);

            Log.Add("first permission checks passed");


            // get root and at the same time auto-create the core folder in case it's missing (important)
            state.ContainerContext.Folder();

            // try to see if we can get into the subfolder - will throw error if missing
            var currentAdam = state.ContainerContext.Folder(fieldSubfolder, false);
            var fs = state.AdamAppContext.EnvironmentFs;
            var currentFolder = fs.GetFolder(currentAdam.Id);

            // ensure that it's super user, or the folder is really part of this item
            if (!state.Security.SuperUserOrAccessingItemFolder(currentFolder.Path, out var exp))
            {
                Log.Add("user is not super-user and folder doesn't seem to be an ADAM folder of this item - will throw");
                throw exp;
            }

            var subfolders = currentFolder.Folders.ToList();
            var files = currentFolder.Files.ToList();

            var dtoMaker = Factory.Resolve<AdamItemDtoMaker>();
            var allDtos = new List<AdamItemDto>();

            var currentFolderDto = dtoMaker.Create(currentFolder, usePortalRoot, state);
            currentFolderDto.Name = ".";
            currentFolderDto.MetadataId = currentFolder.Metadata.EntityId;
            allDtos.Insert(0, currentFolderDto);

            var adamFolders = subfolders.Where(s => s.Id != currentFolder.Id)
                .Select(f =>
                {
                    var dto = dtoMaker.Create(f, usePortalRoot, state);
                    dto.MetadataId = Metadata.GetMetadataId(state.AdamAppContext.AppRuntime, f.Id, true);
                    return dto;
                })
                .ToList();
            allDtos.AddRange(adamFolders);

            var adamFiles = files
                .Select(f =>
                {
                    var dto = dtoMaker.Create(f, usePortalRoot, state);
                    dto.MetadataId = Metadata.GetMetadataId(state.AdamAppContext.AppRuntime, f.Id, false);
                    dto.Type = Classification.TypeName(f.Extension);
                    return dto;
                })
                .ToList();
            allDtos.AddRange(adamFiles);

            return wrapLog($"ok - fld⋮{adamFolders.Count}, files⋮{adamFiles.Count} tot⋮{allDtos.Count}", allDtos.ToList());
        }

    }
}
