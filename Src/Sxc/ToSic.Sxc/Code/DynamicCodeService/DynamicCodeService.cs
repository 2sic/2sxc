using System;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Context;
using ToSic.Sxc.LookUp;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Code
{
    /// <summary>
    /// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
    /// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
    /// </summary>
    public abstract class DynamicCodeService: HasLog, IDynamicCodeService
    {
        #region Constructor and Init

        protected DynamicCodeService(IServiceProvider serviceProvider, Lazy<LogHistory> history, string logPrefix): base($"{logPrefix}.DCS")
        {
            _history = history;
            var serviceScope = serviceProvider.CreateScope();
            ServiceProvider = serviceScope.ServiceProvider;
        }
        protected IServiceProvider ServiceProvider { get; }
        private readonly Lazy<LogHistory> _history;

        public IDynamicCodeService Init(ILog parentLog)
        {
            Log.LinkTo(parentLog);
            _logInitDone = true;
            return this;
        }
        private bool _logInitDone;

        private void MakeSureLogIsInHistory()
        {
            if (_logInitDone) return;
            _logInitDone = true;
            _history.Value.Add("dynamic-code-service", base.Log);
        }

        #endregion

        public IDynamicCode OfApp(int appId)
        {
            var ctxResolver = ServiceProvider.Build<IContextResolver>();
            var appCtx = ctxResolver.App(appId);
            var dynCodeRoot = ServiceProvider.Build<DynamicCodeRoot>();
            // dynCodeRoot.Init()
            return null;
        }

        public abstract IDynamicCode12 OfModule(int pageId, int moduleId);

        //public IDynamicCode OfModule(int pageId, int moduleId) => null;


        /// <summary>
        /// Get a full app-object for accessing data of the app from outside
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="zoneId"></param>
        /// <param name="appId">The AppID of the app you need</param>
        /// <param name="site">The owner portal - this is important when retrieving Apps from another portal.</param>
        /// <param name="withUnpublished">Show draft items - usually false for visitors, true for editors/admins.</param>
        /// <returns>An initialized App object which you can use to access App.Data</returns>
        public IApp App(
            string noParamOrder = Eav.Parameters.Protector,
            int zoneId = Eav.Constants.IdNotInitialized,
            int appId = Eav.Constants.IdNotInitialized,
            ISite site = null,
            bool withUnpublished = false)
        {
            MakeSureLogIsInHistory();
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(App),
                $"{nameof(zoneId)}, {nameof(appId)} (required), {nameof(site)}, {nameof(withUnpublished)}");

            // Ensure AppId is provided
            if (appId < 1) throw new ArgumentException($"At least the {nameof(appId)} is required and must be a valid AppId", nameof(appId));

            // todo: lookup zoneid if not provided
            zoneId = zoneId != Eav.Constants.IdNotInitialized ? zoneId : AppConstants.AutoLookupZone;
            return App(zoneId, appId, site, withUnpublished: withUnpublished);
        }


        private IApp App(int zoneId, int appId, ISite site, bool withUnpublished)
        {
            var wrapLog = Log.Call<IApp>($"z:{zoneId}, a:{appId}, site:{site != null}, showDrafts: {withUnpublished}");
            var app = ServiceProvider.Build<App>();
            if (site != null) app.PreInit(site);
            var appStuff = app.Init(new AppIdentity(zoneId, appId),
                ServiceProvider.Build<AppConfigDelegate>().Init(Log).Build(withUnpublished),
                Log);
            return wrapLog(null, appStuff);
        }

    }
}
