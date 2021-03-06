﻿using System;
using System.Collections.Generic;
using Oqtane.Controllers;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtModule: Module<Module>
    {
        private readonly SettingsHelper _settingsHelper;
        private readonly Lazy<OqtZoneMapper> _zoneMapperLazy;
        private readonly IModuleRepository _moduleRepository;
        private IZoneMapper ZoneMapper => _zoneMapper ??= _zoneMapperLazy.Value.Init(Log);
        private IZoneMapper _zoneMapper;
        private Dictionary<string, string> _settings;

        public OqtModule(SettingsHelper settingsHelper, Lazy<OqtZoneMapper> zoneMapperLazy, IModuleRepository moduleRepository) : base ($"{OqtConstants.OqtLogPrefix}.Cont")
        {
            _settingsHelper = settingsHelper;
            _zoneMapperLazy = zoneMapperLazy;
            _moduleRepository = moduleRepository;
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
            _isPrimary = module.ModuleDefinitionName.Contains(".Content");
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
        public override bool IsPrimary => _isPrimary;
        private bool _isPrimary = true;

        public override IBlockIdentifier BlockIdentifier
        {
            get
            {
                if (_blockIdentifier != null) return _blockIdentifier;

                // find ZoneId, AppId and prepare settings for next values
                var zoneId = ZoneMapper.GetZoneId(UnwrappedContents.SiteId);
                var appId = GetInstanceAppId(zoneId); //appId ?? TestIds.Blog.App;
                var block = Guid.Empty;
                if (_settings.ContainsKey(Settings.ModuleSettingContentGroup))
                    Guid.TryParse(_settings[Settings.ModuleSettingContentGroup], out block);

                // Check if we have preview-view identifier - for blocks which don't exist yet
                var overrideView = new Guid();
                if (_settings.TryGetValue(Settings.ModuleSettingsPreview, out var previewId) && !string.IsNullOrEmpty(previewId))
                    Guid.TryParse(previewId, out overrideView);

                _blockIdentifier = new BlockIdentifier(zoneId, appId, block, overrideView);

                return _blockIdentifier;
            }
        }

        private IBlockIdentifier _blockIdentifier;


        private int GetInstanceAppId(int zoneId)
        {
            var zoneRt = new ZoneRuntime().Init(zoneId, Log);
            if (IsPrimary) return zoneRt.DefaultAppId;

            if (!_settings.ContainsKey(Settings.ModuleSettingApp)) return Eav.Constants.AppIdEmpty;

            var guid = _settings[Settings.ModuleSettingApp] ?? "";
            var appId = zoneRt.FindAppId(guid);
            return appId;

        }
    }

}
