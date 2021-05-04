using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

// The commands here should help Dnn WebAPIs 

// ReSharper disable once CheckNamespace
namespace Custom.Hybrid
{
    public abstract partial class Api12
    {

        /// <summary>
        /// Creates a <see cref="OkResult"/> object that produces an empty <see cref="StatusCodes.Status200OK"/> response.
        /// </summary>
        /// <returns>The created <see cref="OkResult"/> for the response.</returns>
        [NonAction]
        public dynamic Ok()
        {
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates an <see cref="OkObjectResult"/> object that produces an <see cref="StatusCodes.Status200OK"/> response.
        /// </summary>
        /// <param name="value">The content value to format in the entity body.</param>
        /// <returns>The created <see cref="OkObjectResult"/> for the response.</returns>
        [NonAction]
        // maybe ??? low priority
        public dynamic Ok(object value)
        {
            return Request.CreateResponse(HttpStatusCode.OK, value);
        }

        /// <summary>
        /// Creates a <see cref="NoContentResult"/> object that produces an empty
        /// <see cref="StatusCodes.Status204NoContent"/> response.
        /// </summary>
        /// <returns>The created <see cref="NoContentResult"/> object for the response.</returns>
        [NonAction]
        public dynamic NoContent()
        {
            return Request.CreateResponse(HttpStatusCode.NoContent);
        }

        #region Content (ca. 5 overloads) can't be implemented, because it conflicts with our property "Content"
        //public dynamic Content(string content)
        //{
        //    throw new NotImplementedException();
        //}

        //public dynamic Content(string content, string contentType)
        //{

        //}
        #endregion

        #region Redirect Variants
        // Note that .net core has ca. 30 variants - we probably just need 2

        /// <summary>
        /// Creates a <see cref="RedirectResult"/> object that redirects (<see cref="StatusCodes.Status302Found"/>)
        /// to the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">The URL to redirect to.</param>
        /// <returns>The created <see cref="RedirectResult"/> for the response.</returns>
        [NonAction]
        public dynamic Redirect(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
            var response = Request.CreateResponse(HttpStatusCode.Redirect);
            response.Headers.Location = new Uri(url);
            return response;
        }

        /// <summary>
        /// Creates a <see cref="RedirectResult"/> object with <see cref="RedirectResult.Permanent"/> set to true
        /// (<see cref="StatusCodes.Status301MovedPermanently"/>) using the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">The URL to redirect to.</param>
        /// <returns>The created <see cref="RedirectResult"/> for the response.</returns>
        [NonAction]
        public dynamic RedirectPermanent(string url)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentNullException(nameof(url));
            var response = Request.CreateResponse(HttpStatusCode.MovedPermanently);
            response.Headers.Location = new Uri(url);
            return response;
        }



        #endregion



        #region StatusCode - low priority
        /// <summary>
        /// Creates a <see cref="StatusCodeResult"/> object by specifying a <paramref name="statusCode"/>.
        /// </summary>
        /// <param name="statusCode">The status code to set on the response.</param>
        /// <returns>The created <see cref="StatusCodeResult"/> object for the response.</returns>
        [NonAction]
        public dynamic StatusCode(int statusCode)
        {
            return Request.CreateResponse((HttpStatusCode)statusCode);
        }

        /// <summary>
        /// Creates a <see cref="ObjectResult"/> object by specifying a <paramref name="statusCode"/> and <paramref name="value"/>
        /// </summary>
        /// <param name="statusCode">The status code to set on the response.</param>
        /// <param name="value">The value to set on the <see cref="ObjectResult"/>.</param>
        /// <returns>The created <see cref="ObjectResult"/> object for the response.</returns>
        [NonAction]
        public dynamic StatusCode(int statusCode, object value)
            => Request.CreateResponse((HttpStatusCode)statusCode, value);
        #endregion


