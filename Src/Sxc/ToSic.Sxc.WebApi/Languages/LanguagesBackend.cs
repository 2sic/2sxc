using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.WebApi.Languages
{
    public class LanguagesBackend: HasLog<LanguagesBackend>
    {
        #region Constructor & DI
        
        public LanguagesBackend(IZoneMapper zoneMapper) : base("Bck.Admin") 
            => _zoneMapper = zoneMapper.Init(Log);

        private readonly IZoneMapper _zoneMapper;

        #endregion

        public IList<SiteLanguageDto> GetLanguages(int tenantId)
        {
            var callLog = Log.Call();
            var zoneId = _zoneMapper.GetZoneId(tenantId);
            // ReSharper disable once PossibleInvalidOperationException
            var cultures = _zoneMapper.CulturesWithState(tenantId, zoneId)
                .Select(c => new SiteLanguageDto { Code = c.Key, Culture = c.Text, IsEnabled = c.Active })
                .ToList();

            callLog("found:" + cultures.Count);
            return cultures;
        }

        public void Toggle(int tenantId, string cultureCode, bool enable, string niceName)
        {
            Log.Add($"switch language:{cultureCode}, to:{enable}");
            // Activate or Deactivate the Culture
            var zoneMapper = _zoneMapper.Init(Log);
            var zoneId = zoneMapper.GetZoneId(tenantId);
            new ZoneManager(zoneId, Log).SaveLanguage(cultureCode, niceName, enable);
        }
    }
}
