using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Internal;
using static ToSic.Eav.Code.Infos.CodeInfoObsolete;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;
using ILog = ToSic.Lib.Logging.ILog;

namespace ToSic.Sxc.Dnn;

/// <summary>
/// This is a factory to create CmsBlocks, Apps etc. and related objects from DNN.
/// </summary>
[PublicApi]
[Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
public static class Factory
{
    /// <summary>
    /// Workaround - static build should actually be completely deprecated, but as it's not possible yet,
    /// we'll provide this for now
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
#pragma warning disable CS0618
    private static T StaticBuild<T>(ILog parentLog = null) => DnnStaticDi.StaticBuild<T>(parentLog); // Important! Never call NewLog() here - Stack Overflow
#pragma warning restore CS0618

    private static ILog NewLog()
    {
        var log = new Log("Dnn.Factor");
        StaticBuild<ILogStore>(log).Add("obsolete-dnn-factory", log);
        return log;
    }

    /// <summary>
    /// Get a Root CMS Block if you know the TabId and the ModId
    /// </summary>
    /// <param name="pageId">The DNN tab id (page id)</param>
    /// <param name="modId">The DNN Module id</param>
    /// <returns>An initialized CMS Block, ready to use/render</returns>
    [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
    public static IBlockBuilder CmsBlock(int pageId, int modId) => CmsBlock(pageId, modId, NewLog());

    /// <summary>
    /// Get a Root CMS Block if you know the TabId and the ModId
    /// </summary>
    /// <param name="pageId">The DNN tab id (page id)</param>
    /// <param name="modId">The DNN Module id</param>
    /// <param name="parentLog">The parent log, optional</param>
    /// <returns>An initialized CMS Block, ready to use/render</returns>
    [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
    public static IBlockBuilder CmsBlock(int pageId, int modId, ILog parentLog)
    {
        var l = parentLog.Fn<IBlockBuilder>($"{pageId}, {modId}");
        DnnStaticDi.CodeInfos.Warn(V13To17($"ToSic.Sxc.Dnn.Factory.{nameof(CmsBlock)}", "https://go.2sxc.org/brc-13-dnn-factory"));
        return l.ReturnAsOk(StaticBuild<IModuleAndBlockBuilder>(parentLog).BuildBlock(pageId, modId).BlockBuilder);
    }

    /// <summary>
    /// Get a Root CMS Block if you have the ModuleInfo object
    /// </summary>
    /// <param name="moduleInfo">A DNN ModuleInfo object</param>
    /// <returns>An initialized CMS Block, ready to use/render</returns>
    [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
    public static IBlockBuilder CmsBlock(ModuleInfo moduleInfo)
        => CmsBlock(((DnnModule)StaticBuild<IModule>()).Init(moduleInfo));

    /// <summary>
    /// Get a Root CMS Block if you have the ModuleInfo object.
    /// </summary>
    /// <param name="module"></param>
    /// <param name="parentLog">optional logger to attach to</param>
    /// <returns>An initialized CMS Block, ready to use/render</returns>
    [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
    public static IBlockBuilder CmsBlock(IModule module, ILog parentLog = null)
    {
        DnnStaticDi.CodeInfos.Warn(V13To17($"ToSic.Sxc.Dnn.Factory.{nameof(CmsBlock)}", "https://go.2sxc.org/brc-13-dnn-factory"));
        parentLog = parentLog ?? NewLog();
        var dnnModule = ((Module<ModuleInfo>)module)?.GetContents();
        return StaticBuild<IModuleAndBlockBuilder>(parentLog).BuildBlock(dnnModule, null).BlockBuilder;
    }

    /// <summary>
    /// Retrieve a helper object which provides commands like AsDynamic, AsEntity etc.
    /// </summary>
    /// <param name="blockBuilder">CMS Block for which the helper is targeted. </param>
    /// <returns>A Code Helper based on <see cref="IDnnDynamicCode"/></returns>
    [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
    public static IDnnDynamicCode DynamicCode(IBlockBuilder blockBuilder)
    {
        DnnStaticDi.CodeInfos.Warn(V13To17($"ToSic.Sxc.Dnn.Factory.{nameof(DynamicCode)}", "https://go.2sxc.org/brc-13-dnn-factory"));
        return StaticBuild<CodeApiServiceFactory>()
                .BuildCodeRoot(customCodeOrNull: null, blockBuilder.Block, NewLog(), CompatibilityLevels.CompatibilityLevel10) as DnnCodeApiService;
    }

    /// <summary>
    /// Get a full app-object for accessing data of the app from outside
    /// </summary>
    /// <param name="appId">The AppID of the app you need</param>
    /// <param name="unusedButKeepForApiStability">
    /// Tells the App that you'll be using page-publishing.
    /// So changes will me auto-drafted for a future release as the whole page together.
    /// </param>
    /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
    /// <param name="parentLog">optional logger to attach to</param>
    /// <returns>An initialized App object which you can use to access App.Data</returns>
    [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
    public static IApp App(int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false, ILog parentLog = null)
        => App(AutoLookupZoneId, appId, null, showDrafts, parentLog ?? NewLog());

    /// <summary>
    /// Get a full app-object for accessing data of the app from outside
    /// </summary>
    /// <param name="zoneId">The zone the app is in.</param>
    /// <param name="appId">The AppID of the app you need</param>
    /// <param name="unusedButKeepForApiStability">
    /// Tells the App that you'll be using page-publishing.
    /// So changes will me auto-drafted for a future release as the whole page together.
    /// </param>
    /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
    /// <param name="parentLog">optional logger to attach to</param>
    /// <returns>An initialized App object which you can use to access App.Data</returns>
    [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
    public static IApp App(int zoneId, int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false, ILog parentLog = null)
        => App(zoneId, appId, null, showDrafts, parentLog ?? NewLog());

    /// <summary>
    /// Get a full app-object for accessing data of the app from outside
    /// </summary>
    /// <param name="appId">The AppID of the app you need</param>
    /// <param name="ownerPortalSettings">The owner portal - this is important when retrieving Apps from another portal.</param>
    /// <param name="unusedButKeepForApiStability">
    /// Tells the App that you'll be using page-publishing.
    /// So changes will me auto-drafted for a future release as the whole page together.
    /// </param>
    /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
    /// <param name="parentLog">optional logger to attach to</param>
    /// <returns>An initialized App object which you can use to access App.Data</returns>
    [Obsolete("This is obsolete in V13 but will continue to work for now, we plan to remove in v15 or 16. Use the IDynamicCodeService or the IRenderService instead.")]
    public static IApp App(int appId,
        PortalSettings ownerPortalSettings,
        bool unusedButKeepForApiStability = false,
        bool showDrafts = false,
        ILog parentLog = null)
        => App(AutoLookupZoneId, appId, GetSite(ownerPortalSettings, parentLog), showDrafts, parentLog);

    private static ISite GetSite(PortalSettings altPortalSettings = default, ILog log = default)
    {
        var portalSettings = altPortalSettings ?? PortalSettings.Current;
        var l = log.Fn<ISite>($"{nameof(altPortalSettings)}: {altPortalSettings?.PortalId}; {nameof(portalSettings)}: {portalSettings.PortalId}");
        var site = ((DnnSite)StaticBuild<ISite>()).TryInitPortal(altPortalSettings, log);
        return l.Return(site);
    }

    [InternalApi_DoNotUse_MayChangeWithoutNotice]
    private static IApp App(
        int zoneId,
        int appId,
        ISite site,
        bool showDrafts,
        ILog parentLog)
    {
        DnnStaticDi.CodeInfos.Warn(V13To17($"ToSic.Sxc.Dnn.Factory.{nameof(App)}", "https://go.2sxc.org/brc-13-dnn-factory"));
        var log = new Log("Dnn.Factry", parentLog ?? NewLog());
        log.A($"Create App(z:{zoneId}, a:{appId}, tenantObj:{site != null}, showDrafts: {showDrafts}, parentLog: {parentLog != null})");
        var app = StaticBuild<App>(log);
        site ??= GetSite(log: log);
        var appIdentity = zoneId == AutoLookupZoneId ? new(site.ZoneId, appId) : new AppIdentityPure(zoneId, appId);
        app.Init(site, appIdentity, new() { ShowDrafts = showDrafts });
        return app;
    }

    private const int AutoLookupZoneId = -999;
}