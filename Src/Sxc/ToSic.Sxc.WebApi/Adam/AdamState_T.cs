using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.Adam
{
    internal class AdamState<TFolderId, TFileId>: AdamState
    {
        public AdamState(IBlock block, int appId, string contentType, string field, Guid guid, bool usePortalRoot, ILog log) 
            : base(block, appId, contentType, field, guid, usePortalRoot, log)
        {
        }

        internal AdamAppContext<TFolderId, TFileId> AdamAppContext;

        internal AdamOfBase<TFolderId, TFileId> ContainerContext;


        protected override void PrepCore(IApp app, Guid entityGuid, string fieldName, bool usePortalRoot)
        {
            Log.Add("PrepCore(...)");
            AdamAppContext = new AdamAppContext<TFolderId, TFileId>();
            AdamAppContext.Init(Permissions.Context.Tenant, app, Block, 10, Log);
                ContainerContext = usePortalRoot
                ? new AdamOfSite<TFolderId, TFileId>(AdamAppContext) as AdamOfBase<TFolderId, TFileId>
                : new AdamOfField<TFolderId, TFileId>(AdamAppContext, entityGuid, fieldName);
        }

        // temp
        public override AppRuntime AppRuntime => AdamAppContext.AppRuntime;

    }
}
