using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Languages;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Zone;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [DnnLogExceptions]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class ZoneController : SxcApiControllerBase, IZoneController
    {
        protected override string HistoryLogName => "Api.Zone";

        private LanguagesBackend LanguagesBackend() => GetService<LanguagesBackend>().Init(Log);

        /// <inheritdoc />
        [HttpGet]
        public IList<SiteLanguageDto> GetLanguages() => LanguagesBackend().GetLanguages();

        /// <inheritdoc />
        [HttpGet]
        public void SwitchLanguage(string cultureCode, bool enable) =>
            LanguagesBackend().Toggle(cultureCode, enable, LocaleController.Instance.GetLocale(cultureCode).Text);

        /// <inheritdoc />
        [HttpGet]
        public SystemInfoSetDto GetSystemInfo() => GetService<ZoneBackend>().Init(Log).GetSystemInfo();
    }
}