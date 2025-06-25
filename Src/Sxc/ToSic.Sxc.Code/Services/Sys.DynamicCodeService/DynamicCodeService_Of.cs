﻿using ToSic.Eav.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Services.Internal;

/// <summary>
/// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
/// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
/// </summary>
public partial class DynamicCodeService
{
    /// <inheritdoc />
    public IDynamicCode12 OfApp(int appId) => OfAppInternal(appId: appId);

    /// <inheritdoc />
    public IDynamicCode12 OfApp(int zoneId, int appId) => OfAppInternal(zoneId: zoneId, appId: appId);

    /// <inheritdoc />
    public IDynamicCode12 OfApp(IAppIdentity appIdentity) => OfAppInternal(zoneId: appIdentity.ZoneId, appId: appIdentity.AppId);

    /// <inheritdoc />
    public IDynamicCode12 OfModule(int pageId, int moduleId)
    {
        var l = Log.Fn<IDynamicCode12>($"{pageId}, {moduleId}");
        MakeSureLogIsInHistory();
        ActivateEditUi();
        var cmsBlock = _myScopedServices.ModAndBlockBuilder.Value.BuildBlock(pageId, moduleId);
        var codeRoot = _myScopedServices.CodeRootGenerator.New()
            .New(parentClassOrNull: null, cmsBlock, Log, CompatibilityLevels.CompatibilityLevel12);

        var code12 = new DynamicCode12Proxy(codeRoot, ((ExecutionContext)codeRoot).DynamicApi);
        return l.ReturnAsOk(code12);
    }

    /// <inheritdoc />
    public IDynamicCode12 OfSite() => OfApp(GetPrimaryAppIdentity(null));

    /// <inheritdoc />
    public IDynamicCode12 OfSite(int siteId) => OfApp(GetPrimaryAppIdentity(siteId));

    private IDynamicCode12 OfAppInternal(int? zoneId = null, int? appId = null)
    {
        var l = Log.Fn<IDynamicCode12>();
        MakeSureLogIsInHistory();
        ActivateEditUi();
        var codeRoot = _myScopedServices.CodeRootGenerator.New()
            .New(parentClassOrNull: null, null, Log, CompatibilityLevels.CompatibilityLevel12);
        var app = App(zoneId: zoneId, appId: appId);
        ((IExCtxAttachApp)codeRoot).AttachApp(app);
        var code12 = new DynamicCode12Proxy(codeRoot, ((ExecutionContext)codeRoot).DynamicApi);
        return l.ReturnAsOk(code12);
    }
}