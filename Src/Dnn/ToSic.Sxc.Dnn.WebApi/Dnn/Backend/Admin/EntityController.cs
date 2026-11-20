using System.Web.Http.Controllers;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.ImportExport.Sys.Options;
using ToSic.Eav.WebApi.Admin.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.ImportExport;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using Guid = System.Guid;
using RealController = ToSic.Eav.WebApi.Admin.Sys.EntityControllerReal;

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
[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[ShowApiWhenReleased(ShowApiMode.Never)]
public class EntityController : DnnApiController, IEntityController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    #endregion


    private RealController Real => SysHlp.GetService<RealController>();


    /// <inheritdoc/>
    [HttpDelete]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public void Delete(string contentType, int appId, int? id = null, Guid? guid = null, bool force = false, int? parentId = null, string parentField = null)
    {
        SysHlp.PublishContext(Request, new(TryBlock: true, TryApp: true, AppId: appId), Log);
        Real.Delete(contentType, appId, id, guid, force, parentId, parentField);
    }


    /// <inheritdoc/>
    [HttpGet]
    [AllowAnonymous] // will do security check internally
    public HttpResponseMessage Json(int appId, int id, string prefix, bool withMetadata)
    {
        SysHlp.PublishContext(Request, new(TryBlock: true, TryApp: true, AppId: appId), Log);
        return Real.Json(appId, id, prefix, withMetadata).ToHttpResponse();
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
        SysHlp.PublishContext(Request, new(TryBlock: true, TryApp: true, AppId: appId), Log);
        return Real.Download(appId, language, defaultLanguage, contentType, recordExport, resourcesReferences,
            languageReferences, selectedIds).ToHttpResponse();
    }


    /// <inheritdoc/>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public ContentImportResult XmlPreview(ContentFileImportOptions args)
    {
        SysHlp.PublishContextBlockOnly(Request, Log);
        return Real.XmlPreview(args);
    }


    /// <inheritdoc/>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public ContentImportResult XmlUpload(ContentFileImportOptions args)
    {
        SysHlp.PublishContextBlockOnly(Request, Log);
        return Real.XmlUpload(args);
    }


    /// <inheritdoc/>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public bool Upload(DataFileImportOptions args)
    {
        SysHlp.PublishContextBlockOnly(Request, Log);
        return Real.Upload(args);
    }
}
