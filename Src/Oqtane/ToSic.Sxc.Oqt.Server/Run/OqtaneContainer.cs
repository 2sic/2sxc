using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Services;
using Oqtane.Shared;
using Oqtane.UI;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Web;

// TODO: #Oqtane - still very random - uses lots of TestIds

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtaneContainer: HasLog, IContainer
    {
        private readonly IModuleDefinitionRepository _moduleDefinitionRepository;
        private readonly IModuleRepository _moduleRepository;
        private readonly SettingsHelper _settingsHelper;
        private readonly OqtaneZoneMapper _zoneMapper;
        private Dictionary<string, string> _settings;

        public OqtaneContainer(IModuleDefinitionRepository moduleDefinitionRepository,
            IModuleRepository moduleRepository, SettingsHelper settingsHelper, OqtaneZoneMapper zoneMapper) : base ("Oqt.Cont")
        {
            _moduleDefinitionRepository = moduleDefinitionRepository;
            _moduleRepository = moduleRepository;
            _settingsHelper = settingsHelper;
            _zoneMapper = zoneMapper;
        }

        public OqtaneContainer Init(int? tenantId = null,
            int? id = null, int? appId = null, Guid? block = null)
        {
            var wrapLog = Log.Call($"t:{tenantId}, id:{id}, appId:{appId}, block:{block}");
            
            // Need module instance to get siteId.
            var module = _moduleRepository.GetModule(id ?? -1);
            // Need module definition to get module name to check is PrimaryApp.
            var moduleDefinitions = _moduleDefinitionRepository.GetModuleDefinitions(module.SiteId).ToList();
            _moduleDefinition = moduleDefinitions.FirstOrDefault(item => item.ModuleDefinitionName == module.ModuleDefinitionName);

            _settings = _settingsHelper.Init(EntityNames.Module, id).Settings;

            TenantId = tenantId ?? TestIds.Blog.Zone;
            // find ZoneId, AppId and prepare settings for next values
            var zoneId = _zoneMapper.Init(Log).GetZoneId(module.SiteId);
            Id = id ?? TestIds.Blog.Container;
            AppId = GetInstanceAppId(TenantId); //appId ?? TestIds.Blog.App;

            Block = block ?? TestIds.Blog.Block;
            // find block identifier
            // TODO: what is missing
            Guid.TryParse(_settings[Settings.ModuleSettingContentGroup]?.ToString(), out Block);

            wrapLog("ok");

            return this;
        }

        private ModuleDefinition _moduleDefinition;

        // Temp implementation, don't support im MVC
        public IContainer Init(int id, ILog parentLog) => throw new System.NotImplementedException();

        /// <inheritdoc />
        public int Id { get; set; }

        /// <inheritdoc />
        public int TenantId { get; set; }

        /// <inheritdoc />
        public bool IsPrimary => _moduleDefinition.Name == "Content";

        public List<KeyValuePair<string, string>> Parameters
        {
            get => _parameters ??= Eav.Factory.Resolve<IHttp>().QueryStringKeyValuePairs();
            set => _parameters = value;
        }
        private List<KeyValuePair<string, string>> _parameters;

        public IBlockIdentifier BlockIdentifier 
            => _blockIdentifier ??= new BlockIdentifier(TenantId, AppId, Block, Guid.Empty);

        private IBlockIdentifier _blockIdentifier;

        // special while testing
        public int AppId;

        public Guid Block;

        private int GetInstanceAppId(int zoneId)
        {
            var zoneRt = new ZoneRuntime(zoneId, Log);
            if (IsPrimary)
            {
                var appId = zoneRt.DefaultAppId;
                return appId;
            }

            if (_settings.ContainsKey(Settings.ModuleSettingApp))
            {
                var guid = _settings[Settings.ModuleSettingApp]?.ToString() ?? "";
                var appId = zoneRt.FindAppId(guid);
                return appId;
            }

            return Eav.Constants.AppIdEmpty;
        }
    }

}
