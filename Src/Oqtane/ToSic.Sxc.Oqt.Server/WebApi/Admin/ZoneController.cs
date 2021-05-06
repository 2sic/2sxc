using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Languages;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
    //[ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/admin/[controller]/[action]")]
    public class ZoneController : OqtStatefulControllerBase
    {
        private readonly LanguagesBackend _languagesBackend;
        protected override string HistoryLogName => "Api.Zone";

        public ZoneController(StatefulControllerDependencies dependencies, LanguagesBackend languagesBackend) : base()
        {
            _languagesBackend = languagesBackend;
        }

        [HttpGet]
        public IList<SiteLanguageDto> GetLanguages() =>
            _languagesBackend.Init(Log)
                .GetLanguages(GetContext().Site.Id);

        /// <summary>
        /// Enable / disable a language in the EAV
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public void SwitchLanguage(string cultureCode, bool enable) =>
            _languagesBackend.Init(Log)
                .Toggle(
                    GetContext().Site.Id,
                    cultureCode,
                    enable,
                    CultureInfo.GetCultureInfo(cultureCode).EnglishName);
    }
}