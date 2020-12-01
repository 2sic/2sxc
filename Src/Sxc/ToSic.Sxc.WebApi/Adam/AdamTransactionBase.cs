using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Adam
{
    public abstract partial class AdamTransactionBase<T, TFolderId, TFileId>: HasLog where T : class
    {
        private readonly Lazy<AdamState<TFolderId, TFileId>> _adamState;

        #region Constructor / DI

        protected AdamTransactionBase(Lazy<AdamState<TFolderId, TFileId>> adamState, string logName) : base(logName)
        {
            _adamState = adamState;
        }

        public T Init(IContextOfApp context, int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot, ILog parentLog) 
            
        {
            Log.LinkTo(parentLog);
            var logCall = Log.Call<T>($"app: {appId}, type: {contentType}, itemGuid: {itemGuid}, field: {field}, portalRoot: {usePortalRoot}");
            //State = new AdamState<TFolderId, TFileId>();
            State.Init(context, appId, contentType, field, itemGuid, usePortalRoot, Log);
            return logCall(null, this as T);
        }

        protected AdamState<TFolderId, TFileId> State => _adamState.Value;

        #endregion

        /// <summary>
        /// Validate that user has write permissions for folder.
        /// In case the primary file system is used (usePortalRoot) then also check higher permissions
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="target"></param>
        /// <param name="errPrefix"></param>
        [AssertionMethod]
        protected void VerifySecurityAndStructure(Folder<TFolderId, TFileId> parentFolder, IAssetWithParentSysId<TFolderId> target, string errPrefix)
        {
            // In case the primary file system is used (usePortalRoot) then also check higher permissions
            if (State.UseSiteRoot && !State.Security.CanEditFolder(target)) 
                throw HttpException.PermissionDenied(errPrefix + " - permission denied");

            if (!State.Security.SuperUserOrAccessingItemFolder(target.Path, out var exp))
                throw exp;

            if(!EqualityComparer<TFolderId>.Default.Equals(target.ParentSysId, parentFolder.SysId))
                throw HttpException.BadRequest(errPrefix + " - not found in folder");
        }
    }
}
