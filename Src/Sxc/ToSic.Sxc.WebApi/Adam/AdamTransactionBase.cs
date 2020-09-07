using System;
using JetBrains.Annotations;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Logging;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.WebApi.Adam
{
    internal abstract partial class AdamTransactionBase<T>: HasLog where T : class
    {
        protected AdamTransactionBase(string logName) : base(logName)
        {
        }

        public T Init(IBlock block, int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot, ILog parentLog) 
            
        {
            Log.LinkTo(parentLog);
            var logCall = Log.Call<T>($"app: {appId}, type: {contentType}, itemGuid: {itemGuid}, field: {field}, portalRoot: {usePortalRoot}");
            State = new AdamState(block, appId, contentType, field, itemGuid, usePortalRoot, Log);
            return logCall(null, this as T);
        }

        protected AdamState State;


        /// <summary>
        /// Validate that user has write permissions for folder.
        /// In case the primary file system is used (usePortalRoot) then also check higher permissions
        /// </summary>
        /// <param name="state"></param>
        /// <param name="parentFolder"></param>
        /// <param name="target"></param>
        /// <param name="folderId"></param>
        /// <param name="errPrefix"></param>
        [AssertionMethod]
        protected static void VerifySecurityAndStructure(AdamState state, IAsset parentFolder, IAsset target, int folderId, string errPrefix)
        {
            // In case the primary file system is used (usePortalRoot) then also check higher permissions
            if (state.UseTenantRoot && !state.Security.CanEditFolder(folderId))
                throw HttpException.PermissionDenied(errPrefix + " - permission denied");

            if (!state.Security.SuperUserOrAccessingItemFolder(target.Path, out var exp))
                throw exp;

            if (target.ParentId != parentFolder.Id)
                throw HttpException.BadRequest(errPrefix + " - not found in folder");
        }
    }
}
