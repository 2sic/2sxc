using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Controllers;

internal static class OqtApiServices
{
    internal static T GetApiService<T>(this ControllerBase controller) where T : class
        => controller.HttpContext.GetSysRequestServices().Build<T>((controller as IHasLog)?.Log);
}
