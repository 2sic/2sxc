using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Oqt.Server.Cms;

internal class OqtWorkPublishingLookup() : ServiceBase(OqtConstants.OqtLogPrefix + ".PubMode"), IWorkBlockPublishingLookup
{
    public int WorkSequenceOrder => (int)BlockPublishingLookupSequence.Platform;


    public Task<Package<BlockPublishingSettings>> Handle(WorkContext _, Package<BlockPublishingSettings> package)
    {
        return Task.FromResult(BlockPublishingSettingsService.Stop(package, PublishingModes.DraftOptional));
    }
    
}