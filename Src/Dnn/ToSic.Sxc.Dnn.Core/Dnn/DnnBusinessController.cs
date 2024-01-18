using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Cms.Internal.Publishing;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Install;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.Search;

namespace ToSic.Sxc.Dnn;

/// <summary>
/// This is the connector-class which DNN consults when it needs to know things about a module
/// It's used in the background, not when the page is loading
/// </summary>
public class DnnBusinessController : ModuleSearchBase, IHasLog
{
    #region Constructor (not DI)

    /// <summary>
    /// Constructor overload for DotNetNuke
    /// (BusinessController needs a parameterless constructor)
    /// </summary>
    public DnnBusinessController() => Log = new Log("DNN.BusCon", null, "starting");

    public ILog Log { get; }

    #endregion


    #region Service Providing

    /// <summary>
    /// Get the service provider only once - ideally in Dnn9.4 we will get it from Dnn
    /// If we would get it multiple times, there are edge cases where it could be different each time! #2614
    /// </summary>
    private IServiceProvider ServiceProvider => _serviceProvider.Get(DnnStaticDi.GetPageScopedServiceProvider);
    private readonly GetOnce<IServiceProvider> _serviceProvider = new();

    #endregion

    #region DNN Interface Members - search, upgrade, versionable

    private IPagePublishing Publishing
    {
        get
        {
            if (_publishing != null) return Publishing;

            // if publishing is used, make sure it's in the log-history
            _publishing = ServiceProvider.Build<IPagePublishing>(Log);
            ServiceProvider.Build<ILogStore>().Add("dnn-publishing", Log);
            return _publishing;
        }
    }
    private IPagePublishing _publishing;


    public int GetLatestVersion(int instanceId) => Publishing.GetLatestVersion(instanceId);

    public int GetPublishedVersion(int instanceId) => Publishing.GetPublishedVersion(instanceId);

    public void PublishVersion(int instanceId, int version)
    {
        Log.A($"publish m#{instanceId}, v:{version}");
        Publishing.Publish(instanceId, version);

        try
        {
            DnnLogging.LogToDnn("Publishing", "ok", Log, force: true);
        }
        catch
        {
            // ignore
        }
    }



    public void DeleteVersion(int instanceId, int version) 
        => Log.A("delete version is not supported");

    public int RollBackVersion(int instanceId, int version)
    {
        Log.A("DNN tried to rollback version " + version + ", but 2sxc does not support this.");

        // Return the currently published version, because this is what the module's state is after this operation
        return Publishing.GetPublishedVersion(instanceId);
    }

    /// <summary>
    /// This is part of the IUpgradeable of DNN
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public string UpgradeModule(string version)
    {
        var l = Log.Fn<string>($"upgrade module - start for v:{version}");
        var installer = ServiceProvider.Build<DnnEnvironmentInstaller>(Log);
        var res = installer.UpgradeModule(version, true);
        Log.A($"result:{res}");
        DnnLogging.LogToDnn("Upgrade", "ok", Log, force: true); // always log, this often causes hidden problems
        return l.ReturnAndLog(res);
    }

    public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
    {
        try
        {
            return ServiceProvider.Build<SearchController>(Log)
                .GetModifiedSearchDocuments(((DnnModule)ServiceProvider.Build<IModule>(Log)).Init(moduleInfo), beginDate);
        }
        catch (Exception e)
        {
            DnnEnvironmentLogger.AddSearchExceptionToLog(moduleInfo, e, nameof(DnnBusinessController));
            return new List<SearchDocument>();
        }
    }

    #endregion
        
}