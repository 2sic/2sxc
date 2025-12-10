using ToSic.Eav.WebApi.Sys.Install;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.WebApi.Sys;
using RealController = ToSic.Sxc.Backend.Sys.InstallControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class InstallController()
    : DnnSxcControllerRoot(RealController.LogSuffix), IInstallController<HttpResponseMessage>
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <summary>
    /// Make sure that these requests don't land in the normal api-log.
    /// Otherwise each log-access would re-number what item we're looking at
    /// </summary>
    protected override string HistoryLogGroup => "web-api.install";


    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public bool Resume()
        => Real.Resume();

    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public InstallAppsDto InstallSettings(bool isContentApp) 
        => Real.InstallSettings(isContentApp, ((DnnModule)SysHlp.GetService<IModule>()).Init(Request.FindModuleInfo()));


    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken] // Now with RVT as it's post now. Previously not, because this was a GET and could not include the RVT
    public HttpResponseMessage RemotePackage(string packageUrl, string newName = null)
    {
        SysHlp.PreventServerTimeout600();
        // Make sure the Scoped ResponseMaker has this controller context
        SysHlp.SetupResponseMaker(this);
        return Real.RemotePackage(packageUrl, ((DnnModule)SysHlp.GetService<IModule>()).Init(ActiveModule), newName);
    }
}