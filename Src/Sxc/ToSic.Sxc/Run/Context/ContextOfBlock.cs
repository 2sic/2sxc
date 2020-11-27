using System;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Run;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Run.Context
{
    public class ContextOfBlock: ContextOfSite, IContextOfBlock
    {
        #region Constructor / DI

        public ContextOfBlock(IServiceProvider serviceProvider, ISite site, IUser user,
            IPage page, IContainer container, Lazy<IPagePublishingResolver> publishingResolver)
            : base(serviceProvider, site, user)
        {
            Page = page;
            Container = container;
            _publishingResolver = publishingResolver;
        }
        private readonly Lazy<IPagePublishingResolver> _publishingResolver;

        #endregion

        /// <inheritdoc />
        public IPage Page { get; set; }

        /// <inheritdoc />
        public IContainer Container { get; set; }

        /// <inheritdoc />
        public BlockPublishingState Publishing => _publishing ?? (_publishing = _publishingResolver.Value.GetPublishingState(Container?.Id ?? -1));
        private BlockPublishingState _publishing;

        /// <inheritdoc />
        public new IContextOfSite Clone() => new ContextOfBlock(ServiceProvider, Site, User, Page, Container, _publishingResolver);
    }
}
