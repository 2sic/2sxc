using System;
using ToSic.Eav.Context;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Context
{
    public class ContextOfBlock: ContextOfApp, IContextOfBlock
    {
        #region Constructor / DI

        public ContextOfBlock(IServiceProvider serviceProvider, ISite site, IUser user,
            IPage page, IModule module, Lazy<IPagePublishingResolver> publishingResolver)
            : base(serviceProvider, site, user)
        {
            Page = page;
            Module = module;
            _publishingResolver = publishingResolver;
            Log.Rename("Sxc.CtxBlk");
        }
        private readonly Lazy<IPagePublishingResolver> _publishingResolver;

        #endregion

        /// <inheritdoc />
        public IPage Page { get; set; }

        /// <inheritdoc />
        public IModule Module { get; set; }

        /// <inheritdoc />
        public BlockPublishingState Publishing => _publishing ?? (_publishing = _publishingResolver.Value.GetPublishingState(Module?.Id ?? -1));
        private BlockPublishingState _publishing;

        /// <inheritdoc />
        public new IContextOfSite Clone() => new ContextOfBlock(ServiceProvider, Site, User, Page, Module, _publishingResolver);
    }
}
