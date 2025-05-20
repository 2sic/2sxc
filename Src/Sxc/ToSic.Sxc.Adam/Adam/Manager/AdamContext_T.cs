using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Adam.Internal;

/// <inheritdoc />
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamContext<TFolderId, TFileId>(
    LazySvc<AdamManager> adamManagerLazy,
    Generator<AdamStorageOfSite> siteStorageGen,
    Generator<AdamStorageOfField> fieldStorageGen,
    AdamContext.MyServices services)
    : AdamContext(services, "Adm.CtxTT", connect: [adamManagerLazy, siteStorageGen, fieldStorageGen])
{
    public AdamManager AdamManager => adamManagerLazy.Value;
    public override AdamManager AdamManagerSimple => AdamManager;

    public AdamStorage AdamRoot;

    // TODO: @2dm #AdamTyped
    public override AdamContext Init(IContextOfApp context, string contentType, string fieldName, Guid entityGuid, bool usePortalRoot, ICodeDataFactory cdf)
    {
        var logCall = Log.Fn<AdamContext>($"..., usePortalRoot: {usePortalRoot}");
        AdamManager.Init(context, cdf, CompatibilityLevels.CompatibilityLevel10);
        AdamRoot = usePortalRoot
            ? siteStorageGen.New()
            : fieldStorageGen.New().InitItemAndField(entityGuid, fieldName);
        AdamRoot.Init(AdamManager);

        InitBase(context, contentType, fieldName, entityGuid, usePortalRoot, cdf);
            
        return logCall.Return(this);
    }


    // temp

    public override IAppWorkCtx AppWorkCtx => AdamManager.AppWorkCtx;
}