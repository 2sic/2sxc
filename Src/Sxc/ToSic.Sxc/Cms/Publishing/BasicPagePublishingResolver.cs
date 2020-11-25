using ToSic.Eav;
using ToSic.Eav.Apps.Enums;

namespace ToSic.Sxc.Cms.Publishing
{
    internal class BasicPagePublishingResolver : PagePublishingResolverBase
    {
        public BasicPagePublishingResolver() : base(LogNames.NotImplemented) { }

        protected override PublishingMode LookupRequirements(int instanceId) 
            => PublishingMode.DraftOptional;
    }
}
