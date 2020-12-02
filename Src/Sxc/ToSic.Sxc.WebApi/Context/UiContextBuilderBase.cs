using System;
using System.Linq;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.Apps;
using ToSic.Eav.WebApi.Security;

namespace ToSic.Sxc.WebApi.Context
{
    public abstract class UiContextBuilderBase: IUiContextBuilder
    {
        private readonly Apps.App _app;

        protected UiContextBuilderBase(Apps.App app) => _app = app;

        protected int ZoneId;
        protected IApp App;

        protected IApp AppTemp;

        public virtual IUiContextBuilder SetZoneAndApp(int zoneId, IAppIdentity app)
        {
            App = app != null ? _app.InitNoData(app, null) : null;

            if(zoneId == null) throw new ArgumentNullException(nameof(zoneId));
            ZoneId = (int) zoneId;
            return this;
        }

        public ContextDto Get(Ctx flags)
        {
            var ctx = new ContextDto();
            // logic for activating each part
            // 1. either that switch is on
            // 2. or the null-check: all is on
            // 3. This also means if the switch is off, it's off
            if (flags.HasFlag(Ctx.AppBasic) | flags.HasFlag(Ctx.AppAdvanced))
                ctx.App = GetApp(flags);
            if (flags.HasFlag(Ctx.Enable)) ctx.Enable = GetEnable();
            if (flags.HasFlag(Ctx.Language)) ctx.Language = GetLanguage();
            if (flags.HasFlag(Ctx.Page)) ctx.Page = GetPage();
            if (flags.HasFlag(Ctx.Site)) ctx.Site = GetSite();
            if (flags.HasFlag(Ctx.System)) ctx.System = GetSystem();
            return ctx;
        }

        protected abstract LanguageDto GetLanguage();

        protected abstract WebResourceDto GetSystem();
        protected abstract WebResourceDto GetSite();
        protected abstract WebResourceDto GetPage();

        protected abstract EnableDto GetEnable();
        protected abstract string GetGettingStartedUrl();

        private AppDto GetApp(Ctx flags)
        {
            if (App == null) return null;
            var result = new AppDto
            {
                Id = App.AppId,
                Url = (App as Apps.IApp)?.Path,
                Name = App.Name,
                Folder = App.Folder,
            };
            if (!flags.HasFlag(Ctx.AppAdvanced)) return result;

            result.GettingStartedUrl = GetGettingStartedUrl();
            result.Identifier = _app.AppGuid;
            result.Permissions = new HasPermissionsDto {Count = App.Metadata.Permissions.Count()};
            return result;
        }
    }
}
