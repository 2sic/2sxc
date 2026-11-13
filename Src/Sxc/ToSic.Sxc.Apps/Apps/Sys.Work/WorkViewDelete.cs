using ToSic.Eav.WebApi.Sys.Security;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Apps.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkViewDelete(AppWorkChain<WorkViewsGet> appViews, AppWorkChain<WorkEntityDelete> entityDelete, IUser user)
    : ServiceWithSetup<IAppWorkContext>("AWk.EntCre", connect: [appViews, entityDelete, user])
{

    public bool DeleteViewWithSiteSwitchCheck(int viewId)
    {
        var l = Log.Fn<bool>($"delete a{MyOptions.AppId}, t:{viewId}");

        // extra security to only allow zone change if host user
        SecurityHelpers.ThrowIfNotSiteAdmin(user, Log);

        // really get template first, to be sure it is a template
        var template = appViews.New(MyOptions).Get(viewId);
        var result = entityDelete.New(MyOptions).Delete(template.Id);

        return l.Return(result);
    }
}