using ToSic.Sys.Users;

namespace ToSic.Sxc.Backend.ImportExport;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ImpExpHelpers(IAppReaderFactory appReadFac) : ServiceBase("Sxc.ImExHl", connect: [appReadFac])
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
            throw HttpException.PermissionDenied("Tried to access app from another zone. Requires SuperUser permissions.");
        }

        var app = appReadFac.Get(new AppIdentity(zoneId, appId));
        return l.Return(app);
    }

}