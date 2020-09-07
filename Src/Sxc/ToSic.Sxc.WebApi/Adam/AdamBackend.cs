using JetBrains.Annotations;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.WebApi.Adam
{
    internal partial class AdamBackend: HasLog<AdamBackend>
    {
        public AdamBackend() : base("Adm.BckEnd")
        {

        }

        /// <summary>
        /// Validate that user has write permissions for folder.
        /// In case the primary file system is used (usePortalRoot) then also check higher permissions
        /// </summary>
        /// <param name="state"></param>
        /// <param name="parentFolder"></param>
        /// <param name="target"></param>
        /// <param name="folderId"></param>
        /// <param name="usePortalRoot"></param>
        /// <param name="errPrefix"></param>
        [AssertionMethod]
        private static void VerifySecurityAndStructure(AdamState state, Eav.Apps.Assets.IAsset parentFolder, Eav.Apps.Assets.IAsset target, int folderId, bool usePortalRoot, string errPrefix)
        {
            // In case the primary file system is used (usePortalRoot) then also check higher permissions
            if (usePortalRoot && !state.Security.CanEditFolder(folderId))
                throw HttpException.PermissionDenied(errPrefix + " - permission denied");

            if (!state.Security.SuperUserOrAccessingItemFolder(target.Path, out var exp))
                throw exp;

            if (target.ParentId != parentFolder.Id)
                throw HttpException.BadRequest(errPrefix + " - not found in folder");
        }


    }
}
