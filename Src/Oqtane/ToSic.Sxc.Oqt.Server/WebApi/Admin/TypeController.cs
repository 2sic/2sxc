using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.WebApi.Admin.Sys;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;
using RealController = ToSic.Eav.WebApi.Admin.Sys.TypeControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

/// <summary>
/// This one supplies portal-wide (or cross-portal) settings / configuration
/// </summary>
//[SupportedModules("2sxc,2sxc-app")]
//[DnnLogExceptions]
[Authorize(Roles = RoleNames.Admin)]
[AutoValidateAntiforgeryToken]

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]


[OqtLogWebApi]
[OqtInitializeAlias]
[NoContentForEmptyAction]
[SystemTextJsonFormatter]
[ServiceFilter(typeof(OptionalBodyFilter))]
[ServiceFilter(typeof(HttpResponseExceptionFilter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class TypeController : ControllerBase, IHasLog, ITypeController
{
    /// <inheritdoc />
    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}", null, GetType().Name);

    private RealController Real => this.GetApiService<RealController>();
    
    [HttpDelete]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Delete(int appId, string staticName) => Real.Delete(appId, staticName);


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public bool Save(int appId, [FromBody] Dictionary<string, object> item) => Real.Save(appId, item);


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Host)]
    public bool AddGhost(int appId, string sourceNameId) => Real.AddGhost(appId, sourceNameId);


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public void SetTitle(int appId, int contentTypeId, int attributeId) => Real.SetTitle(appId, contentTypeId, attributeId);


    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public IActionResult Json(int appId, string name)
        => Real.Json(appId, name).ToHttpResponse();


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]
    public ImportResult Import(int zoneId, int appId) => Real.Import(new(Request), zoneId, appId);


    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public IActionResult JsonBundleExport(int appId, Guid exportConfiguration, int indentation = 0)
        => Real.JsonBundleExport(appId, exportConfiguration, indentation).ToHttpResponse();
}
