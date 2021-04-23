using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Languages;

namespace ToSic.Sxc.Oqt.Server.Controllers.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Oqtane.Shared.RoleNames.Admin)]
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

        public ZoneController(StatefulControllerDependencies dependencies, LanguagesBackend languagesBackend) : base(dependencies)
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