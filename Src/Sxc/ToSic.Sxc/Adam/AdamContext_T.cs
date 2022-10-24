using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Adam
{
    /// <inheritdoc />
    public class AdamContext<TFolderId, TFileId>: AdamContext
    {
        internal AdamManager<TFolderId, TFileId> AdamManager => _adamManagerLazy.Value;
        private readonly Lazy<AdamManager<TFolderId, TFileId>> _adamManagerLazy;

        public AdamContext(Lazy<AdamManager<TFolderId, TFileId>> adamManagerLazy, IServiceProvider serviceProvider): base(serviceProvider, "Adm.CtxTT")
        {
            _adamManagerLazy = adamManagerLazy;
        }

        internal AdamStorage<TFolderId, TFileId> AdamRoot;

        public override AdamContext Init(IContextOfApp context, string contentType, string fieldName, Guid entityGuid, bool usePortalRoot, ILog parentLog)
        {
            Log.LinkTo(parentLog);
            var logCall = Log.Fn<AdamContext>($"..., usePortalRoot: {usePortalRoot}");
            AdamManager.Init(context, Constants.CompatibilityLevel10, Log);
            AdamRoot = usePortalRoot
                ? new AdamStorageOfSite<TFolderId, TFileId>(AdamManager) as AdamStorage<TFolderId, TFileId>
                : new AdamStorageOfField<TFolderId, TFileId>(AdamManager, entityGuid, fieldName);
            AdamRoot.Init(Log);

            base.Init(context, contentType, fieldName, entityGuid, usePortalRoot, parentLog);
            
            return logCall.Return(this);
        }


        // temp
        public override AppRuntime AppRuntime => AdamManager.AppRuntime;

    }
}
