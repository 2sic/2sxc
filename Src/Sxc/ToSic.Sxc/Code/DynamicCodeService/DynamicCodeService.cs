using System;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
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
            public Dependencies(IServiceProvider serviceProvider, Lazy<LogHistory> history, Lazy<IUser> user)
            {
                ServiceProvider = serviceProvider;
                History = history;
                User = user;
            }
            public IServiceProvider ServiceProvider { get; }
            public Lazy<LogHistory> History { get; }
            public Lazy<IUser> User { get; }
        }

        public DynamicCodeService(Dependencies dependencies): base($"{Constants.SxcLogName}.DCS")
        {
            _dependencies = dependencies;
            ServiceProvider = dependencies.ServiceProvider.CreateScope().ServiceProvider;
            // Important: These generators must be built inside the scope, so they must be made here
            // and not come from the constructor injection
            CodeRootGenerator = ServiceProvider.Build<Generator<DynamicCodeRoot>>();
            AppGenerator = ServiceProvider.Build<Generator<App>>();
            AppConfigDelegateGenerator = ServiceProvider.Build<GeneratorLog<AppConfigDelegate>>().SetLog(Log);
            ModuleAndBlockBuilder = ServiceProvider.Build<LazyInitLog<IModuleAndBlockBuilder>>().SetLog(Log);
        }
        protected IServiceProvider ServiceProvider { get; }
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
            var wrapLog = Log.Call<IDynamicCode12>();
            MakeSureLogIsInHistory();
            var codeRoot = CodeRootGenerator.New.Init(null, Log, Constants.CompatibilityLevel12);
            var app = App(zoneId: zoneId, appId: appId);
            codeRoot.AttachApp(app);
            return wrapLog("ok", codeRoot);
        }

        public IDynamicCode12 OfModule(int pageId, int moduleId)
        {
            var wrapLog = Log.Call<IDynamicCodeRoot>($"{pageId}, {moduleId}");
            MakeSureLogIsInHistory();
            //var module = ModuleAndBlockBuilder.Ready.GetModule(pageId, moduleId);
            var cmsBlock = ModuleAndBlockBuilder.Ready.GetBlock(pageId, moduleId);
            var codeRoot = CodeRootGenerator.New.Init(cmsBlock, Log, Constants.CompatibilityLevel12);

            return wrapLog("ok", codeRoot);
        }

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
            return App(zoneId ?? Eav.Constants.IdNotInitialized, realAppId, site, withUnpublished: withUnpublished);
        }


        private IApp App(int zoneId, int appId, ISite site, bool? withUnpublished = null)
        {
            var wrapLog = Log.Call<IApp>($"z:{zoneId}, a:{appId}, site:{site != null}, showDrafts: {withUnpublished}");
            var app = AppGenerator.New;
            if (site != null) app.PreInit(site);
            var appStuff = app.Init(new AppIdentity(zoneId, appId),
                AppConfigDelegateGenerator.New.Build(withUnpublished ?? _dependencies.User.Value.IsAdmin),
                Log);
            return wrapLog(null, appStuff);
        }

    }
}
