using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
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
            Lazy<IPagePublishingResolver> publishingResolver,
            PageServiceShared pageServiceShared,
            ContextOfSiteDependencies contextOfSiteDependencies,
            ContextOfAppDependencies appDependencies)
            : base(contextOfSiteDependencies, appDependencies)
        {
            Page = page;
            Module = module;
            PageServiceShared = pageServiceShared;
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

        public PageServiceShared PageServiceShared { get; }

        /// <inheritdoc />
        public BlockPublishingState Publishing => _publishing ?? (_publishing = _publishingResolver.Value.GetPublishingState(Module?.Id ?? -1));
        private BlockPublishingState _publishing;

        /// <inheritdoc />
        public new IContextOfSite Clone(ILog parentLog) => new ContextOfBlock(Page, Module, _publishingResolver, PageServiceShared, Dependencies, Deps)
            .Init(parentLog);
    }
}
