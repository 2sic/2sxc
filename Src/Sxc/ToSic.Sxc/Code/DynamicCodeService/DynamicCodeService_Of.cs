using ToSic.Eav.Apps;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Code;

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
        var wrapLog = Log.Fn<IDynamicCodeRoot>($"{pageId}, {moduleId}");
        MakeSureLogIsInHistory();
        ActivateEditUi();
        var cmsBlock = _myScopedServices.ModAndBlockBuilder.Value.GetProvider(pageId, moduleId).LoadBlock();
        var codeRoot = _myScopedServices.CodeRootGenerator.New()
            .BuildCodeRoot(customCodeOrNull: null, cmsBlock, Log, CompatibilityLevels.CompatibilityLevel12);

        return wrapLog.ReturnAsOk(codeRoot);
    }

    /// <inheritdoc />
    public IDynamicCode12 OfSite() => OfApp(GetPrimaryAppIdentity(null, Services.Site.Value));

    /// <inheritdoc />
    public IDynamicCode12 OfSite(int siteId) => OfApp(GetPrimaryAppIdentity(siteId, null));

    private IDynamicCode12 OfAppInternal(int? zoneId = null, int? appId = null)
    {
        var wrapLog = Log.Fn<IDynamicCode12>();
        MakeSureLogIsInHistory();
        ActivateEditUi();
        var codeRoot = _myScopedServices.CodeRootGenerator.New()
            .BuildCodeRoot(customCodeOrNull: null, null, Log, CompatibilityLevels.CompatibilityLevel12);
        var app = App(zoneId: zoneId, appId: appId);
        ((IDynamicCodeRootInternal)codeRoot).AttachApp(app);
        return wrapLog.ReturnAsOk(codeRoot);
    }
}