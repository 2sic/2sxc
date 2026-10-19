using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Sys;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys.Module;
using ISite = ToSic.Eav.Context.ISite;

namespace ToSic.Sxc.Dnn.Context;

/// <summary>
/// The DNN implementation of a Block Container (a Module).
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
[PrivateApi("this is just internal, external users don't really have anything to do with this")]
public class DnnModuleFactory(IAppsCatalog appsCatalog, LazySvc<AppFinder> appFinderLazy, ISite site)
    : ServiceBase("Dnn.Contnr", connect: [appsCatalog, appFinderLazy, site]), IModuleFactory
{
    /// <summary>
    /// We don't use a Constructor because of DI
    /// So you must always call Init
    /// </summary>
    public IModule Get(int moduleId)
    {
        var l = Log.Fn<IModule>($"{moduleId}");
        var mod = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, false);
        return l.Return(Get(mod));
    }

    /// <summary>
    /// We don't use a Constructor because of DI
    /// So you must always call Init
    /// </summary>
    public IModule Get(ModuleInfo moduleInfo)
    {
        var l = Log.Fn<IModule>($"moduleId: {moduleInfo?.ModuleID}");
        //Init(moduleInfo);

        var id = moduleInfo?.ModuleID ?? EavConstants.NullId;
        var isContent = (moduleInfo?.DesktopModule.ModuleName ?? "2sxc") == "2sxc";
        var blockIdentifier = BlockIdentifier(moduleInfo, isContent);
        var modResult = new ModuleRecord<ModuleInfo>(Id: id, IsContent: isContent, BlockIdentifier: blockIdentifier)
        {
            Contents = moduleInfo,
        };
        return l.ReturnAsOk(modResult);
    }

    #region Constructors and DI

    //public new DnnModule Init(ModuleInfo moduleInfo)
    //{
    //    var l = Log.Fn<DnnModule>($"moduleId: {moduleInfo?.ModuleID}");
    //    base.Init(moduleInfo);
    //    return l.ReturnAsOk(this);
    //}

    //public /*override*/ void Init(int moduleId)
    //{
    //    var l = Log.Fn($"{moduleId}");
    //    var mod = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, false);
    //    Init(mod);
    //    l.Done();
    //}

    #endregion


    ///// <inheritdoc />
    //public override int Id => UnwrappedModule?.ModuleID ?? EavConstants.NullId;


    ///// <inheritdoc />
    //public override bool IsContent => (UnwrappedModule?.DesktopModule.ModuleName ?? "2sxc") == "2sxc";


    ///// <inheritdoc />
    public /*override*/ IBlockIdentifier BlockIdentifier(ModuleInfo moduleInfo, bool isContent)
    {
        //if (field != null)
        //    return field;
        if (moduleInfo == null!)
            return null;

        // find ZoneId, AppId and prepare settings for next values
        // note: this is the correct zone, even if the module is shared from another portal, because the Site is prepared correctly
        var zoneId = site.ZoneId;
        var (appId, appNameId) = GetInstanceAppIdAndName(moduleInfo, zoneId, isContent);
        var settings = moduleInfo.ModuleSettings;

        // find block identifier
        Guid.TryParse(settings[ModuleSettingNames.ContentGroup]?.ToString(), out var blockGuid);

        // Check if we have preview-view identifier - for blocks which don't exist yet
        var previewTemplateString = settings[ModuleSettingNames.PreviewView]?.ToString();
        var overrideView = !string.IsNullOrEmpty(previewTemplateString)
            ? Guid.Parse(previewTemplateString)
            : new();

        // Create identifier
        return /*field =*/ new BlockIdentifier(zoneId, appId, appNameId, blockGuid, overrideView);
    }

    private (int AppId, string AppNameId) GetInstanceAppIdAndName(ModuleInfo moduleInfo, int zoneId, bool isContent)
    {
        var l = Log.Fn<(int, string)>($"{zoneId}");
        var module = moduleInfo ?? throw new("instance is not ModuleInfo");
        var msg = $"get appid from instance for Z:{zoneId} Mod:{module.ModuleID}";
        if (isContent)
        {
            var appId = appsCatalog.DefaultAppIdentity(zoneId).AppId;
            return l.Return((appId, "Content"), $"{msg} - use Default app: {appId}");
        }

        if (module.ModuleSettings.ContainsKey(ModuleSettingNames.AppName))
        {
            var guid = module.ModuleSettings[ModuleSettingNames.AppName].ToString();
            var appId = appFinderLazy.Value.FindAppId(zoneId, guid);
            return l.Return((appId, guid), $"{msg} AppG:{guid} = app:{appId}");
        }

        Log.A($"{msg} not found = null");
        return l.Return((KnownAppsConstants.AppIdEmpty, KnownAppsConstants.AppNameIdEmpty), "not found");
    }

}