        /// <summary>
        /// Creates an <see cref="UnauthorizedResult"/> that produces an <see cref="StatusCodes.Status401Unauthorized"/> response.
        /// </summary>
        /// <returns>The created <see cref="UnauthorizedResult"/> for the response.</returns>
        [NonAction]
        public dynamic Unauthorized()
            => Request.CreateResponse(HttpStatusCode.Unauthorized);

        /// <summary>
        /// Creates an <see cref="UnauthorizedObjectResult"/> that produces a <see cref="StatusCodes.Status401Unauthorized"/> response.
        /// </summary>
        /// <returns>The created <see cref="UnauthorizedObjectResult"/> for the response.</returns>
        [NonAction]
        public dynamic Unauthorized(object value)
            => Request.CreateResponse(HttpStatusCode.Unauthorized, value);

        /// <summary>
        /// Creates an <see cref="NotFoundResult"/> that produces a <see cref="StatusCodes.Status404NotFound"/> response.
        /// </summary>
        /// <returns>The created <see cref="NotFoundResult"/> for the response.</returns>
        [NonAction]
        public dynamic NotFound()
            => Request.CreateResponse(HttpStatusCode.NotFound);

        /// <summary>
        /// Creates an <see cref="NotFoundObjectResult"/> that produces a <see cref="StatusCodes.Status404NotFound"/> response.
        /// </summary>
        /// <returns>The created <see cref="NotFoundObjectResult"/> for the response.</returns>
        [NonAction]
        public dynamic NotFound(object value)
            => Request.CreateResponse(HttpStatusCode.NotFound, value);

        /// <summary>
        /// Creates an <see cref="BadRequestResult"/> that produces a <see cref="StatusCodes.Status400BadRequest"/> response.
        /// </summary>
        /// <returns>The created <see cref="BadRequestResult"/> for the response.</returns>
        [NonAction]
        public dynamic BadRequest()
            => Request.CreateResponse(HttpStatusCode.BadRequest);

        ///// <summary>
        ///// Creates an <see cref="BadRequestObjectResult"/> that produces a <see cref="StatusCodes.Status400BadRequest"/> response.
        ///// </summary>
        ///// <param name="error">An error object to be returned to the client.</param>
        ///// <returns>The created <see cref="BadRequestObjectResult"/> for the response.</returns>
        //[NonAction]
        //public dynamic BadRequest(object error)
        //    => throw new NotImplementedException();

        ///// <summary>
        ///// Creates an <see cref="BadRequestObjectResult"/> that produces a <see cref="StatusCodes.Status400BadRequest"/> response.
        ///// </summary>
        ///// <param name="modelState">The <see cref="ModelStateDictionary" /> containing errors to be returned to the client.</param>
        ///// <returns>The created <see cref="BadRequestObjectResult"/> for the response.</returns>
        //[NonAction]
        //public dynamic BadRequest(object modelState)
        //    => throw new NotImplementedException();

        #region
        /// <summary>
        /// Creates an <see cref="ConflictResult"/> that produces a <see cref="StatusCodes.Status409Conflict"/> response.
        /// </summary>
        /// <returns>The created <see cref="ConflictResult"/> for the response.</returns>
        [NonAction]
        public dynamic Conflict()
            => Request.CreateResponse(HttpStatusCode.Conflict);

        /// <summary>
        /// Creates an <see cref="ConflictObjectResult"/> that produces a <see cref="StatusCodes.Status409Conflict"/> response.
        /// </summary>
        /// <param name="error">Contains errors to be returned to the client.</param>
        /// <returns>The created <see cref="ConflictObjectResult"/> for the response.</returns>
        [NonAction]
        public dynamic Conflict(object error)
            => Request.CreateResponse(HttpStatusCode.Conflict, error);

