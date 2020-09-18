using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Services.Localization;
using ToSic.Eav;
using ToSic.Sxc.WebApi.Languages;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class SystemController
    {

        [HttpGet]
        public IList<TenantLanguageDto> GetLanguages() => 
            Factory.Resolve<LanguagesBackend>().Init(Log)
                .GetLanguages(PortalSettings.PortalId);

        /// <summary>
        /// Enable / disable a language in the EAV
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public void SwitchLanguage(string cultureCode, bool enable) =>
            Factory.Resolve<LanguagesBackend>().Init(Log)
                .Toggle(
                    PortalSettings.PortalId,
                    cultureCode,
                    enable,
                    LocaleController.Instance.GetLocale(cultureCode).Text);
    }
}
