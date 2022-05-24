using System;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Services;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
    /// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
    /// </summary>
    public class DynamicCodeService: HasLog, IDynamicCodeService
    {
        private readonly Dependencies _dependencies;

        #region Constructor and Init

        public class Dependencies
        {
            public Dependencies(IServiceProvider serviceProvider, Lazy<LogHistory> history, Lazy<IUser> user, 
                // Dependencies to get primary app
                Lazy<ISite> site,
                Lazy<IZoneMapper> zoneMapper, 
                Lazy<IAppStates> appStates)
            {
                ServiceProvider = serviceProvider;
                History = history;
                User = user;
                Site = site;
                ZoneMapper = zoneMapper;
                AppStates = appStates;
            }
            internal IServiceProvider ServiceProvider { get; }
            public Lazy<LogHistory> History { get; }
            public Lazy<IUser> User { get; }
            public Lazy<ISite> Site { get; }
            public Lazy<IZoneMapper> ZoneMapper { get; }
            public Lazy<IAppStates> AppStates { get; }
        }

        public DynamicCodeService(Dependencies dependencies): base($"{Constants.SxcLogName}.DCS")
        {
            _dependencies = dependencies;
            var newScopedServiceProvider = dependencies.ServiceProvider.CreateScope().ServiceProvider;
            // Important: These generators must be built inside the scope, so they must be made here
            // and not come from the constructor injection
            CodeRootGenerator = newScopedServiceProvider.Build<Generator<DynamicCodeRoot>>();
            AppGenerator = newScopedServiceProvider.Build<Generator<App>>();
            AppConfigDelegateGenerator = newScopedServiceProvider.Build<GeneratorLog<AppConfigDelegate>>().SetLog(Log);
            ModuleAndBlockBuilder = newScopedServiceProvider.Build<LazyInitLog<IModuleAndBlockBuilder>>().SetLog(Log);
        }
        protected readonly Generator<DynamicCodeRoot> CodeRootGenerator;
        protected readonly Generator<App> AppGenerator;
        protected readonly GeneratorLog<AppConfigDelegate> AppConfigDelegateGenerator;
        protected readonly LazyInitLog<IModuleAndBlockBuilder> ModuleAndBlockBuilder;

        public IDynamicCodeService Init(ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _logInitDone = true;
            return this;
        }
        private bool _logInitDone;

        protected void MakeSureLogIsInHistory()
        {
            if (_logInitDone) return;
            _logInitDone = true;
            _dependencies.History.Value.Add("dynamic-code-service", base.Log);
        }

        #endregion


        /// <inheritdoc />
        public IDynamicCode12 OfApp(int appId) => OfAppInternal(appId: appId);
        /// <inheritdoc />
        public IDynamicCode12 OfApp(int zoneId, int appId) => OfAppInternal(zoneId: zoneId, appId: appId);

        /// <inheritdoc />
        public IDynamicCode12 OfApp(IAppIdentity appIdentity) => OfAppInternal(zoneId: appIdentity.ZoneId, appId: appIdentity.AppId);

        private IDynamicCode12 OfAppInternal(int? zoneId = null, int? appId = null)
        {
            var wrapLog = Log.Fn<IDynamicCode12>();
            MakeSureLogIsInHistory();
            var codeRoot = CodeRootGenerator.New.InitDynCodeRoot(null, Log, Constants.CompatibilityLevel12);
            var app = App(zoneId: zoneId, appId: appId);
            codeRoot.AttachApp(app);
            return wrapLog.Return(codeRoot, "ok");
        }

        public IDynamicCode12 OfModule(int pageId, int moduleId)
        {
            var wrapLog = Log.Fn<IDynamicCodeRoot>($"{pageId}, {moduleId}");
            MakeSureLogIsInHistory();
            var cmsBlock = ModuleAndBlockBuilder.Ready.GetBlock(pageId, moduleId);
            var codeRoot = CodeRootGenerator.New.InitDynCodeRoot(cmsBlock, Log, Constants.CompatibilityLevel12);

            return wrapLog.Return(codeRoot, "ok");
        }


        public IDynamicCode12 OfSite() => OfApp(GetPrimaryApp(null, _dependencies.Site.Value));

        public IDynamicCode12 OfSite(int siteId) => OfApp(GetPrimaryApp(siteId, null));

        /// <inheritdoc />
        public IApp App(
            string noParamOrder = Eav.Parameters.Protector,
            int? zoneId = null,
            int? appId = null,
            ISite site = null,
            bool? withUnpublished = null)
        {
            MakeSureLogIsInHistory();
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(App),
                $"{nameof(zoneId)}, {nameof(appId)} (required), {nameof(site)}, {nameof(withUnpublished)}");

            // Ensure AppId is provided
            var realAppId = appId ?? throw new ArgumentException($"At least the {nameof(appId)} is required and must be a valid AppId", nameof(appId));

            // todo: lookup zoneId if not provided
            var realZoneId = zoneId ?? AppConstants.AutoLookupZone;
            return App(new AppIdentity(zoneId ?? Eav.Constants.IdNotInitialized, realAppId), site, withUnpublished: withUnpublished);
        }

        public IApp AppOfSite() => AppOfSite(siteId: null);

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public IApp AppOfSite(string noParamOrder = Parameters.Protector, int? siteId = null, ISite site = null, bool? withUnpublished = null)
        {
            var primaryApp = GetPrimaryApp(siteId, site);
            return App(primaryApp, site, withUnpublished);
        }

        private AppState GetPrimaryApp(int? siteId, ISite site)
        {
            siteId = siteId ?? site?.Id ?? _dependencies.Site.Value.Id;
            var zoneId = _dependencies.ZoneMapper.Value.GetZoneId(siteId.Value);
            var primaryApp = _dependencies.AppStates.Value.GetPrimaryApp(zoneId, Log);
            return primaryApp;
        }


        private IApp App(IAppIdentity appIdentity, ISite site, bool? withUnpublished = null)
        {
            var wrapLog = Log.Fn<IApp>($"{appIdentity.LogState()}, site:{site != null}, showDrafts: {withUnpublished}");
            var app = AppGenerator.New;
            if (site != null) app.PreInit(site);
            var appStuff = app.Init(appIdentity, AppConfigDelegateGenerator.New.Build(withUnpublished ?? _dependencies.User.Value.IsAdmin), Log);
            return wrapLog.Return(appStuff);
        }

    }
}
