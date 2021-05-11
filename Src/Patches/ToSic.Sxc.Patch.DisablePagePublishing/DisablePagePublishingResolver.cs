using ToSic.Eav;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Patch.DisablePagePublishing
{
    public class DisablePagePublishingResolver : PagePublishingResolverBase
    {
        public DisablePagePublishingResolver() : base(LogNames.NotImplemented) { }

        protected override PublishingMode LookupRequirements(int instanceId)
        {
            try
            {
                if (!_alreadyLogged)
                {
                    // Use existing logger, add some messages once
                    Log.Add($"{nameof(DisablePagePublishingResolver)} is active.");
                    Log.Add("This log will only be added once so you can see it was activated.");
                    Log.Add($"All requests to {nameof(LookupRequirements)} will return {PublishingMode.DraftOptional}");
                    
                    // Then add to special section in insights
                    History.Add(Constants.PatchesHistoryName, Log);
                }

                _alreadyLogged = true;
            }
            catch { /* ignore */ }
            return PublishingMode.DraftOptional;
        }

        private static bool _alreadyLogged;
    }
}