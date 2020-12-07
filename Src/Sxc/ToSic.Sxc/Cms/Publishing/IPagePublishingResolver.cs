using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Cms.Publishing
{
    public interface IPagePublishingResolver: IHasLog<IPagePublishingResolver>
    {
        BlockPublishingState GetPublishingState(int instanceId);
    }
}
