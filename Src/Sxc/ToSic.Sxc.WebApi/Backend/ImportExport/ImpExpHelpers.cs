using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Backend.ImportExport;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ImpExpHelpers(
    IAppReaderFactory appReadFac,
    IAppPathsMicroSvc appPathSvc
) : ServiceBase("Sxc.ImExHl", connect: [appReadFac, appPathSvc])
{
    /// <summary>
    /// Get an app - but only allow zone change if super-user
    /// </summary>
    /// <returns></returns>
    internal IAppReader GetAppAndCheckZoneSwitchPermissions(IAppIdentity appIdentity, IUser user, int contextZoneId)
    {
        var l = Log.Fn<IAppReader>($"superuser: {user.IsSystemAdmin}; appIdentity: {appIdentity.Show()}");
        if (!user.IsSystemAdmin && appIdentity.ZoneId != contextZoneId)
        {
            l.ReturnNull("error");
            throw HttpException.PermissionDenied(
                "Tried to access app from another zone. Requires SuperUser permissions.");
        }

        var app = appReadFac.Get(appIdentity);
        return l.Return(app);
    }
}