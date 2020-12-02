using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi.Errors;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Adam
{
    public abstract partial class AdamTransactionBase<T, TFolderId, TFileId>: HasLog where T : class
    {
        private readonly Lazy<AdamState<TFolderId, TFileId>> _adamState;
        private readonly IContextResolver _ctxResolver;

        #region Constructor / DI

        protected AdamTransactionBase(Lazy<AdamState<TFolderId, TFileId>> adamState, IContextResolver ctxResolver, string logName) : base(logName)
        {
            _adamState = adamState;
            _ctxResolver = ctxResolver.Init(Log);
        }

        public T Init(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot, ILog parentLog) 
            
        {
            Log.LinkTo(parentLog);
            var context = _ctxResolver.App(appId);
            var logCall = Log.Call<T>($"app: {context.AppState.Show()}, type: {contentType}, itemGuid: {itemGuid}, field: {field}, portalRoot: {usePortalRoot}");
            State.Init(context, contentType, field, itemGuid, usePortalRoot, Log);
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
