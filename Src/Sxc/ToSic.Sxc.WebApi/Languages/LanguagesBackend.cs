using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.WebApi.Languages
{
    public class LanguagesBackend: HasLog<LanguagesBackend>
    {
        private readonly ZoneManager _zoneManager;

        #region Constructor & DI
        
        public LanguagesBackend(IZoneMapper zoneMapper, ZoneManager zoneManager) : base("Bck.Admin")
        {
            _zoneManager = zoneManager;
            _zoneMapper = zoneMapper.Init(Log);
        }

        private readonly IZoneMapper _zoneMapper;

        #endregion

        public IList<SiteLanguageDto> GetLanguages(int siteId)
        {
            var callLog = Log.Call();
            var zoneId = _zoneMapper.GetZoneId(siteId);
            // ReSharper disable once PossibleInvalidOperationException
            var cultures = _zoneMapper.CulturesWithState(siteId, zoneId)
                .Select(c => new SiteLanguageDto { Code = c.Key, Culture = c.Text, IsEnabled = c.Active })
                .ToList();

            callLog("found:" + cultures.Count);
            return cultures;
        }

        public void Toggle(int siteId, string cultureCode, bool enable, string niceName)
        {
            Log.Add($"switch language:{cultureCode}, to:{enable}");
            // Activate or Deactivate the Culture
            var zoneMapper = _zoneMapper.Init(Log);
            var zoneId = zoneMapper.GetZoneId(siteId);
            _zoneManager.Init(zoneId, Log).SaveLanguage(cultureCode, niceName, enable);
        }
    }
}
