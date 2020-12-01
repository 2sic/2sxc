using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.WebApi.Context;

namespace ToSic.Sxc.WebApi.Admin
{
    public class AdminBackend: WebApiBackendBase<AdminBackend>
    {
        public AdminBackend(IServiceProvider serviceProvider) : base(serviceProvider, "Bck.Admin")
        {
        }

        public dynamic DialogSettings(IContextOfSite context, ContextBuilderBase contextBuilder, int appId)
        {
            IApp app = null;
            // if we have an appid (we don't have it in an install-new-apps-scenario) check permissions
            if (appId != Eav.Constants.AppIdEmpty)
            {
                app = GetApp(appId, false);
                var appAndPerms = ServiceProvider.Build<MultiPermissionsApp>().Init(context, app, Log);
                if (!appAndPerms.ZoneIsOfCurrentContextOrUserIsSuper(out var error))
                    throw HttpException.PermissionDenied(error);
            }

            var cb = contextBuilder.InitApp(app?.ZoneId ?? context.Site.ZoneId, app);

            return new
            {
                Context = cb.Get(Ctx.All),
            };
        }

    }
}
