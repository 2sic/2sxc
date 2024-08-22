using ToSic.Eav.Apps.State;

namespace ToSic.Sxc.Backend.ImportExport;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ImpExpHelpers(IAppReaders appStates) : ServiceBase("Sxc.ImExHl", connect: [appStates])
{
    /// <summary>
    /// Get an app - but only allow zone change if super-user
    /// </summary>
    /// <returns></returns>
    internal IAppReader GetAppAndCheckZoneSwitchPermissions(int zoneId, int appId, IUser user, int contextZoneId)
    {
        var l = Log.Fn<IAppReader>($"superuser: {user.IsSystemAdmin}");
        if (!user.IsSystemAdmin && zoneId != contextZoneId)
        {
            l.ReturnNull("error");
            throw Eav.WebApi.Errors.HttpException.PermissionDenied("Tried to access app from another zone. Requires SuperUser permissions.");
        }

        var app = appStates.Get(new AppIdentity(zoneId, appId));
        return l.Return(app);
    }

}