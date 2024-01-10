using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Apps.Work;
using ToSic.Eav.Security.Permissions;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Backend.InPage;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AppViewPickerBackend: BlockWebApiBackendBase
{
    public AppViewPickerBackend(
        Generator<MultiPermissionsApp> multiPermissionsApp,
        ISxcContextResolver ctxResolver,
        LazySvc<BlockEditorSelector> blockEditorSelectorLazy,
        GenWorkPlus<WorkViews> workViews,
        AppWorkContextService appWorkCtxService,
        GenWorkDb<WorkEntityPublish> publisher
    ) : base(multiPermissionsApp, appWorkCtxService, ctxResolver,"Bck.ViwApp")
    {
        ConnectServices(
            _workViews = workViews,
            _publisher = publisher,
            _blockEditorSelectorLazy = blockEditorSelectorLazy
        );
    }

    private readonly GenWorkPlus<WorkViews> _workViews;
    private readonly GenWorkDb<WorkEntityPublish> _publisher;
    private readonly LazySvc<BlockEditorSelector> _blockEditorSelectorLazy;

    public void SetAppId(int? appId) => _blockEditorSelectorLazy.Value.GetEditor(Block).SetAppId(appId);

    public IEnumerable<TemplateUiInfo> Templates() =>
        Block?.App == null 
            ? Array.Empty<TemplateUiInfo>()
            : _workViews.New(AppWorkCtxPlus).GetCompatibleViews(Block?.App, Block?.Configuration);

    public IEnumerable<ContentTypeUiInfo> ContentTypes()
    {
        // nothing to do without app
        if (Block?.App == null) return null;
        return _workViews.New(AppWorkCtxPlus).GetContentTypesWithStatus(Block.App.Path ?? "", Block.App.PathShared ?? "");
    }

    public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
    {
        var callLog = Log.Fn<Guid?>($"{templateId}, {forceCreateContentGroup}");
        ThrowIfNotAllowedInApp(GrantSets.WriteSomething);
        return callLog.ReturnAsOk(_blockEditorSelectorLazy.Value.GetEditor(Block).SaveTemplateId(templateId, forceCreateContentGroup));
    }

    public bool Publish(int id)
    {
        var callLog = Log.Fn<bool>($"{id}");
        ThrowIfNotAllowedInApp(GrantSets.WritePublished);
        _publisher.New(AppWorkCtx).Publish(id);
        return callLog.ReturnTrue("ok");
    }
}