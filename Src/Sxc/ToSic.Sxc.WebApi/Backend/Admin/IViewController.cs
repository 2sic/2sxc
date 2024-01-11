using ToSic.Eav.WebApi.Context;
using ToSic.Sxc.Backend.Views;
#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.Admin;

/// <summary>
/// Provide information about views and manage views as needed.
/// This should also work on Oqtane once released May the 4th 2021 :)
/// </summary>
/// <remarks>
/// Because download JSON call is made in a new window, they won't contain any http-headers like module-id or security token. 
/// So we can't use the classic protection attributes to the class like:
/// - [SupportedModules("2sxc,2sxc-app")]
/// - [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
/// - [ValidateAntiForgeryToken]
/// Instead, each method must have all attributes, or do additional security checking.
/// Security checking is possible, because the cookie still contains user information
/// </remarks>
public interface IViewController
{
    /// <summary>
    /// Get the views of this App
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    IEnumerable<ViewDetailsDto> All(int appId);

    /// <summary>
    /// Find out how polymorphism is configured in this App.
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    PolymorphismDto Polymorphism(int appId);

    /// <summary>
    /// Delete a View
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    bool Delete(int appId, int id);

    /// <summary>
    /// Download / export a view as JSON to easily re-import into another App.
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="viewId"></param>
    /// <returns></returns>
    /// <remarks>
    /// New in 2sxc 11.07
    /// </remarks>
    THttpResponseType Json(int appId, int viewId);

    /// <summary>
    /// Used to be POST ImportExport/ImportContent
    /// </summary>
    /// <remarks>
    /// New in 2sxc 11.07
    /// </remarks>
    /// <returns></returns>
    ImportResultDto Import(int zoneId, int appId);

    /// <summary>
    /// Get usage statistics for entities so the UI can guide the user
    /// to find out if data is being used or if it can be safely deleted.
    /// </summary>
    /// <param name="appId">App ID</param>
    /// <param name="guid">Guid of the Entity</param>
    /// <returns></returns>
    /// <remarks>
    /// New in 2sxc 11.11
    /// </remarks>
    IEnumerable<ViewDto> Usage(int appId, Guid guid);
}