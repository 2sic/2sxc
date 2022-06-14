using ToSic.Eav.DI;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Cms.Publishing
{
    public interface IPagePublishingSettings: IHasLog, ISwitchableService
    {
        BlockPublishingSettings SettingsOfModule(int moduleId);
    }
}
