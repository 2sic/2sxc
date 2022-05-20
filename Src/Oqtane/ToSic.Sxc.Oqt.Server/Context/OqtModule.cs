using System;
using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Context
{
    public class OqtModule: Module<Module>
    {
        private readonly SettingsHelper _settingsHelper;
        private readonly IModuleRepository _moduleRepository;
        private readonly IAppStates _appStates;
        private readonly Lazy<AppFinder> _appFinderLazy;
        private readonly ISite _site;
        private Dictionary<string, string> _settings;

        public OqtModule(SettingsHelper settingsHelper, IModuleRepository moduleRepository, 
            IAppStates appStates, Lazy<AppFinder> appFinderLazy, ISite site) : base ($"{OqtConstants.OqtLogPrefix}.Cont")
        {
            _settingsHelper = settingsHelper;
            _moduleRepository = moduleRepository;
            _appStates = appStates;
            _appFinderLazy = appFinderLazy;
            _site = site;
        }

        public new OqtModule Init(Module module, ILog parentLog)
        {
            var wrapLog = Log.Call($"id:{module.ModuleId}");
            base.Init(module, parentLog);

            InitializeIsPrimary(module);

            _settings = _settingsHelper.Init(EntityNames.Module, module.ModuleId).Settings;

            _id = module.ModuleId;

            wrapLog("ok");

            return this;
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
        public override IModule Init(int id, ILog parentLog)
        {
            var module = _moduleRepository.GetModule(id);
            return Init(module, parentLog);
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
                if (_blockIdentifier != null) return _blockIdentifier;

                // find ZoneId, AppId and prepare settings for next values
                var zoneId = _site.ZoneId; // ZoneMapper.GetZoneId(UnwrappedContents.SiteId);
                var (appId, appNameId) = GetInstanceAppId(zoneId); //appId ?? TestIds.Blog.App;
                var block = Guid.Empty;
                if (_settings.ContainsKey(Settings.ModuleSettingContentGroup))
                    Guid.TryParse(_settings[Settings.ModuleSettingContentGroup], out block);

                // Check if we have preview-view identifier - for blocks which don't exist yet
                var overrideView = new Guid();
                if (_settings.TryGetValue(Settings.ModuleSettingsPreview, out var previewId) && !string.IsNullOrEmpty(previewId))
                    Guid.TryParse(previewId, out overrideView);

                _blockIdentifier = new BlockIdentifier(zoneId, appId, appNameId, block, overrideView);

                return _blockIdentifier;
            }
        }

        private IBlockIdentifier _blockIdentifier;


        private (int AppId, string AppNameId) GetInstanceAppId(int zoneId)
        {
            var wrapLog = Log.Fn<(int, string)>($"{zoneId}");

            if (IsContent) 
                return wrapLog.Return((_appStates.DefaultAppId(zoneId), "Content"), "Content");

            if (!_settings.ContainsKey(Settings.ModuleSettingApp)) 
                return wrapLog.Return((Eav.Constants.AppIdEmpty, Eav.Constants.AppNameIdEmpty), Eav.Constants.AppNameIdEmpty);

            var guid = _settings[Settings.ModuleSettingApp] ?? "";
            var appId = _appFinderLazy.Value.Init(Log).FindAppId(zoneId, guid);
            return wrapLog.Return((appId, guid), "ok");

        }
    }

}
