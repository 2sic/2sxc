using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Cms.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Internal;
using ISite = ToSic.Eav.Context.ISite;

namespace ToSic.Sxc.Dnn.Context;

/// <summary>
/// The DNN implementation of a Block Container (a Module).
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
[PrivateApi("this is just internal, external users don't really have anything to do with this")]
public class DnnModule: Module<ModuleInfo>
{
    #region Constructors and DI
        
    public DnnModule(IAppsCatalog appsCatalog, LazySvc<AppFinder> appFinderLazy, ISite site): base("Dnn.Contnr")
    {
        ConnectLogs([
            _appsCatalog = appsCatalog,
            _appFinderLazy = appFinderLazy,
        _site = site,
        ]);
    }

    private readonly IAppsCatalog _appsCatalog;
    private readonly LazySvc<AppFinder> _appFinderLazy;
    private readonly ISite _site;

    /// <summary>
    /// We don't use a Constructor because of DI
    /// So you must always call Init
    /// </summary>
    public new DnnModule Init(ModuleInfo item)
    {
        var l = Log.Fn<DnnModule>($"{item?.ModuleID}");
        base.Init(item);
        return l.ReturnAsOk(this);
    }

    /// <summary>
    /// We don't use a Constructor because of DI
    /// So you must always call Init
    /// </summary>
    public override IModule Init(int moduleId)
    {
        var l = Log.Fn<IModule>($"{moduleId}");
        var mod = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, false);
        Init(mod);
        return l.ReturnAsOk(this);
    }

    #endregion


    /// <inheritdoc />
    public override int Id => UnwrappedModule?.ModuleID ?? Eav.Constants.NullId;


    /// <inheritdoc />
    public override bool IsContent => (UnwrappedModule?.DesktopModule.ModuleName ?? "2sxc") == "2sxc";


    /// <inheritdoc />
    public override IBlockIdentifier BlockIdentifier
    {
        get
        {
            if (_blockIdentifier != null) return _blockIdentifier;
            if (UnwrappedModule == null) return null;

            // find ZoneId, AppId and prepare settings for next values
            // note: this is the correct zone, even if the module is shared from another portal, because the Site is prepared correctly
            var zoneId = _site.ZoneId;
            var (appId, appNameId) = GetInstanceAppIdAndName(zoneId);
            var settings = UnwrappedModule.ModuleSettings;

            // find block identifier
            Guid.TryParse(settings[ModuleSettingNames.ContentGroup]?.ToString(), out var blockGuid);

            // Check if we have preview-view identifier - for blocks which don't exist yet
            var previewTemplateString = settings[ModuleSettingNames.PreviewView]?.ToString();
            var overrideView = !string.IsNullOrEmpty(previewTemplateString)
                ? Guid.Parse(previewTemplateString)
                : new();

            // Create identifier
            return _blockIdentifier = new BlockIdentifier(zoneId, appId, appNameId, blockGuid, overrideView);
        }
    }
    private IBlockIdentifier _blockIdentifier;

    private (int AppId, string AppNameId) GetInstanceAppIdAndName(int zoneId)
    {
        var l = Log.Fn<(int, string)>($"{zoneId}");
        var module = UnwrappedModule ?? throw new("instance is not ModuleInfo");
        var msg = $"get appid from instance for Z:{zoneId} Mod:{module.ModuleID}";
        if (IsContent)
        {
            var appId = _appsCatalog.DefaultAppIdentity(zoneId).AppId;
            return l.Return((appId, "Content"), $"{msg} - use Default app: {appId}");
        }

        if (module.ModuleSettings.ContainsKey(ModuleSettingNames.AppName))
        {
            var guid = module.ModuleSettings[ModuleSettingNames.AppName].ToString();
            var appId = _appFinderLazy.Value.FindAppId(zoneId, guid);
            return l.Return((appId, guid), $"{msg} AppG:{guid} = app:{appId}");
        }

        Log.A($"{msg} not found = null");
        return l.Return((Eav.Constants.AppIdEmpty, Eav.Constants.AppNameIdEmpty), "not found");
    }
}