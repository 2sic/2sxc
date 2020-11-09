using ToSic.Eav.Apps.Enums;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Mvc.Run
{
    internal class MvcPagePublishingResolver : PagePublishingResolverBase
    {
        public MvcPagePublishingResolver() : base("Mvc") { }

        protected override PublishingMode LookupRequirements(int instanceId) 
            => PublishingMode.DraftOptional;
    }
}
