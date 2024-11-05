using System.Web;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.Backend.Admin;
using RealController = ToSic.Sxc.Backend.Admin.TypeControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <summary>
/// Web API Controller for Content-Type structures, fields etc.
/// </summary>
/// <remarks>
/// Because download JSON call is made in a new window, they won't contain any http-headers like module-id or security token. 
/// So we can't use the classic protection attributes to the class like:
/// - [SupportedModules(DnnSupportedModuleNames)]
/// - [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
/// - [ValidateAntiForgeryToken]
/// Instead, each method must have all attributes, or do additional security checking.
/// Security checking is possible, because the cookie still contains user information
/// </remarks>
[DnnLogExceptions]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TypeController() : DnnSxcControllerBase(RealController.LogSuffix), ITypeController
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <summary>
    /// Get a list of all content-types.
    /// See https://docs.2sxc.org/basics/data/content-types/index.html
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="scope"></param>
    /// <param name="withStatistics"></param>
    /// <returns></returns>
    [HttpGet]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public IEnumerable<ContentTypeDto> List(int appId, string scope = null, bool withStatistics = false) => Real.List(appId, scope, withStatistics);


    /// <summary>
    /// Used to be GET Scopes.
    /// Scopes are a way to organize content types, see https://docs.2sxc.org/basics/data/content-types/scopes.html
    /// </summary>
    [HttpGet]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public ScopesDto Scopes(int appId) => Real.Scopes(appId);


    /// <summary>
    /// Used to be GET ContentTypes.
    /// See https://docs.2sxc.org/basics/data/content-types/index.html
    /// </summary>
    [HttpGet]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public ContentTypeDto Get(int appId, string contentTypeId, string scope = null) => Real.Get(appId, contentTypeId, scope);


    /// <summary>
    /// Delete a Content-Type
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="staticName"></param>
    /// <returns></returns>
    /// <remarks>
    /// ATM it requires the DELETE verb, but this often causes problems on IIS with WebDav.
    /// TODO: probably switch over to use Get again, even if it's not as descriptive as delete
    /// </remarks>
    [HttpDelete]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public bool Delete(int appId, string staticName) => Real.Delete(appId, staticName);


    /// <summary>
    /// Save a Content-Type.
    /// See https://docs.2sxc.org/basics/data/content-types/index.html
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    // 2019-11-15 2dm special change: item to be Dictionary<string, object> because in DNN 9.4
    // it causes problems when a content-type has metadata, where a value then is a deeper object
    // in future, the JS front-end should send something clearer and not the whole object
    public bool Save(int appId, Dictionary<string, object> item) => Real.Save(appId, item);


    /// <summary>
    /// Used to add a Ghost content-type.
    /// See https://docs.2sxc.org/basics/data/content-types/range-app-shared.html
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="sourceStaticName"></param>
    /// <returns></returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public bool AddGhost(int appId, string sourceStaticName) => Real.AddGhost(appId, sourceStaticName);


    /// <summary>
    /// Change which attribute on a Content-Type is the title. 
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="contentTypeId"></param>
    /// <param name="attributeId"></param>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public void SetTitle(int appId, int contentTypeId, int attributeId) => Real.SetTitle(appId, contentTypeId, attributeId);


    /// <summary>
    /// Export a Content-Type as JSON
    /// </summary>
    /// <remarks>
    /// New in 2sxc 11.07
    /// </remarks>
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public HttpResponseMessage Json(int appId, string name)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        SysHlp.SetupResponseMaker(this);
        return Real.Json(appId, name);
    }


    /// <summary>
    /// Used to be POST ImportExport/ImportContent
    /// </summary>
    /// <remarks>
    /// New in 2sxc 11.07
    /// </remarks>
    /// <returns></returns>
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public ImportResultDto Import(int zoneId, int appId)
    {
        SysHlp.PreventServerTimeout300();
        return Real.Import(new(Request, HttpContext.Current.Request), zoneId, appId);
    }

    /// <summary>
    /// Json Bundle Export
    /// </summary>
    /// <remarks>
    /// New in 2sxc v15.x
    /// </remarks>
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public HttpResponseMessage JsonBundleExport(int appId, Guid exportConfiguration, int indentation = 0)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        SysHlp.SetupResponseMaker(this);
        return Real.JsonBundleExport(appId, exportConfiguration, indentation);
    }

}