using System;
using ToSic.Sxc.Cms.Publishing;

// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Context
{
    public class ContextOfBlock: ContextOfSite, IContextOfBlock
    {
        #region Constructor / DI

        public ContextOfBlock(IServiceProvider serviceProvider, ISite site, IUser user,
            IPage page, IModule module, Lazy<IPagePublishingResolver> publishingResolver)
            : base(serviceProvider, site, user)
        {
            Page = page;
            Container = module;
            _publishingResolver = publishingResolver;
        }
        private readonly Lazy<IPagePublishingResolver> _publishingResolver;

        #endregion

        /// <inheritdoc />
        public IPage Page { get; set; }

        /// <inheritdoc />
        public IModule Container { get; set; }

        /// <inheritdoc />
        public BlockPublishingState Publishing => _publishing ?? (_publishing = _publishingResolver.Value.GetPublishingState(Container?.Id ?? -1));
        private BlockPublishingState _publishing;

        /// <inheritdoc />
        public new IContextOfSite Clone() => new ContextOfBlock(ServiceProvider, Site, User, Page, Container, _publishingResolver);
    }
}
