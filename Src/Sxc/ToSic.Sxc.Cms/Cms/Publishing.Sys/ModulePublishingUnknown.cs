namespace ToSic.Sxc.Cms.Publishing.Sys;

internal class ModulePublishingUnknown(WarnUseOfUnknown<ModulePublishingUnknown> _) : ServiceBase($"{LogScopes.NotImplemented}.Publsh"), IModulePublishing
{
    public int GetLatestVersion(int moduleId) => 0;

    public int GetPublishedVersion(int moduleId) => 0;


    public void Publish(int moduleId, int version)
    {
        Log.A($"Publish(m:{moduleId}, v:{version})");
        Log.A("publish never happened ");
    }

}
