using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Adam
{
    /// <inheritdoc />
    public class AdamContext<TFolderId, TFileId>: AdamContext
    {
        internal AdamManager<TFolderId, TFileId> AdamManager => _adamAppContext.Value;
        private readonly Lazy<AdamManager<TFolderId, TFileId>> _adamAppContext;

        public AdamContext(Lazy<AdamManager<TFolderId, TFileId>> adamAppContext, IServiceProvider serviceProvider): base(serviceProvider, "Adm.CtxTT")
        {
            _adamAppContext = adamAppContext;
        }

        internal AdamOfBase<TFolderId, TFileId> ContainerContext;

        public override AdamContext Init(IContextOfApp context, string contentType, string fieldName, Guid entityGuid, bool usePortalRoot, ILog parentLog)
        {
            Log.Add("PrepCore(...)");
            AdamManager.Init(context, 10, Log);
            ContainerContext = usePortalRoot
                ? new AdamOfSite<TFolderId, TFileId>(AdamManager) as AdamOfBase<TFolderId, TFileId>
                : new AdamOfField<TFolderId, TFileId>(AdamManager, entityGuid, fieldName);

            return base.Init(context, contentType, fieldName, entityGuid, usePortalRoot, parentLog);
        }


        // temp
        public override AppRuntime AppRuntime => AdamManager.AppRuntime;

    }
}
