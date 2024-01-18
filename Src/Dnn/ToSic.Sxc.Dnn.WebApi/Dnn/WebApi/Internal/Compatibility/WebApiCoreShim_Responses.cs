using System.Net;

// The commands here should help Dnn WebAPIs 

namespace ToSic.Sxc.Dnn.WebApi.Internal.Compatibility;

partial class WebApiCoreShim
{

    /// <summary>
    /// Creates a .net-core like `OkResult` object that produces an empty .net-core like `StatusCodes.Status200OK` response.
    ///
    /// Typical use: `return Ok();`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <returns>The created .net-core like `OkResult` for the response.</returns>
    [NonAction]
    public HttpResponseMessage Ok() => Request.CreateResponse(HttpStatusCode.OK);

    /// <summary>
    /// Creates an .net-core like `OkObjectResult` object that produces an .net-core like `StatusCodes.Status200OK` response.
    ///
    /// Typical use: `return Ok(objectToInclude);`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <param name="value">The content value to format in the entity body.</param>
    /// <returns>The created .net-core like `OkObjectResult` for the response.</returns>
    [NonAction]
    // maybe ??? low priority
    public HttpResponseMessage Ok(object value) => Request.CreateResponse(HttpStatusCode.OK, value);

    /// <summary>
    /// Creates a .net-core like `NoContentResult` object that produces an empty
    /// .net-core like `StatusCodes.Status204NoContent` response.
    ///
    /// Typical use: `return NoContent();`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <returns>The created .net-core like `NoContentResult` object for the response.</returns>
    [NonAction]
    public HttpResponseMessage NoContent() => Request.CreateResponse(HttpStatusCode.NoContent);

    // TODO: this Shim could now be implemented after 16.02 - since we don't have the Content property any more
    #region Content (ca. 5 overloads) can't be implemented, because it conflicts with our property "Content"
    //public dynamic Content(string content)
    //{
    //    throw new NotSupportedException();
    //}

    //public dynamic Content(string content, string contentType)
    //{

    //}
    #endregion

    #region Redirect Variants
    // Note that .net core has ca. 30 variants - we probably just need 2

