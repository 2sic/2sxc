using ToSic.Eav.ImportExport.Internal.Options;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using Guid = System.Guid;
using RealController = ToSic.Eav.WebApi.Admin.EntityControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <summary>
/// Proxy Class to the EAV EntitiesController (Web API Controller)
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
public class EntityController() : DnnSxcControllerBase(RealController.LogSuffix), IEntityController
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc/>
    [HttpGet]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public IEnumerable<Dictionary<string, object>> List(int appId, string contentType)
        => Real.List(appId, contentType);


    /// <inheritdoc/>
    [HttpDelete]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public void Delete(string contentType, int appId, int? id = null, Guid? guid = null, bool force = false, int? parentId = null, string parentField = null)
        => Real.Delete(contentType, appId, id, guid, force, parentId, parentField);


    /// <inheritdoc/>
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public HttpResponseMessage Json(int appId, int id, string prefix, bool withMetadata)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        SysHlp.SetupResponseMaker(this);

        return Real.Json(appId, id, prefix, withMetadata);
    }


    /// <inheritdoc/>
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public HttpResponseMessage Download(
        int appId,
        string language,
        string defaultLanguage,
        string contentType,
        ExportSelection recordExport, 
        ExportResourceReferenceMode resourcesReferences,
        ExportLanguageResolution languageReferences, 
        string selectedIds = null)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        SysHlp.SetupResponseMaker(this);

        return Real.Download(appId, language, defaultLanguage, contentType, recordExport, resourcesReferences,
            languageReferences, selectedIds);
    }


    /// <inheritdoc/>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public ContentImportResultDto XmlPreview(ContentImportArgsDto args)
        => Real.XmlPreview(args);


    /// <inheritdoc/>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public ContentImportResultDto XmlUpload(ContentImportArgsDto args)
        => Real.XmlUpload(args);


    /// <inheritdoc/>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public bool Upload(EntityImportDto args) => Real.Upload(args);


    ///// <inheritdoc/>
    //// not final yet, so no [HttpGet]
    //public dynamic Usage(int appId, Guid guid) => Real.Usage(appId, guid);
}