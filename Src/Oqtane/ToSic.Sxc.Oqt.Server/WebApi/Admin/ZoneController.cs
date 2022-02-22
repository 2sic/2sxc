using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Languages;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Eav.WebApi.Zone;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    // Beta routes - TODO: @STV - why is this beta?
    [Route(WebApiConstants.WebApiStateRoot + $"/{AreaRoutes.Admin}")]
    public class ZoneController : OqtStatefulControllerBase, IZoneController
    {
        protected override string HistoryLogName => "Api.Zone";

        public ZoneController(LanguagesBackend languagesBackend, Lazy<ZoneBackend> zoneBackendLazy)
        {
            _languagesBackend = languagesBackend;
            _zoneBackendLazy = zoneBackendLazy;
        }
        private readonly LanguagesBackend _languagesBackend;
        private readonly Lazy<ZoneBackend> _zoneBackendLazy;

        /// <inheritdoc />
        [HttpGet]
        public IList<SiteLanguageDto> GetLanguages() => _languagesBackend.Init(Log).GetLanguages();

        /// <inheritdoc />
        [HttpGet]
        public void SwitchLanguage(string cultureCode, bool enable) =>
            _languagesBackend.Init(Log)
                .Toggle(cultureCode, enable, CultureInfo.GetCultureInfo(cultureCode).EnglishName);

        /// <inheritdoc />
        [HttpGet]
        public SystemInfoSetDto GetSystemInfo() => _zoneBackendLazy.Value.Init(Log).GetSystemInfo();

    }
}