        ///// <summary>
        ///// Creates an <see cref="ConflictObjectResult"/> that produces a <see cref="StatusCodes.Status409Conflict"/> response.
        ///// </summary>
        ///// <param name="modelState">The <see cref="ModelStateDictionary" /> containing errors to be returned to the client.</param>
        ///// <returns>The created <see cref="ConflictObjectResult"/> for the response.</returns>
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
        /// Creates a <see cref="AcceptedResult"/> object that produces an <see cref="StatusCodes.Status202Accepted"/> response.
        /// </summary>
        /// <returns>The created <see cref="AcceptedResult"/> for the response.</returns>
        [NonAction]
        public dynamic Accepted()
            => Request.CreateResponse(HttpStatusCode.Accepted);

        // All other accepted - don't implement
        ///// <summary>
        ///// Creates a <see cref="AcceptedResult"/> object that produces an <see cref="StatusCodes.Status202Accepted"/> response.
        ///// </summary>
        ///// <param name="value">The optional content value to format in the entity body; may be null.</param>
        ///// <returns>The created <see cref="AcceptedResult"/> for the response.</returns>
        //[NonAction]
        //public dynamic Accepted(object value)
        //    => new AcceptedResult(location: null, value: value);

        ///// <summary>
        ///// Creates a <see cref="AcceptedResult"/> object that produces an <see cref="StatusCodes.Status202Accepted"/> response.
        ///// </summary>
        ///// <param name="uri">The optional URI with the location at which the status of requested content can be monitored.
        ///// May be null.</param>
        ///// <returns>The created <see cref="AcceptedResult"/> for the response.</returns>
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
        ///// Creates a <see cref="AcceptedResult"/> object that produces an <see cref="StatusCodes.Status202Accepted"/> response.
        ///// </summary>
        ///// <param name="uri">The optional URI with the location at which the status of requested content can be monitored.
        ///// May be null.</param>
        ///// <returns>The created <see cref="AcceptedResult"/> for the response.</returns>
        //[NonAction]
        //public dynamic Accepted(string uri)
        //    => new AcceptedResult(location: uri, value: null);

        ///// <summary>
        ///// Creates a <see cref="AcceptedResult"/> object that produces an <see cref="StatusCodes.Status202Accepted"/> response.
        ///// </summary>
        ///// <param name="uri">The URI with the location at which the status of requested content can be monitored.</param>
        ///// <param name="value">The optional content value to format in the entity body; may be null.</param>
        ///// <returns>The created <see cref="AcceptedResult"/> for the response.</returns>
        //[NonAction]
        //public dynamic Accepted(string uri, object value)
        //    => new AcceptedResult(uri, value);

        ///// <summary>
        ///// Creates a <see cref="AcceptedResult"/> object that produces an <see cref="StatusCodes.Status202Accepted"/> response.
        ///// </summary>
        ///// <param name="uri">The URI with the location at which the status of requested content can be monitored.</param>
        ///// <param name="value">The optional content value to format in the entity body; may be null.</param>
        ///// <returns>The created <see cref="AcceptedResult"/> for the response.</returns>
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
        ///// Creates a <see cref="ChallengeResult"/>.
        ///// </summary>
        ///// <returns>The created <see cref="ChallengeResult"/> for the response.</returns>
        ///// <remarks>
        ///// The behavior of this method depends on the <see cref="IAuthenticationService"/> in use.
        ///// <see cref="StatusCodes.Status401Unauthorized"/> and <see cref="StatusCodes.Status403Forbidden"/>
        ///// are among likely status results.
        ///// </remarks>
        //[NonAction]
        //public dynamic Challenge()
        //    => Request.CreateResponse(HttpStatusCode.Forbidden);
        #endregion

        #region Forbid - only one implemented
        /// <summary>
        /// Creates a <see cref="ForbidResult"/> (<see cref="StatusCodes.Status403Forbidden"/> by default).
        /// </summary>
        /// <returns>The created <see cref="ForbidResult"/> for the response.</returns>
        /// <remarks>
        /// Some authentication schemes, such as cookies, will convert <see cref="StatusCodes.Status403Forbidden"/> to
        /// a redirect to show a login page.
        /// </remarks>
        [NonAction]
        public dynamic Forbid()
            => Request.CreateResponse(HttpStatusCode.Forbidden);

        #endregion
    }
}
