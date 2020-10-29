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
    [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
    [Route(WebApiConstants.WebApiStateRoot + "/admin/[controller]/[action]")]
    public class ZoneController : SxcStatefulControllerBase
    {
        protected override string HistoryLogName => "Api.Zone";

        public ZoneController(StatefulControllerDependencies dependencies) : base(dependencies)
        { }

        [HttpGet]
        public IList<SiteLanguageDto> GetLanguages() =>
            Eav.Factory.Resolve<LanguagesBackend>().Init(Log)
                .GetLanguages(GetContext().Tenant.Id);

        /// <summary>
        /// Enable / disable a language in the EAV
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public void SwitchLanguage(string cultureCode, bool enable) =>
            Eav.Factory.Resolve<LanguagesBackend>().Init(Log)
                .Toggle(
                    GetContext().Tenant.Id,
                    cultureCode,
                    enable,
                    CultureInfo.GetCultureInfo(cultureCode).EnglishName);
    }
}