using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Zone;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
	[SupportedModules("2sxc,2sxc-app")]
    [DnnLogExceptions]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class ZoneController : SxcApiControllerBase<ZoneControllerReal>, IZoneController
    {
        public ZoneController() : base(ZoneControllerReal.LogSuffix) { }

        /// <inheritdoc />
        [HttpGet]
        public IList<SiteLanguageDto> GetLanguages() => Real.GetLanguages();

        /// <inheritdoc />
        [HttpGet]
        public void SwitchLanguage(string cultureCode, bool enable) => Real.SwitchLanguage(cultureCode, enable);

        /// <inheritdoc />
        [HttpGet]
        public SystemInfoSetDto GetSystemInfo() => Real.GetSystemInfo();
    }
}