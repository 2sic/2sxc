using System;
using System.Collections.Generic;
using Oqtane.Models;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtaneContainer: Container<Module>
    {
        private readonly SettingsHelper _settingsHelper;
        private readonly OqtaneZoneMapper _zoneMapper;
        private Dictionary<string, string> _settings;

        public OqtaneContainer(SettingsHelper settingsHelper, OqtaneZoneMapper zoneMapper) : base ("Oqt.Cont")
        {
            _settingsHelper = settingsHelper;
            _zoneMapper = zoneMapper;
        }

        public new OqtaneContainer Init(Module module, ILog parentLog)
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
        public override IContainer Init(int id, ILog parentLog) => throw new NotImplementedException();

        /// <inheritdoc />
        public override int Id => _id;
        private int _id;

        /// <inheritdoc />
        public override bool IsPrimary => _isPrimary;
        private bool _isPrimary = true;

        public List<KeyValuePair<string, string>> Parameters
        {
            get => _parameters ??= Eav.Factory.Resolve<IHttp>().QueryStringKeyValuePairs();
            set => _parameters = value;
        }
        private List<KeyValuePair<string, string>> _parameters;

        public override IBlockIdentifier BlockIdentifier
        {
            get
            {
                if (_blockIdentifier != null) return _blockIdentifier;

                // find ZoneId, AppId and prepare settings for next values
                var zoneId = _zoneMapper.Init(Log).GetZoneId(UnwrappedContents.SiteId);
                var appId = GetInstanceAppId(zoneId); //appId ?? TestIds.Blog.App;
                var block = Guid.Empty;
                if (_settings.ContainsKey(Settings.ModuleSettingContentGroup))
                    Guid.TryParse(_settings[Settings.ModuleSettingContentGroup], out block);

                _blockIdentifier = new BlockIdentifier(zoneId, appId, block, Guid.Empty);

                return _blockIdentifier;
            }
        }

        private IBlockIdentifier _blockIdentifier;


        private int GetInstanceAppId(int zoneId)
        {
            var zoneRt = new ZoneRuntime(zoneId, Log);
            if (IsPrimary) return zoneRt.DefaultAppId;

            if (!_settings.ContainsKey(Settings.ModuleSettingApp)) return Eav.Constants.AppIdEmpty;

            var guid = _settings[Settings.ModuleSettingApp] ?? "";
            var appId = zoneRt.FindAppId(guid);
            return appId;

        }
    }

}
