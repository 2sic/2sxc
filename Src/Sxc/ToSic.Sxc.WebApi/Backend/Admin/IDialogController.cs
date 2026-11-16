using ToSic.Eav.WebApi.Admin.Sys;

namespace ToSic.Sxc.Backend.Admin;

public interface IDialogController: IAppAdminWebApiController
{
    /// <summary>
    /// This is the subsystem which delivers the getting-started app-iframe with instructions etc.
    /// Used to be GET System/DialogSettings
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    DialogContextStandaloneDto Settings(int appId);
}