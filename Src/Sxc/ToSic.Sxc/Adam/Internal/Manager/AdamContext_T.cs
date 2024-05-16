using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Data;
using ToSic.Sxc.Internal;
using CodeDataFactory = ToSic.Sxc.Data.Internal.CodeDataFactory;

namespace ToSic.Sxc.Adam.Internal;

/// <inheritdoc />
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        MyServices services): base(services, "Adm.CtxTT")
    {
        ConnectLogs([
            _adamManagerLazy = adamManagerLazy,
            _siteStoreGenerator = siteStoreGenerator,
            _fieldStoreGenerator = fieldStoreGenerator
        ]);
    }

    internal AdamStorage<TFolderId, TFileId> AdamRoot;

    // TODO: @2dm #AdamTyped
    public override AdamContext Init(IContextOfApp context, string contentType, string fieldName, Guid entityGuid,
        bool usePortalRoot, CodeDataFactory cdf)
    {
        var logCall = Log.Fn<AdamContext>($"..., usePortalRoot: {usePortalRoot}");
        AdamManager.Init(context, cdf, CompatibilityLevels.CompatibilityLevel10);
        AdamRoot = usePortalRoot
            ? _siteStoreGenerator.New() as AdamStorage<TFolderId, TFileId>
            : _fieldStoreGenerator.New().InitItemAndField(entityGuid, fieldName);
        AdamRoot.Init(AdamManager);

        base.Init(context, contentType, fieldName, entityGuid, usePortalRoot, cdf);
            
        return logCall.Return(this);
    }


    // temp

    public override IAppWorkCtx AppWorkCtx => AdamManager.AppWorkCtx;
}