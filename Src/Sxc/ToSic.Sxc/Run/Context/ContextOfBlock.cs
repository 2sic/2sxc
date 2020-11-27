using System;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Run;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Run.Context
{
    public class ContextOfBlock: ContextOfSite, IContextOfBlock
    {
        public ContextOfBlock(IServiceProvider serviceProvider, ISite site, IUser user): base(serviceProvider, site, user)
        {
        }

        public IContextOfBlock Init(IPage page, IContainer container, BlockPublishingState publishing)
        {
            Page = page;
            Container = container;
            Publishing = publishing;
            return this;
        }


        public IPage Page { get; set; }
        public IContainer Container { get; set; }
        public virtual BlockPublishingState Publishing { get; private set; } = new BlockPublishingState();

        public new IContextOfSite Clone() => new ContextOfBlock(ServiceProvider, Site, User)
            .Init(Page, Container, Publishing);
    }
}
