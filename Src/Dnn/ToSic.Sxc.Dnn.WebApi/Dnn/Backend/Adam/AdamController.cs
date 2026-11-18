using System.Web;
using System.Web.Http.Controllers;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Adam.AdamControllerReal<int>;

namespace ToSic.Sxc.Dnn.Backend;

/// <summary>
/// Direct access to app-content items, simple manipulations etc.
/// Should check for security at each standard call - to see if the current user may do this
/// Then we can reduce security access level to anonymous, because each method will do the security check
/// </summary>
[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[SupportedModules(DnnSupportedModuleNames)]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]    // use view, all methods must re-check permissions
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AdamController : DnnApiController, IAdamController<int>
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    /// <summary>
    /// The entity controller probably needs this for every request.
    /// </summary>
    /// <param name="controllerContext"></param>
    protected override void Initialize(HttpControllerContext controllerContext)
    {
        base.Initialize(controllerContext);
        new DnnSxcWebApiInitBlockContextAndShare().InitBlockSpecs(SysHlp, controllerContext.Request, Log);
    }

    #endregion

    private void PreShareAppStateWip(int appId) 
        => SysHlp.GetService<ISxcCurrentContextService>().ShareAppStateOptimized(appId);


    private RealController Real => SysHlp.GetService<RealController>();

    [HttpPost]
    [HttpPut]
    public object Upload(int appId, string contentType, Guid guid, string field, [FromUri] string subFolder = "", bool usePortalRoot = false)
    {
        PreShareAppStateWip(appId);
        return Real.Upload(new(Request, HttpContext.Current.Request), appId, contentType, guid, field, subFolder,
            usePortalRoot);
    }

    // Note: #AdamItemDto - as of now, we must use object because System.Io.Text.Json will otherwise not convert the object correctly :(

    [HttpGet]
    public IEnumerable</*AdamItemDto*/object> Items(int appId, string contentType, Guid guid, string field, string subfolder, bool usePortalRoot = false)
    {
        PreShareAppStateWip(appId);
        return Real.Items(appId, contentType, guid, field, subfolder, usePortalRoot);
    }


    [HttpPost]
    public IEnumerable</*AdamItemDto*/object> Folder(int appId, string contentType, Guid guid, string field, string subfolder, string newFolder, bool usePortalRoot)
    {
        PreShareAppStateWip(appId);
        return Real.Folder(appId, contentType, guid, field, subfolder, newFolder, usePortalRoot);
    }


    [HttpGet]
    public bool Delete(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, bool usePortalRoot)
    {
        PreShareAppStateWip(appId);
        return Real.Delete(appId, contentType, guid, field, subfolder, isFolder, id, usePortalRoot);
    }


    [HttpGet]
    public bool Rename(int appId, string contentType, Guid guid, string field, string subfolder, bool isFolder, int id, string newName, bool usePortalRoot)
    {
        PreShareAppStateWip(appId);
        return Real.Rename(appId, contentType, guid, field, subfolder, isFolder, id, newName, usePortalRoot);
    }
}