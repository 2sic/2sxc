using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Globalization;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Languages;
using ToSic.Sxc.WebApi.PublicApi;
using ToSic.Sxc.WebApi.Zone;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/admin/[controller]/[action]")]
    public class ZoneController : OqtStatefulControllerBase, IZoneController
    {
        protected override string HistoryLogName => "Api.Zone";

        public ZoneController(LanguagesBackend languagesBackend, Lazy<SiteStateInitializer> siteStateInitializerLazy, Lazy<ZoneBackend> zoneBackendLazy)
        {
            _languagesBackend = languagesBackend;
            _siteStateInitializerLazy = siteStateInitializerLazy;
            _zoneBackendLazy = zoneBackendLazy;
        }
        private readonly LanguagesBackend _languagesBackend;
        private readonly Lazy<SiteStateInitializer> _siteStateInitializerLazy;
        private readonly Lazy<ZoneBackend> _zoneBackendLazy;

        /// <inheritdoc />
        [HttpGet]
        public IList<SiteLanguageDto> GetLanguages() =>
            _languagesBackend.Init(Log)
                .GetLanguages(_siteStateInitializerLazy.Value.InitializedState.Alias.SiteId);

        /// <inheritdoc />
        [HttpGet]
        public void SwitchLanguage(string cultureCode, bool enable) =>
            _languagesBackend.Init(Log)
                .Toggle(
                    _siteStateInitializerLazy.Value.InitializedState.Alias.SiteId,
                    cultureCode,
                    enable,
                    CultureInfo.GetCultureInfo(cultureCode).EnglishName);

        /// <inheritdoc />
        [HttpGet]
        public SystemInfoSetDto GetSystemInfo() => _zoneBackendLazy.Value.Init(Log)
            .GetSystemInfo(_siteStateInitializerLazy.Value.InitializedState.Alias.SiteId);

    }
}