using Microsoft.AspNetCore.Mvc;

namespace ToSic.Sxc.Oqt.Server.Controllers;

internal static class OqtApiServices
{
    internal static T GetApiService<T>(this ControllerBase controller) where T : class
        => controller.HttpContext.RequestServices.Build<T>((controller as IHasLog)?.Log);
}
