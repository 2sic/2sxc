using System.Linq;
using System.Web.Http;
using DotNetNuke.Services.Localization;
using ToSic.Eav.Apps;
using ToSic.Eav.Run;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class SystemController
    {
        [HttpGet]
        // todo: deprecate PARAMS find out if / where used
        public dynamic GetLanguages()
        {
            Log.Add("get languages");
            var portalId = PortalSettings.PortalId;
            var zoneMapper = Eav.Factory.Resolve<IZoneMapper>().Init(Log);
            var zoneId = zoneMapper.GetZoneId(portalId);
            // ReSharper disable once PossibleInvalidOperationException
            var cultures = zoneMapper.CulturesWithState(portalId, zoneId)
                .Select(c => new
                {
                    Code = c.Key,
                    Culture = c.Text,
                    IsEnabled = c.Active
                });

            Log.Add("languages - found:" + cultures.Count());
            return cultures;
        }

        /// <summary>
        /// Enable / disable a language in the EAV
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public void SwitchLanguage(string cultureCode, bool enable)
        {
            Log.Add($"switch language:{cultureCode}, to:{enable}");
            // Activate or Deactivate the Culture
            var zoneMapper = Eav.Factory.Resolve<IZoneMapper>().Init(Log);
            var zoneId = zoneMapper.GetZoneId(PortalSettings.PortalId);

            var cultureText = LocaleController.Instance.GetLocale(cultureCode).Text;
            new ZoneManager(zoneId, Log).SaveLanguage(cultureCode, cultureText, enable);
        }

    }
}
