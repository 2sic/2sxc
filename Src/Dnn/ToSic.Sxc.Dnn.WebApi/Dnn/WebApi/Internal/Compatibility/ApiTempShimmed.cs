using System.IO;
using ToSic.Lib.Coding;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Code.Internal;

namespace ToSic.Sxc.Dnn.WebApi.Internal.Compatibility;

/// <summary>
/// </summary>
[DnnLogExceptions]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal abstract class ApiTempShimmed : DnnSxcCustomControllerBase, IDynamicWebApi, IHasCodeApiService, INetCoreCompatibility
{

    #region Net Core Compatibility Shims - Copy this entire section to WebApi Files

    public IFile SaveInAdam(NoParamOrder noParamOrder = default, Stream stream = null, string fileName = null,
        string contentType = null, Guid? guid = null, string field = null, string subFolder = "")
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc cref="IDynamicWebApi.File"/>
    public dynamic File(NoParamOrder noParamOrder = default,
        bool? download = null,
        string virtualPath = null,
        string contentType = null,
        string fileDownloadName = null,
        object contents = null)
        => Shim.File(noParamOrder, download, virtualPath, contentType, fileDownloadName, contents);

    private WebApiCoreShim Shim => new(Request);

    /// <inheritdoc cref="WebApiCoreShim.Ok()"/>
    [NonAction] public new dynamic Ok() => Shim.Ok();

    /// <inheritdoc cref="WebApiCoreShim.Ok(object)"/>
    [NonAction] public dynamic Ok(object value) => Shim.Ok(value);

    /// <inheritdoc cref="WebApiCoreShim.NoContent()"/>
    [NonAction]
    public dynamic NoContent() => Shim.NoContent();

    // TODO: this Shim could now be implemented after 16.02 - since we don't have the Content property any more
    #region Content (ca. 5 overloads) can't be implemented, because it conflicts with our property "Content"

    #endregion

    /// <inheritdoc cref="WebApiCoreShim.Redirect"/>
    [NonAction] public new dynamic Redirect(string url) => Shim.Redirect(url);

    /// <inheritdoc cref="WebApiCoreShim.RedirectPermanent"/>
    [NonAction] public dynamic RedirectPermanent(string url) => Shim.RedirectPermanent(url);


    /// <inheritdoc cref="WebApiCoreShim.StatusCode(int)"/>
    [NonAction] public dynamic StatusCode(int statusCode) => Shim.StatusCode(statusCode);

    /// <inheritdoc cref="WebApiCoreShim.StatusCode(int, object)"/>
    [NonAction] public dynamic StatusCode(int statusCode, object value) => Shim.StatusCode(statusCode, value);


    /// <inheritdoc cref="WebApiCoreShim.Unauthorized()"/>
    [NonAction] public dynamic Unauthorized() => Shim.Unauthorized();

    /// <inheritdoc cref="WebApiCoreShim.Unauthorized(object)"/>
    [NonAction] public dynamic Unauthorized(object value) => Shim.Unauthorized(value);

    /// <inheritdoc cref="WebApiCoreShim.NotFound()"/>
    [NonAction] public new dynamic NotFound() => Shim.NotFound();

    /// <inheritdoc cref="WebApiCoreShim.NotFound(object)"/>
    [NonAction] public dynamic NotFound(object value) => Shim.NotFound(value);

    /// <inheritdoc cref="WebApiCoreShim.BadRequest()"/>
    [NonAction] public new dynamic BadRequest() => Shim.BadRequest();

    /// <inheritdoc cref="WebApiCoreShim.Conflict()"/>
    [NonAction] public new dynamic Conflict() => Shim.Conflict();

    /// <inheritdoc cref="WebApiCoreShim.Conflict(object)"/>
    [NonAction] public dynamic Conflict(object error) => Shim.Conflict(error);

    /// <inheritdoc cref="WebApiCoreShim.Accepted()"/>
    [NonAction] public dynamic Accepted() => Shim.Accepted();

    /// <inheritdoc cref="WebApiCoreShim.Forbid()"/>
    [NonAction] public dynamic Forbid() => Shim.Forbid();

    #endregion
}