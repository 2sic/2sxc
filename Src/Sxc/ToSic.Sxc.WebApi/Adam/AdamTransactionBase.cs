using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Assets;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.Adam;
using ToSic.Eav.WebApi.Errors;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.Adam
{
    public abstract partial class AdamTransactionBase<T, TFolderId, TFileId>: ServiceBase, IAdamTransactionBase where T : AdamTransactionBase<T, TFolderId, TFileId>
    {
        #region Constructor / DI

        protected AdamTransactionBase(ILazySvc<AdamContext<TFolderId, TFileId>> adamState, IContextResolver ctxResolver, string logName) : base(logName) =>
            ConnectServices(
                _adamState = adamState,
                _ctxResolver = ctxResolver
            );
        private readonly ILazySvc<AdamContext<TFolderId, TFileId>> _adamState;
        private readonly IContextResolver _ctxResolver;

        public T Init(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot, ILog parentLog)
        {
            this.Init(parentLog);
            var context = appId > 0 ? _ctxResolver.BlockOrApp(appId) : _ctxResolver.AppNameRouteBlock(null);
            var logCall = Log.Fn<T>($"app: {context.AppState.Show()}, type: {contentType}, itemGuid: {itemGuid}, field: {field}, portalRoot: {usePortalRoot}");
            AdamContext.Init(context, contentType, field, itemGuid, usePortalRoot, Log);
            return logCall.Return(this as T);
        }

        void IAdamTransactionBase.Init(int appId, string contentType, Guid itemGuid, string field, bool usePortalRoot, ILog parentLog)
        {
            Init(appId, contentType, itemGuid, field, usePortalRoot, parentLog);
        }


        protected AdamContext<TFolderId, TFileId> AdamContext => _adamState.Value;

        #endregion

        /// <summary>
        /// Validate that user has write permissions for folder.
        /// In case the primary file system is used (usePortalRoot) then also check higher permissions
        /// </summary>
        /// <param name="parentFolder"></param>
        /// <param name="target"></param>
        /// <param name="errPrefix"></param>
        [AssertionMethod]
        protected void VerifySecurityAndStructure(Eav.Apps.Assets.Folder<TFolderId, TFileId> parentFolder, IAssetWithParentSysId<TFolderId> target, string errPrefix)
        {
            // In case the primary file system is used (usePortalRoot) then also check higher permissions
            if (AdamContext.UseSiteRoot && !AdamContext.Security.CanEditFolder(target)) 
                throw HttpException.PermissionDenied(errPrefix + " - permission denied");

            if (!AdamContext.Security.SuperUserOrAccessingItemFolder(target.Path, out var exp))
                throw exp;

            if(!EqualityComparer<TFolderId>.Default.Equals(target.ParentSysId, parentFolder.SysId))
                throw HttpException.BadRequest(errPrefix + " - not found in folder");
        }

    }
}
