using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Cms.Publishing
{
    public abstract class PagePublishingResolverBase: HasLog<IPagePublishingResolver>, IPagePublishingResolver
    {
        protected PagePublishingResolverBase(string logName) : base(logName) { }

        //public abstract bool Supported { get; }

        public abstract PublishingMode Requirements(int instanceId);

        //public bool IsEnabled(int instanceId) => RequirementsIsEnabled(PublishingMode.DraftOptional);

        protected bool RequirementsIsEnabled(PublishingMode mode) => mode != PublishingMode.DraftOptional;

        public InstancePublishingState GetPublishingState(int instanceId)
        {
            var mode = Requirements(instanceId);
            return new InstancePublishingState { ForceDraft = RequirementsIsEnabled(mode), Mode = mode };
        }

    }
}
