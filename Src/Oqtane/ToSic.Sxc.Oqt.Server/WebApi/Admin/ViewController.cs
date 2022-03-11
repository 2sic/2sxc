using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Context;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Plumbing;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Adam;
using ToSic.Sxc.WebApi.Admin;
using ToSic.Sxc.WebApi.Views;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    [AllowAnonymous] // necessary at this level, because otherwise download would fail

    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    public class ViewController : OqtStatefulControllerBase<ViewControllerReal<IActionResult>>, IViewController<IActionResult>
    {
        public ViewController(LazyInitLog<Pages.Pages> pages) : base(ViewControllerReal<IActionResult>.LogSuffix)
        {
            _pages = pages.SetLog(Log);
        }
        private readonly LazyInitLog<Pages.Pages> _pages;

        /// <inheritdoc />
        [HttpGet]
        //[SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<ViewDetailsDto> All(int appId) => Real.All(appId);

        /// <inheritdoc />
        [HttpGet]
        //[SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public PolymorphismDto Polymorphism(int appId) => Real.Polymorphism(appId);

        /// <inheritdoc />
        [HttpGet, HttpDelete]
        //[SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public bool Delete(int appId, int id) => Real.Delete(appId, id);

        /// <inheritdoc />
        [HttpGet]
        [AllowAnonymous] // will do security check internally
        public IActionResult Json(int appId, int viewId)
        {
            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = (OqtResponseMaker)GetService<ResponseMaker<IActionResult>>();
            responseMaker.Init(this);

            return Real.Json(appId, viewId);
        }

        /// <inheritdoc />
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public ImportResultDto Import(int zoneId, int appId) => Real.Import(new HttpUploadedFile(Request), zoneId, appId);

        /// <inheritdoc />
        [HttpGet]
        //[SupportedModules("2sxc,2sxc-app")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<ViewDto> Usage(int appId, Guid guid) => Real.UsagePreparations((views, blocks) =>
        {
            // create list with all 2sxc modules in this site
            var allMods = _pages.Ready.AllModulesWithContent(Real.SiteId);
            Log.Add($"Found {allMods.Count} modules");

            return views.Select(vwb => _pages.Ready.ViewDtoBuilder(vwb, blocks, allMods));
        }).Usage(appId, guid);


    }
}