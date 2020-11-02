using System;
using ToSic.Eav.Apps;
using ToSic.Sxc.Adam;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.WebApi.Adam
{
    public class AdamState<TFolderId, TFileId>: AdamState
    {
        private readonly Lazy<AdamAppContext<TFolderId, TFileId>> _adamAppContext;
        internal AdamAppContext<TFolderId, TFileId> AdamAppContext => _adamAppContext.Value;
        public AdamState(Lazy<AdamAppContext<TFolderId, TFileId>> adamAppContext): base("Adm.StatTT")
        {
            _adamAppContext = adamAppContext;
        }

        //public AdamState<TFolderId, TFileId> Init(IBlock block, int appId, string contentType, string field, Guid guid, bool usePortalRoot, ILog log) 
        //    : base("Adm.StatTT") // block, appId, contentType, field, guid, usePortalRoot, log)
        //{
        //    base.Init(block, appId, contentType, field, guid, usePortalRoot, log);
        //}

        //internal AdamAppContext<TFolderId, TFileId> AdamAppContext;

        internal AdamOfBase<TFolderId, TFileId> ContainerContext;


        protected override void Init(IApp app, Guid entityGuid, string fieldName, bool usePortalRoot)
        {
            Log.Add("PrepCore(...)");
            //AdamAppContext = new AdamAppContext<TFolderId, TFileId>();
            AdamAppContext.Init(Permissions.Context.Tenant, app, Block, 10, Log);
                ContainerContext = usePortalRoot
                ? new AdamOfSite<TFolderId, TFileId>(AdamAppContext) as AdamOfBase<TFolderId, TFileId>
                : new AdamOfField<TFolderId, TFileId>(AdamAppContext, entityGuid, fieldName);
        }

        // temp
        public override AppRuntime AppRuntime => AdamAppContext.AppRuntime;

    }
}
