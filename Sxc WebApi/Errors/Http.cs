using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToSic.Eav.Security.Permissions;

namespace ToSic.SexyContent.WebApi.Errors
{
    internal class Http
    {
        /// <summary>
        /// Throw a correct HTTP error with the right error-numbr. This is important for the JavaScript which changes behavior & error messages based on http status code
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <param name="message"></param>
        /// <param name="tags"></param>
        internal static HttpResponseException WithLink(HttpStatusCode httpStatusCode, string message, string tags = "")
        {
            var helpText = " See http://2sxc.org/help" + (tags == "" ? "" : "?tag=" + tags);
            var err = new HttpResponseException(new HttpResponseMessage(httpStatusCode)
            {
                Content = new StringContent(message + helpText),
                ReasonPhrase = "Error in 2sxc Content API - not allowed"
            });
            return err;
        }

        internal static HttpResponseException BadRequest(string message)
            => new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                ReasonPhrase = "can't delete folder - not found in folder"
            });

        internal static HttpResponseException InformativeErrorForTypeAccessDenied(string contentType, List<Grants> grant, bool staticNameIsGuid)
        {
            var grantCodes = string.Join(",", grant);

            // if the cause was not-admin and not testable, report better error
            if (!staticNameIsGuid)
                return WithLink(HttpStatusCode.Unauthorized,
                    "Content Type '" + contentType + "' is not a standard Content Type - no permissions possible.");

            // final case: simply not allowed
            return WithLink(HttpStatusCode.Unauthorized,
                "Request not allowed. User needs permissions to " + grantCodes + " for Content Type '" + contentType + "'.",
                "permissions");
        }

        internal static HttpResponseException NotAllowedFileType(string filename, string message = null)
        {
            return new HttpResponseException(new HttpResponseMessage(HttpStatusCode.UnsupportedMediaType)
            {
                ReasonPhrase = $"file {filename} has an unsupported file type. {message}"
            });
        }
    }
}
