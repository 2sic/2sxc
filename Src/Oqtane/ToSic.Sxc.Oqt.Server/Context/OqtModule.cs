using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys.Module;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Context;

internal class OqtModule(
    SettingsHelper settingsHelper,
    IModuleRepository moduleRepository,
    IAppsCatalog appsCatalog,
    LazySvc<AppFinder> appFinderLazy,
    ISite site)
    : Module<Module>($"{OqtConstants.OqtLogPrefix}.Cont",
        connect: [settingsHelper, moduleRepository, appsCatalog, appFinderLazy, site])
{
    private Dictionary<string, string> _settings;

    public new OqtModule Init(Module module)
    {
        base.Init(module);
        var l = Log.Fn<OqtModule>($"id:{module.ModuleId}", timer: true);

        InitializeIsPrimary(module);

        _settings = settingsHelper.Init(EntityNames.Module, module.ModuleId).Settings;

        _id = module.ModuleId;

        return l.ReturnAsOk(this);
    }

    /// <summary>
    /// Need module definition to get module name to check is PrimaryApp.
    /// </summary>
    /// <param name="module"></param>
    private void InitializeIsPrimary(Module module)
    {
        if (module == null) return;
        // note that it's "ToSic.Sxc.Oqt.App, ToSic.Sxc.Oqtane.Client" or "ToSic.Sxc.Oqt.Content, ToSic.Sxc.Oqtane.Client"
        _isContent = module.ModuleDefinitionName.Contains(".Content");
    }

    // Temp implementation, don't support im MVC
    public override IModule Init(int id)
    {
        var module = moduleRepository.GetModule(id);
        return Init(module);
    }

    /// <inheritdoc />
    public override int Id => _id;
    private int _id;

    /// <inheritdoc />
    public override bool IsContent => _isContent;
    private bool _isContent = true;

    public override IBlockIdentifier BlockIdentifier
    {
        get
        {
            if (_blockIdentifier != null)
                return _blockIdentifier;

            // find ZoneId, AppId and prepare settings for next values
            var zoneId = site.ZoneId; // ZoneMapper.GetZoneId(UnwrappedContents.SiteId);
            var (appId, appNameId) = GetInstanceAppId(zoneId); //appId ?? TestIds.Blog.App;
            var block = Guid.Empty;
            if (_settings.ContainsKey(ModuleSettingNames.ContentGroup))
                Guid.TryParse(_settings[ModuleSettingNames.ContentGroup], out block);

            // Check if we have preview-view identifier - for blocks which don't exist yet
            var overrideView = new Guid();
            if (_settings.TryGetValue(ModuleSettingNames.PreviewView, out var previewId) && !string.IsNullOrEmpty(previewId))
                Guid.TryParse(previewId, out overrideView);

            _blockIdentifier = new BlockIdentifier(zoneId, appId, appNameId, block, overrideView);

            return _blockIdentifier;
        }
    }

    private IBlockIdentifier _blockIdentifier;


    private (int AppId, string AppNameId) GetInstanceAppId(int zoneId)
    {
        var l = Log.Fn<(int, string)>($"{zoneId}", timer: true);

        if (IsContent) 
            return l.Return((appsCatalog.DefaultAppIdentity(zoneId).AppId, "Content"), "Content");

        if (!_settings.TryGetValue(ModuleSettingNames.AppName, out var setting)) 
            return l.Return((KnownAppsConstants.AppIdEmpty, KnownAppsConstants.AppNameIdEmpty), KnownAppsConstants.AppNameIdEmpty);

        var guid = setting ?? "";
        var appId = appFinderLazy.Value.FindAppId(zoneId, guid);
        return l.ReturnAsOk((appId, guid));

    }
}