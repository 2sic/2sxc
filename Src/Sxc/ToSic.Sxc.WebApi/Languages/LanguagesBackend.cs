using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;

namespace ToSic.Sxc.WebApi.Languages
{
    public class LanguagesBackend: HasLog<LanguagesBackend>
    {
        #region Constructor & DI
        
        public LanguagesBackend(IZoneMapper zoneMapper, ZoneManager zoneManager, ISite site) : base("Bck.Admin")
        {
            _zoneManager = zoneManager;
            _site = site;
            _zoneMapper = zoneMapper.Init(Log);
        }
        private readonly IZoneMapper _zoneMapper;
        private readonly ZoneManager _zoneManager;
        private readonly ISite _site;

        #endregion

        public IList<SiteLanguageDto> GetLanguages(/*int siteId*/)
        {
            var callLog = Log.Call();
            //var zoneId = _zoneMapper.GetZoneId(siteId);
            // ReSharper disable once PossibleInvalidOperationException
            var cultures = _zoneMapper.CulturesWithState(_site.Id, _site.ZoneId)// siteId, zoneId)
                .Select(c => new SiteLanguageDto { Code = c.Key, Culture = c.Text, IsEnabled = c.Active })
                .ToList();

            callLog("found:" + cultures.Count);
            return cultures;
        }

        public void Toggle(/*int siteId, */string cultureCode, bool enable, string niceName)
        {
            Log.Add($"switch language:{cultureCode}, to:{enable}");
            // Activate or Deactivate the Culture
            //var zoneMapper = _zoneMapper.Init(Log);
            //var zoneId = zoneMapper.GetZoneId(siteId);
            _zoneManager.Init(_site.ZoneId /*zoneId*/, Log).SaveLanguage(cultureCode, niceName, enable);
        }
    }
}
