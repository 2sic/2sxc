using ToSic.Eav.Apps.State;

namespace ToSic.Sxc.Backend.ImportExport;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ImpExpHelpers: ServiceBase
{

    #region Constructor / DI

    public ImpExpHelpers(IAppStates appStates) : base("Sxc.ImExHl")
    {
        ConnectServices(
            _appStates = appStates
        );
    }

    private readonly IAppStates _appStates;


    #endregion

    /// <summary>
    /// Get an app - but only allow zone change if super-user
    /// </summary>
    /// <returns></returns>
    internal IAppStateInternal GetAppAndCheckZoneSwitchPermissions(int zoneId, int appId, IUser user, int contextZoneId)
    {
        var l = Log.Fn<IAppStateInternal>($"superuser: {user.IsSystemAdmin}");
        if (!user.IsSystemAdmin && zoneId != contextZoneId)
        {
            l.ReturnNull("error");
            throw Eav.WebApi.Errors.HttpException.PermissionDenied("Tried to access app from another zone. Requires SuperUser permissions.");
        }

        var app = _appStates.GetReader(new AppIdentity(zoneId, appId));
        return l.Return(app);
    }

}