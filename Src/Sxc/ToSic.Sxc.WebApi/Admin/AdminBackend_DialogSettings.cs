using ToSic.Eav.Apps.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.WebApi.Context;
using ToSic.Sxc.WebApi.Security;

namespace ToSic.Sxc.WebApi.Admin
{
    public partial class AdminBackend
    {

        public dynamic DialogSettings(IInstanceContext context, ContextBuilderBase contextBuilder, int appId)
        {
            IApp app = null;
            // if we have an appid (we don't have it in an install-new-apps-scenario) check permissions
            if (appId != 0 && appId != Eav.Constants.AppIdEmpty)
            {
                var appAndPerms = new MultiPermissionsApp().Init(context, GetApp(appId, null), Log);
                if (!appAndPerms.ZoneIsOfCurrentContextOrUserIsSuper(out var error))
                    throw HttpException.PermissionDenied(error);
                app = appAndPerms.App;
            }

            var cb = contextBuilder.InitApp(app?.ZoneId, app);

            return new
            {
                // TODO: Deprecate PARAMS these properties as soon as old UI is gone
                //IsContent = app?.AppGuid == "Default",
                //Language = psCurrent.CultureCode,
                //LanguageDefault = psCurrent.DefaultLanguage,
                //AppPath = app?.Path,
                //GettingStartedUrl = cb.GettingStartedUrl(),
                // END TODO
                Context = cb.Get(Ctx.All),
            };
        }
    }
}
