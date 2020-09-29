using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Languages;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [DnnLogExceptions]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class ZoneController : SxcApiControllerBase
    {
        protected override string HistoryLogName => "Api.Zone";

        [HttpGet]
        public IList<TenantLanguageDto> GetLanguages() =>
            Eav.Factory.Resolve<LanguagesBackend>().Init(Log)
                .GetLanguages(PortalSettings.PortalId);

        /// <summary>
        /// Enable / disable a language in the EAV
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public void SwitchLanguage(string cultureCode, bool enable) =>
            Eav.Factory.Resolve<LanguagesBackend>().Init(Log)
                .Toggle(
                    PortalSettings.PortalId,
                    cultureCode,
                    enable,
                    LocaleController.Instance.GetLocale(cultureCode).Text);

    }
}