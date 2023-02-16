using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Web.PageService;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("Internal stuff, not for public use")]
    public class ContextOfBlock: ContextOfApp, IContextOfBlock
    {
        #region Constructor / DI

        public ContextOfBlock(
            IPage page, 
            IModule module,
            LazySvc<ServiceSwitcher<IPagePublishingGetSettings>> publishingResolver,
            PageServiceShared pageServiceShared,
            ContextOfSite.MyServices siteCtxDeps,
            ContextOfApp.MyServices appServices)
            : base(siteCtxDeps, appServices, "Sxc.CtxBlk")
        {
            Page = page;
            ConnectServices(
                Module = module,
                PageServiceShared = pageServiceShared,
                _publishingResolver = publishingResolver
            );

            // 2022-12-21 2dm removed this now that we have connectServices - could cause side-effects?
            // special check to prevent duplicate SetLog, because it could be cloned and already initialized
            //if (!_publishingResolver.HasInitCall)
            //    _publishingResolver.SetLog(Log);
            //Log.Rename("Sxc.CtxBlk");
        }
        private readonly LazySvc<ServiceSwitcher<IPagePublishingGetSettings>> _publishingResolver;

        #endregion

        #region Override AppIdentity based on module information

        protected override IAppIdentity AppIdentity
        {
            get
            {
                if (base.AppIdentity != null) return base.AppIdentity;
                var wrapLog = Log.Fn<IAppIdentity>();
                var identifier = Module?.BlockIdentifier;
                if (identifier == null) return wrapLog.ReturnNull("no mod-block-id");
                AppIdentity = identifier;
                return wrapLog.Return(base.AppIdentity);
            }
        }

        #endregion

        /// <inheritdoc />
        public IPage Page { get; }

        /// <inheritdoc />
        public IModule Module { get; }

        public PageServiceShared PageServiceShared { get; }

        /// <inheritdoc />
        public BlockPublishingSettings Publishing => _publishing ?? (_publishing = _publishingResolver.Value.Value.SettingsOfModule(Module?.Id ?? -1));
        private BlockPublishingSettings _publishing;

        /// <inheritdoc />
        public new IContextOfSite Clone(ILog parentLog) => new ContextOfBlock(Page, Module, _publishingResolver, PageServiceShared, SiteDeps, Deps)
            .LinkLog(parentLog);
    }
}
