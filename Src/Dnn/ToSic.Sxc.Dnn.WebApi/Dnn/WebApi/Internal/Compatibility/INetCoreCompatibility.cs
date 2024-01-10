namespace ToSic.Sxc.Dnn.WebApi.Internal.Compatibility;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface INetCoreCompatibility
{
    /// <inheritdoc cref="WebApiCoreShim.Ok()"/>
    dynamic Ok();

    /// <inheritdoc cref="WebApiCoreShim.Ok(object)"/>
    dynamic Ok(object value);

    /// <inheritdoc cref="WebApiCoreShim.NoContent()"/>
    dynamic NoContent();

    /// <inheritdoc cref="WebApiCoreShim.Redirect"/>
    dynamic Redirect(string url);

    /// <inheritdoc cref="WebApiCoreShim.RedirectPermanent"/>
    dynamic RedirectPermanent(string url);

    /// <inheritdoc cref="WebApiCoreShim.StatusCode(int)"/>
    dynamic StatusCode(int statusCode);

    /// <inheritdoc cref="WebApiCoreShim.StatusCode(int, object)"/>
    dynamic StatusCode(int statusCode, object value);

    /// <inheritdoc cref="WebApiCoreShim.Unauthorized()"/>
    dynamic Unauthorized();

    /// <inheritdoc cref="WebApiCoreShim.Unauthorized(object)"/>
    dynamic Unauthorized(object value);

    /// <inheritdoc cref="WebApiCoreShim.NotFound()"/>
    dynamic NotFound();

    /// <inheritdoc cref="WebApiCoreShim.NotFound(object)"/>
    dynamic NotFound(object value);

    /// <inheritdoc cref="WebApiCoreShim.BadRequest()"/>
    dynamic BadRequest();

    /// <inheritdoc cref="WebApiCoreShim.Conflict()"/>
    dynamic Conflict();

    /// <inheritdoc cref="WebApiCoreShim.Conflict(object)"/>
    dynamic Conflict(object error);

    /// <inheritdoc cref="WebApiCoreShim.Accepted()"/>
    dynamic Accepted();

    /// <inheritdoc cref="WebApiCoreShim.Forbid()"/>
    dynamic Forbid();
}