using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Adam
{
    /// <inheritdoc />
    public class AdamContext<TFolderId, TFileId>: AdamContext
    {
        private readonly Generator<AdamStorageOfSite<TFolderId, TFileId>> _siteStoreGenerator;
        private readonly Generator<AdamStorageOfField<TFolderId, TFileId>> _fieldStoreGenerator;
        internal AdamManager<TFolderId, TFileId> AdamManager => _adamManagerLazy.Value;
        private readonly LazySvc<AdamManager<TFolderId, TFileId>> _adamManagerLazy;

        public AdamContext(
            LazySvc<AdamManager<TFolderId, TFileId>> adamManagerLazy,
            Generator<AdamStorageOfSite<TFolderId, TFileId>> siteStoreGenerator,
            Generator<AdamStorageOfField<TFolderId, TFileId>> fieldStoreGenerator,
            Dependencies dependencies): base(dependencies, "Adm.CtxTT")
        {
            ConnectServices(
                _adamManagerLazy = adamManagerLazy,
                _siteStoreGenerator = siteStoreGenerator,
                _fieldStoreGenerator = fieldStoreGenerator
            );
        }

        internal AdamStorage<TFolderId, TFileId> AdamRoot;

        public override AdamContext Init(IContextOfApp context, string contentType, string fieldName, Guid entityGuid, bool usePortalRoot)
        {
            var logCall = Log.Fn<AdamContext>($"..., usePortalRoot: {usePortalRoot}");
            AdamManager.Init(context, Constants.CompatibilityLevel10);
            AdamRoot = usePortalRoot
                ? _siteStoreGenerator.New() as AdamStorage<TFolderId, TFileId>
                : _fieldStoreGenerator.New().InitItemAndField(entityGuid, fieldName);
            AdamRoot.Init(AdamManager);

            base.Init(context, contentType, fieldName, entityGuid, usePortalRoot);
            
            return logCall.Return(this);
        }


        // temp
        public override AppRuntime AppRuntime => AdamManager.AppRuntime;

    }
}
