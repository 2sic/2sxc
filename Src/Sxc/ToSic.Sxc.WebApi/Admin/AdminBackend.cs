using System;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Errors;
using ToSic.Eav.WebApi.Security;
using ToSic.Sxc.Context;
using ToSic.Sxc.WebApi.Context;

namespace ToSic.Sxc.WebApi.Admin
{
    public class AdminBackend: WebApiBackendBase<AdminBackend>
    {
        private readonly IContextResolver _ctxResolver;

        public AdminBackend(IServiceProvider serviceProvider, IContextResolver ctxResolver) : base(serviceProvider, "Bck.Admin")
        {
            _ctxResolver = ctxResolver;
        }

        public DialogContextStandalone DialogSettings(int appId, IJsContextBuilder jsContextBuilder)
        {            
            // reset app-id if we get a info-token like -100
            if (appId < 0) appId = Eav.Constants.AppIdEmpty;

            var appContext = appId != Eav.Constants.AppIdEmpty ? _ctxResolver.App(appId) : null;
            var context = appContext ?? _ctxResolver.Site();
            // if we have an appid (we don't have it in an install-new-apps-scenario) check permissions
            if (appContext != null)
            {
                var appAndPerms = ServiceProvider.Build<MultiPermissionsApp>().Init(appContext, appContext.AppState, Log);
                if (!appAndPerms.ZoneIsOfCurrentContextOrUserIsSuper(out var error))
                    throw HttpException.PermissionDenied(error);
            }

            var app = appId != Eav.Constants.AppIdEmpty ? GetApp(appId, false) : null;
            var cb = jsContextBuilder.InitApp(context.Site.ZoneId, app);

            return new DialogContextStandalone
            {
                Context = cb.Get(Ctx.All),
            };
        }

    }

    public class DialogContextStandalone
    {
        public ContextDto Context { get; set; }
    }

}
