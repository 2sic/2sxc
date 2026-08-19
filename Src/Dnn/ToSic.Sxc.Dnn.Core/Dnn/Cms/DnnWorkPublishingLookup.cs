using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Dnn.Features;
using ToSic.Sxc.Services;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Dnn.Cms;

/// <summary>
/// Lookup in DNN if the module is in a page with versioning enabled, and if so, return the required publishing mode.
/// </summary>
/// <param name="featuresService"></param>
internal class DnnWorkPublishingLookup(IFeaturesService featuresService)
    : ServiceBase(DnnConstants.LogName + ".PubMode"), IWorkBlockPublishingLookup
{
    public Task<Package<BlockPublishingSettings>> Handle(WorkContext _, Package<BlockPublishingSettings> package)
    {
        if (!featuresService.IsEnabled(DnnBuiltInFeatures.DnnPageWorkflow.NameId))
            return Task.FromResult(package with { Decision = ResultState.Skip });

        var mode = LookupRequirements(package.Data.ModuleId);
        var result = BlockPublishingSettingsService.Stop(package, mode);
        return Task.FromResult(result);
    }


    protected PublishingMode LookupRequirements(int moduleId)
    {
        var l = Log.Fn<PublishingMode>($"Requirements(mod:{moduleId}) - checking first time (others will be cached)");
        try
        {
            // TODO V14 - probably we can set ignoreCache to false then, as it's probably just a workaround for an old bug
            var mod = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
            var versioningEnabled = TabChangeSettings.Instance.IsChangeControlEnabled(mod.PortalID, mod.TabID);
            var mode = !versioningEnabled
                ? PublishingMode.DraftOptional
                : PublishingMode.DraftRequired;
            return l.Return(mode);
        }
        catch (Exception ex)
        {
            l.Done(ex);
            throw;
        }
    }


    public int WorkSequenceOrder => (int)BlockPublishingLookupSequence.Platform;

}