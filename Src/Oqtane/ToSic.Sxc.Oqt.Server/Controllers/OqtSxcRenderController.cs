using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Controllers;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using System;
using System.Net;
using ToSic.Sxc.Oqt.Shared.Helpers;
using ToSic.Sxc.Oqt.Shared.Interfaces;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Controllers;

//[Route("{alias:int}/api/[controller]")]
[Route(ControllerRoutes.ApiRoute)]

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class OqtSxcRenderController(
    IHttpContextAccessor accessor,
    ILogManager logger,
    IOqtSxcRenderService oqtSxcRenderService)
    : ModuleControllerBase(logger, accessor)
{
    private readonly IHttpContextAccessor _accessor = accessor;
    private bool IsSuperUser => User.IsInRole(RoleNames.Host) || User.IsInRole(RoleNames.Admin);


    [HttpGet("{aliasId:int}/{pageId:int}/{moduleId:int}/{culture}/{preRender:bool}/Render")]
    //[Authorize(Policy = PolicyNames.ViewModule)]
    public OqtViewResultsDto Render([FromRoute] int aliasId, [FromRoute] int pageId, [FromRoute] int moduleId, [FromRoute] string culture, [FromRoute] bool preRender, [FromQuery] string originalParameters)
    {
        var @params = new RenderParameters()
        {
            AliasId = aliasId,
            PageId = pageId,
            ModuleId = moduleId,
            Culture = culture,
            PreRender = preRender,
            OriginalParameters = originalParameters
        };

        try
        {
            if (moduleId != AuthEntityId(EntityNames.Module))
                return Forbidden("Unauthorized OqtSxcRenderController Get Attempt {ModuleId}", moduleId);

            return oqtSxcRenderService.Render(@params);
        }
        catch (Exception ex)
        {
            return Error(ex);
        }
    }

    private OqtViewResultsDto Forbidden(string message, params object[] args)
    {
        logger.Log(LogLevel.Error, this, LogFunction.Security, message, args);
        HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        return null;
    }

    private OqtViewResultsDto Error(Exception ex)
    {
        logger.Log(LogLevel.Error, this, LogFunction.Read, ex, $"exception in {nameof(Render)}");
        return new() { ErrorMessage = ErrorHelper.ErrorMessage(ex, IsSuperUser) };
    }
}