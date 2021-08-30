using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Logging;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Context
{
    public class ContextOfBlock: ContextOfApp, IContextOfBlock
    {
        #region Constructor / DI

        public ContextOfBlock(
            IServiceProvider serviceProvider, 
            ISite site, 
            IUser user,
            IPage page, 
            IModule module, 
            Lazy<IPagePublishingResolver> publishingResolver, IAppStates appStates)
            : base(serviceProvider, site, user, appStates)
        {
            Page = page;
            Module = module;
            _publishingResolver = publishingResolver;
            Log.Rename("Sxc.CtxBlk");
        }
        private readonly Lazy<IPagePublishingResolver> _publishingResolver;

        #endregion

        #region Override AppIdentity based on module information

        protected override IAppIdentity AppIdentity
        {
            get
            {
                if (base.AppIdentity != null) return base.AppIdentity;
                var wrapLog = Log.Call<IAppIdentity>();
                var identifier = Module?.BlockIdentifier;
                if (identifier == null) return wrapLog("no mod-block-id", null);
                AppIdentity = identifier;
                return wrapLog(null, base.AppIdentity);
            }
        }

        #endregion

        /// <inheritdoc />
        public IPage Page { get; }

        /// <inheritdoc />
        public IModule Module { get; }

        /// <inheritdoc />
        public BlockPublishingState Publishing => _publishing ?? (_publishing = _publishingResolver.Value.GetPublishingState(Module?.Id ?? -1));
        private BlockPublishingState _publishing;

        /// <inheritdoc />
        public new IContextOfSite Clone(ILog parentLog) => new ContextOfBlock(ServiceProvider, Site, User, Page, Module, _publishingResolver, AppStates)
            .Init(parentLog);
    }
}
