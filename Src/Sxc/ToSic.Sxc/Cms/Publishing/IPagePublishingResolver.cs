using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing.DI;

namespace ToSic.Sxc.Cms.Publishing
{
    public interface IPagePublishingSettings: IHasLog, ISwitchableService
    {
        BlockPublishingSettings SettingsOfModule(int moduleId);
    }
}