    /// <summary>
    /// Creates a .net-core like `RedirectResult` object that redirects (.net-core like `StatusCodes.Status302Found`)
    /// to the specified <paramref name="url"/>.
    ///
    /// Typical use: `return Redirect("https://2sxc.org");`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <param name="url">The URL to redirect to.</param>
    /// <returns>The created .net-core like `RedirectResult` for the response.</returns>
    [NonAction]
    public HttpResponseMessage Redirect(string url)
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
        var response = Request.CreateResponse(HttpStatusCode.Redirect);
        response.Headers.Location = new(url);
        return response;
    }

    /// <summary>
    /// Creates a .net-core like `RedirectResult` object with .net-core like `RedirectResult.Permanent` set to true
    /// (.net-core like `StatusCodes.Status301MovedPermanently`) using the specified <paramref name="url"/>.
    ///
    /// Typical use: `return RedirectPermanent("https://2sxc.org");`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <param name="url">The URL to redirect to.</param>
    /// <returns>The created .net-core like `RedirectResult` for the response.</returns>
    [NonAction]
    public HttpResponseMessage RedirectPermanent(string url)
    {
        if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
        var response = Request.CreateResponse(HttpStatusCode.MovedPermanently);
        response.Headers.Location = new(url);
        return response;
    }



    #endregion



    #region StatusCode - low priority
    /// <summary>
    /// Creates a .net-core like `StatusCodeResult` object by specifying a <paramref name="statusCode"/>.
    ///
    /// Typical use: `return StatusCode(403);`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <param name="statusCode">The status code to set on the response.</param>
    /// <returns>The created .net-core like `StatusCodeResult` object for the response.</returns>
    [NonAction]
    public HttpResponseMessage StatusCode(int statusCode) => Request.CreateResponse((HttpStatusCode)statusCode);

    /// <summary>
    /// Creates a .net-core like `ObjectResult` object by specifying a <paramref name="statusCode"/> and <paramref name="value"/>
    ///
    /// Typical use: `return StatusCode(304, "not modified");`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <param name="statusCode">The status code to set on the response.</param>
    /// <param name="value">The value to set on the .net-core like `ObjectResult"/>.</param>
    /// <returns>The created .net-core like `ObjectResult` object for the response.</returns>
    [NonAction]
    public HttpResponseMessage StatusCode(int statusCode, object value) => Request.CreateResponse((HttpStatusCode)statusCode, value);
    #endregion


    /// <summary>
    /// Creates an .net-core like `UnauthorizedResult` that produces an .net-core like `StatusCodes.Status401Unauthorized` response.
    ///
    /// Typical use: `return Unauthorized();`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <returns>The created .net-core like `UnauthorizedResult` for the response.</returns>
    [NonAction]
    public HttpResponseMessage Unauthorized() => Request.CreateResponse(HttpStatusCode.Unauthorized);

    /// <summary>
    /// Creates an .net-core like `UnauthorizedObjectResult` that produces a .net-core like `StatusCodes.Status401Unauthorized` response.
    ///
    /// Typical use: `return Unauthorized("we don't like this");`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <returns>The created .net-core like `UnauthorizedObjectResult` for the response.</returns>
    [NonAction]
    public HttpResponseMessage Unauthorized(object value) => Request.CreateResponse(HttpStatusCode.Unauthorized, value);

    /// <summary>
    /// Creates an .net-core like `NotFoundResult` that produces a .net-core like `StatusCodes.Status404NotFound` response.
    ///
    /// Typical use: `return NotFound();`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <returns>The created .net-core like `NotFoundResult` for the response.</returns>
    [NonAction]
    public HttpResponseMessage NotFound() => Request.CreateResponse(HttpStatusCode.NotFound);

    /// <summary>
    /// Creates an .net-core like `NotFoundObjectResult` that produces a .net-core like `StatusCodes.Status404NotFound` response.
    ///
    /// Typical use: `return Unauthorized("try another ID");`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <returns>The created .net-core like `NotFoundObjectResult` for the response.</returns>
    [NonAction]
    public HttpResponseMessage NotFound(object value) => Request.CreateResponse(HttpStatusCode.NotFound, value);

    /// <summary>
    /// Creates an .net-core like `BadRequestResult` that produces a .net-core like `StatusCodes.Status400BadRequest` response.
    ///
    /// Typical use: `return BadRequest();`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <returns>The created .net-core like `BadRequestResult` for the response.</returns>
    [NonAction]
    public HttpResponseMessage BadRequest() => Request.CreateResponse(HttpStatusCode.BadRequest);

    ///// <summary>
    ///// Creates an .net-core like `BadRequestObjectResult` that produces a .net-core like `StatusCodes.Status400BadRequest` response.
    ///// </summary>
    ///// <param name="error">An error object to be returned to the client.</param>
    ///// <returns>The created .net-core like `BadRequestObjectResult` for the response.</returns>
    //[NonAction]
    //public dynamic BadRequest(object error)
    //    => throw new NotSupportedException();

    ///// <summary>
    ///// Creates an .net-core like `BadRequestObjectResult` that produces a .net-core like `StatusCodes.Status400BadRequest` response.
    ///// </summary>
    ///// <param name="modelState">The .net-core like `ModelStateDictionary" /> containing errors to be returned to the client.</param>
    ///// <returns>The created .net-core like `BadRequestObjectResult` for the response.</returns>
    //[NonAction]
    //public dynamic BadRequest(object modelState)
    //    => throw new NotSupportedException();

    #region Conflict

    /// <summary>
    /// Creates an .net-core like `ConflictResult` that produces a .net-core like `StatusCodes.Status409Conflict` response.
    ///
    /// Typical use: `return Conflict();`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <returns>The created .net-core like `ConflictResult` for the response.</returns>
    [NonAction]
    public HttpResponseMessage Conflict() => Request.CreateResponse(HttpStatusCode.Conflict);

    /// <summary>
    /// Creates an .net-core like `ConflictObjectResult` that produces a .net-core like `StatusCodes.Status409Conflict` response.
    ///
    /// Typical use: `return Conflict("the stored file is newer");`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <param name="error">Contains errors to be returned to the client.</param>
    /// <returns>The created .net-core like `ConflictObjectResult` for the response.</returns>
    [NonAction]
    public HttpResponseMessage Conflict(object error) => Request.CreateResponse(HttpStatusCode.Conflict, error);

    ///// <summary>
    ///// Creates an .net-core like `ConflictObjectResult` that produces a .net-core like `StatusCodes.Status409Conflict` response.
    ///// </summary>
    ///// <param name="modelState">The .net-core like `ModelStateDictionary" /> containing errors to be returned to the client.</param>
    ///// <returns>The created .net-core like `ConflictObjectResult` for the response.</returns>
    //[NonAction]
    //public dynamic Conflict(ModelStateDictionary modelState)
    //    => new ConflictObjectResult(modelState);


    #endregion

    #region Problem, ValidationProblem - won't implement
    #endregion

    #region Created, CreatedAtAction, CreatedAtRoute - won't impelment
    #endregion

    #region Accepted

    /// <summary>
    /// Creates a .net-core like `AcceptedResult` object that produces an .net-core like `StatusCodes.Status202Accepted` response.
    ///
    /// Typical use: `return Accepted();`
    /// </summary>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    /// </remarks>
    /// <returns>The created .net-core like `AcceptedResult` for the response.</returns>
    [NonAction]
    public HttpResponseMessage Accepted() => Request.CreateResponse(HttpStatusCode.Accepted);

    // All other accepted - don't implement
    ///// <summary>
    ///// Creates a .net-core like `AcceptedResult` object that produces an .net-core like `StatusCodes.Status202Accepted` response.
    ///// </summary>
    ///// <param name="value">The optional content value to format in the entity body; may be null.</param>
    ///// <returns>The created .net-core like `AcceptedResult` for the response.</returns>
    //[NonAction]
    //public dynamic Accepted(object value)
    //    => new AcceptedResult(location: null, value: value);

    ///// <summary>
    ///// Creates a .net-core like `AcceptedResult` object that produces an .net-core like `StatusCodes.Status202Accepted` response.
    ///// </summary>
    ///// <param name="uri">The optional URI with the location at which the status of requested content can be monitored.
    ///// May be null.</param>
    ///// <returns>The created .net-core like `AcceptedResult` for the response.</returns>
    //[NonAction]
    //public dynamic Accepted(Uri uri)
    //{
    //    if (uri == null)
    //    {
    //        throw new ArgumentNullException(nameof(uri));
    //    }

    //    return new AcceptedResult(locationUri: uri, value: null);
    //}

    ///// <summary>
    ///// Creates a .net-core like `AcceptedResult` object that produces an .net-core like `StatusCodes.Status202Accepted` response.
    ///// </summary>
    ///// <param name="uri">The optional URI with the location at which the status of requested content can be monitored.
    ///// May be null.</param>
    ///// <returns>The created .net-core like `AcceptedResult` for the response.</returns>
    //[NonAction]
    //public dynamic Accepted(string uri)
    //    => new AcceptedResult(location: uri, value: null);

    ///// <summary>
    ///// Creates a .net-core like `AcceptedResult` object that produces an .net-core like `StatusCodes.Status202Accepted` response.
    ///// </summary>
    ///// <param name="uri">The URI with the location at which the status of requested content can be monitored.</param>
    ///// <param name="value">The optional content value to format in the entity body; may be null.</param>
    ///// <returns>The created .net-core like `AcceptedResult` for the response.</returns>
    //[NonAction]
    //public dynamic Accepted(string uri, object value)
    //    => new AcceptedResult(uri, value);

    ///// <summary>
    ///// Creates a .net-core like `AcceptedResult` object that produces an .net-core like `StatusCodes.Status202Accepted` response.
    ///// </summary>
    ///// <param name="uri">The URI with the location at which the status of requested content can be monitored.</param>
    ///// <param name="value">The optional content value to format in the entity body; may be null.</param>
    ///// <returns>The created .net-core like `AcceptedResult` for the response.</returns>
    //[NonAction]
    //public dynamic Accepted(Uri uri, object value)
    //{
    //    if (uri == null)
    //    {
    //        throw new ArgumentNullException(nameof(uri));
    //    }

    //    return new AcceptedResult(locationUri: uri, value: value);
    //}
    #endregion

    #region Challenge - only one implemented
    ///// <summary>
    ///// Creates a .net-core like `ChallengeResult"/>.
    ///// </summary>
    ///// <returns>The created .net-core like `ChallengeResult` for the response.</returns>
    ///// <remarks>
    ///// The behavior of this method depends on the .net-core like `IAuthenticationService` in use.
    ///// .net-core like `StatusCodes.Status401Unauthorized` and .net-core like `StatusCodes.Status403Forbidden"/>
    ///// are among likely status results.
    ///// </remarks>
    //[NonAction]
    //public dynamic Challenge()
    //    => Request.CreateResponse(HttpStatusCode.Forbidden);
    #endregion

    #region Forbid - only one implemented
    /// <summary>
    /// Creates a .net-core like `ForbidResult` (.net-core like `StatusCodes.Status403Forbidden` by default).
    ///
    /// Typical use: `return Forbid();`
    /// </summary>
    /// <returns>The created .net-core like `ForbidResult` for the response.</returns>
    /// <remarks>
    /// This is a shim to ensure that .net Framework code can be written the same way as .net core WebApis.
    /// It returns a `dynamic` to make it easy to use, but the real .net core implementation returns a typed object.
    ///
    /// Some authentication schemes, such as cookies, will convert .net-core like `StatusCodes.Status403Forbidden` to
    /// a redirect to show a login page.
    /// </remarks>
    [NonAction]
    public HttpResponseMessage Forbid() => Request.CreateResponse(HttpStatusCode.Forbidden);

    #endregion
}