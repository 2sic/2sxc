using ToSic.Eav.Apps.Enums;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Mvc.NotImplemented
{
    internal class NotImplementedPagePublishingResolver : PagePublishingResolverBase
    {
        public NotImplementedPagePublishingResolver() : base("Mvc") { }

        protected override PublishingMode LookupRequirements(int instanceId) 
            => PublishingMode.DraftOptional;
    }
}
