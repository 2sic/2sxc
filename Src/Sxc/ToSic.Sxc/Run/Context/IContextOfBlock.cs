using ToSic.Eav.Apps.Run;
using ToSic.Eav.Run;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Run.Context
{
    public interface IContextOfBlock: IContextOfSite
    {
        IContextOfBlock Init(IPage page, IContainer container, BlockPublishingState publishing);

        IPage Page { get; }

        IContainer Container { get; }

        BlockPublishingState Publishing { get; }
    }
}
