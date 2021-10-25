using ToSic.Eav;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Cms.Publishing
{
    internal class BasicPagePublishingResolver : PagePublishingResolverBase
    {
        public BasicPagePublishingResolver(WarnUseOfUnknown<BasicPagePublishingResolver> warn) : base(LogNames.NotImplemented) { }

        protected override PublishingMode LookupRequirements(int instanceId) 
            => PublishingMode.DraftOptional;
    }
}
